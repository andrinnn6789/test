using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.CDV.Resource;
using IAG.VinX.CDV.Wamas.Common.BusinessLogic;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.DataAccess;
using IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.GoodsReceiptImport.Dto;

namespace IAG.VinX.CDV.Wamas.GoodsReceiptImport.BusinessLogic;

public class GoodsReceiptImporter : BaseImporter, IGoodsReceiptImporter
{
    public GoodsReceiptImporter(
        ISybaseConnectionFactory databaseConnectionFactory, IFtpConnector ftpConnector)
        : base(databaseConnectionFactory, ftpConnector)
    {
    }
    
    protected override Tuple<Type, string>[] RecordTypeMappings => new[]
    {
        new Tuple<Type, string>(typeof(GoodsReceipt), ResourceIds.WamasGoodsReceiptRecordType),
        new Tuple<Type, string>(typeof(GoodsReceiptLine), ResourceIds.WamasGoodsReceiptLineRecordType),
        new Tuple<Type, string>(typeof(GoodsReceiptLineDifference), ResourceIds.WamasGoodsReceiptLineDifferenceRecordType)
    };

    public new void SetConfig(
        WamasFtpConfig wamasFtpConfig,
        string connectionString,
        IMessageLogger messageLogger)
    {
        base.SetConfig(wamasFtpConfig, connectionString, messageLogger);
    }

    public WamasImportJobResult ImportGoodsReceipts()
    {
        var jobResult = new WamasImportJobResult();

        try
        {
            var filesToImport = SearchFilesToImport(ResourceIds.WamasGoodsReceiptRecordType);

            foreach (var file in filesToImport)
            {
                try
                {
                    var records = DownloadAndDeserialize(file);
                    var goodsReceipts = CastRecords(records);
                    var partialJobResult = Import(goodsReceipts, file);

                    if (partialJobResult.Result != JobResultEnum.PartialSuccess)
                        ArchiveFile(file, partialJobResult.Result == JobResultEnum.Success);

                    jobResult.ImportedCount += partialJobResult.ImportedCount;
                    jobResult.ErrorCount += partialJobResult.ErrorCount;
                }
                catch (Exception e)
                {
                    jobResult.ErrorCount++;

                    MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasGoodsReceiptsImportFileError, file,
                        e.Message);
                    ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasGoodsReceiptImportErrorTitle,
                        string.Format(ResourceIds.WamasGoodsReceiptsImportFileError, file, e.Message));
                }
            }

            if (jobResult.ErrorCount > 0 && jobResult.ImportedCount == 0)
                jobResult.Result = JobResultEnum.Failed;
            else
                jobResult.Result = jobResult.ErrorCount > 0 ? JobResultEnum.PartialSuccess : JobResultEnum.Success;
        }
        catch (Exception e)
        {
            jobResult.Result = JobResultEnum.Failed;
            jobResult.ErrorCount++;
            
            MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasGoodsReceiptsImportError, e.Message);
            ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasGoodsReceiptImportErrorTitle,
                string.Format(ResourceIds.WamasGoodsReceiptsImportError, e.Message));
        }
        finally
        {
            Dispose();
        }

        return jobResult;
    }

    private WamasImportJobResult Import(List<GoodsReceipt> goodsReceipts, string file)
    {
        var jobResult = new WamasImportJobResult();

        foreach (var goodsReceipt in goodsReceipts)
        {
            try
            {
                var purchaseOrderForGoodsReceipt = DatabaseConnector.GetQueryable<Document>()
                    .FirstOrDefault(g => g.Id == int.Parse(goodsReceipt.Id));

                // skip if purchase order has been deleted (.?) has already been "copied to" to a warehouse entry or invoice
                if (purchaseOrderForGoodsReceipt?.DocumentType is not ReceiptType.Order)
                    continue;
                
                // skip if logistic state is already completed
                if (purchaseOrderForGoodsReceipt.LogisticState is LogisticState.LogisticsCompleted
                    or LogisticState.LogisticsCompletedWithDifferences)
                    continue;
                
                DatabaseConnector.BeginTransaction();
                
                purchaseOrderForGoodsReceipt.LogisticState = LogisticState.LogisticsCompleted;

                if (goodsReceipt.Lines.Any())
                {
                    foreach (var goodsReceiptsLine in goodsReceipt.Lines)
                    {
                        var purchaseOrderLineForGoodsReceipt = DatabaseConnector.GetQueryable<ArticlePosition>()
                            .FirstOrDefault(ap =>
                                ap.DocumentId == int.Parse(goodsReceiptsLine.Id) &&
                                ap.Id == goodsReceiptsLine.LineId);

                        if (purchaseOrderLineForGoodsReceipt == null)
                            continue;
                        
                        decimal diffFromOrderedQuantity = 0;

                        if (goodsReceiptsLine.LineDifference != null)
                        {
                            var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };
                            diffFromOrderedQuantity = decimal.Parse(goodsReceiptsLine.LineDifference.DiffAmountBaseQuantity, 
                                numberFormatInfo);
                        }

                        purchaseOrderLineForGoodsReceipt.QuantityConfirmed =
                            purchaseOrderLineForGoodsReceipt.Quantity + diffFromOrderedQuantity;

                        DatabaseConnector.Update(purchaseOrderLineForGoodsReceipt);

                        if (diffFromOrderedQuantity < 0)
                            purchaseOrderForGoodsReceipt.LogisticState = LogisticState.LogisticsCompletedWithDifferences;
                    }
                }

                DatabaseConnector.Update(purchaseOrderForGoodsReceipt);
                DatabaseConnector.Commit();

                jobResult.ImportedCount++;
            }
            catch (Exception e)
            {
                jobResult.ErrorCount++;
                DatabaseConnector.Rollback();
                
                MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasGoodsReceiptImportError,
                    goodsReceipt.Id, file, e.Message);
                ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasGoodsReceiptImportErrorTitle,
                    string.Format(ResourceIds.WamasGoodsReceiptImportError, goodsReceipt.Id, file, e.Message));
            }
        }
        
        if (jobResult.ErrorCount > 0 && jobResult.ImportedCount == 0)
            jobResult.Result = JobResultEnum.Failed;
        else
            jobResult.Result = jobResult.ErrorCount > 0 ? JobResultEnum.PartialSuccess : JobResultEnum.Success;

        return jobResult;
    }
    
    private static List<GoodsReceipt> CastRecords(List<GenericWamasRecord> records)
    {
        var goodsReceipts = new List<GoodsReceipt>();
        var goodsReceiptsLines = new List<GoodsReceiptLine>();
        var goodsReceiptsLinesDifferences = new List<GoodsReceiptLineDifference>();

        foreach (var record in records)
        {
            if (record.RecordType == typeof(GoodsReceipt))
                goodsReceipts.Add((GoodsReceipt)record.RecordValue);
            else if (record.RecordType == typeof(GoodsReceiptLine))
                goodsReceiptsLines.Add((GoodsReceiptLine)record.RecordValue);
            else if (record.RecordType == typeof(GoodsReceiptLineDifference))
                goodsReceiptsLinesDifferences.Add((GoodsReceiptLineDifference)record.RecordValue);
        }

        foreach (var goodsReceipt in goodsReceipts)
        {
            var linesForGoodsReceipt = goodsReceiptsLines
                .Where(l => int.Parse(l.Id) == int.Parse(goodsReceipt.Id))
                .ToList();

            foreach (var goodsReceiptLine in linesForGoodsReceipt)
            {
                var differencesForLine = goodsReceiptsLinesDifferences.FirstOrDefault(ld =>
                    int.Parse(ld.Id) == int.Parse(goodsReceiptLine.Id) &&
                    ld.LineId == goodsReceiptLine.LineId);

                goodsReceiptLine.LineDifference = differencesForLine;
            }

            goodsReceipt.Lines = linesForGoodsReceipt;
        }

        return goodsReceipts;
    }
}