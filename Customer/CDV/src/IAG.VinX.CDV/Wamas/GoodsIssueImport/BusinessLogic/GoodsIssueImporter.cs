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
using IAG.VinX.CDV.Wamas.GoodsIssueImport.Dto;

namespace IAG.VinX.CDV.Wamas.GoodsIssueImport.BusinessLogic;

public class GoodsIssueImporter : BaseImporter, IGoodsIssueImporter
{
    public GoodsIssueImporter(
        ISybaseConnectionFactory databaseConnectionFactory, IFtpConnector ftpConnector)
        : base(databaseConnectionFactory, ftpConnector)
    {
    }
    
    protected override Tuple<Type, string>[] RecordTypeMappings => new[]
    {
        new Tuple<Type, string>(typeof(GoodsIssue), ResourceIds.WamasGoodsIssueRecordType),
        new Tuple<Type, string>(typeof(GoodsIssueLine), ResourceIds.WamasGoodsIssueLineRecordType),
        new Tuple<Type, string>(typeof(GoodsIssueLineDifference), ResourceIds.WamasGoodsIssueLineDifferenceRecordType)
    };

    public new void SetConfig(
        WamasFtpConfig wamasFtpConfig,
        string connectionString,
        IMessageLogger messageLogger)
    {
        base.SetConfig(wamasFtpConfig, connectionString, messageLogger);
    }

    public WamasImportJobResult ImportGoodsIssues()
    {
        var jobResult = new WamasImportJobResult();

        try
        {
            var filesToImport = SearchFilesToImport(ResourceIds.WamasGoodsIssueRecordType);

            foreach (var file in filesToImport)
            {
                try
                {
                    var records = DownloadAndDeserialize(file);
                    var goodsIssues = CastRecords(records);
                    var partialJobResult = Import(goodsIssues, file);

                    if (partialJobResult.Result != JobResultEnum.PartialSuccess)
                        ArchiveFile(file, partialJobResult.Result == JobResultEnum.Success);

                    jobResult.ImportedCount += partialJobResult.ImportedCount;
                    jobResult.ErrorCount += partialJobResult.ErrorCount;
                }
                catch (Exception e)
                {
                    jobResult.ErrorCount++;

                    MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasGoodsIssuesImportFileError, 
                        file, e.Message);
                    ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasGoodsIssueImportErrorTitle,
                        string.Format(ResourceIds.WamasGoodsIssuesImportFileError, file, e.Message));
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
            
            MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasGoodsIssuesImportError, e.Message);
            ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasGoodsIssueImportErrorTitle,
                string.Format(ResourceIds.WamasGoodsIssuesImportError, e.Message));
        }
        finally
        {
            Dispose();
        }

        return jobResult;
    }
    
    private WamasImportJobResult Import(List<GoodsIssue> goodsIssues, string file)
    {
        var jobResult = new WamasImportJobResult();

        foreach (var goodsIssue in goodsIssues)
        {
            try
            {
                var pickListForGoodsIssue = DatabaseConnector.GetQueryable<Document>()
                    .FirstOrDefault(g => g.Id == int.Parse(goodsIssue.Id));

                // skip if picklist has been deleted (.?) has already been "copied to" to a delivery note or invoice
                if (pickListForGoodsIssue?.DocumentType is not ReceiptType.Confirmation)
                    continue;
                
                // skip if logistic state is already completed
                if (pickListForGoodsIssue.LogisticState is LogisticState.LogisticsCompleted
                    or LogisticState.LogisticsCompletedWithDifferences)
                    continue;
                
                DatabaseConnector.BeginTransaction();
                
                pickListForGoodsIssue.LogisticState = LogisticState.LogisticsCompleted;

                if (goodsIssue.Lines.Any())
                {
                    foreach (var goodsIssueLine in goodsIssue.Lines)
                    {
                        var pickListLineForGoodsIssue = DatabaseConnector.GetQueryable<ArticlePosition>()
                            .FirstOrDefault(ap =>
                                ap.DocumentId == int.Parse(goodsIssueLine.Id) &&
                                ap.Id == goodsIssueLine.LineId);

                        if (pickListLineForGoodsIssue == null)
                            continue;

                        decimal diffFromOrderedQuantity = 0;

                        if (goodsIssueLine.LineDifference != null)
                        {
                            var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };
                            diffFromOrderedQuantity = decimal.Parse(goodsIssueLine.LineDifference.DiffAmountBaseQuantity,
                                numberFormatInfo);
                        }

                        pickListLineForGoodsIssue.QuantityConfirmed =
                            pickListLineForGoodsIssue.Quantity + diffFromOrderedQuantity;

                        DatabaseConnector.Update(pickListLineForGoodsIssue);

                        if (diffFromOrderedQuantity < 0)
                            pickListForGoodsIssue.LogisticState = LogisticState.LogisticsCompletedWithDifferences;
                    }
                }

                DatabaseConnector.Update(pickListForGoodsIssue);
                DatabaseConnector.Commit();

                jobResult.ImportedCount++;
            }
            catch (Exception e)
            {
                jobResult.ErrorCount++;
                DatabaseConnector.Rollback();

                MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasGoodsIssueImportError, 
                    goodsIssue.Id, file, e.Message);
                ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasGoodsIssueImportErrorTitle,
                    string.Format(ResourceIds.WamasGoodsIssueImportError, goodsIssue.Id, file, e.Message));
            }
        }

        if (jobResult.ErrorCount > 0 && jobResult.ImportedCount == 0)
            jobResult.Result = JobResultEnum.Failed;
        else
            jobResult.Result = jobResult.ErrorCount > 0 ? JobResultEnum.PartialSuccess : JobResultEnum.Success;

        return jobResult;
    }
    
    private static List<GoodsIssue> CastRecords(List<GenericWamasRecord> records)
    {
        var goodsIssues = new List<GoodsIssue>();
        var goodsIssueLines = new List<GoodsIssueLine>();
        var goodsIssueLinesDifferences = new List<GoodsIssueLineDifference>();

        foreach (var record in records)
        {
            if (record.RecordType == typeof(GoodsIssue))
                goodsIssues.Add((GoodsIssue)record.RecordValue);
            else if (record.RecordType == typeof(GoodsIssueLine))
                goodsIssueLines.Add((GoodsIssueLine)record.RecordValue);
            else if (record.RecordType == typeof(GoodsIssueLineDifference))
                goodsIssueLinesDifferences.Add((GoodsIssueLineDifference)record.RecordValue);
        }

        foreach (var goodsIssue in goodsIssues)
        {
            var linesForGoodsIssue = goodsIssueLines
                .Where(l => int.Parse(l.Id) == int.Parse(goodsIssue.Id))
                .ToList();

            foreach (var line in linesForGoodsIssue)
            {
                var differencesForLine = goodsIssueLinesDifferences.FirstOrDefault(ld =>
                    ld.Id == line.Id &&
                    ld.LineId == line.LineId);

                line.LineDifference = differencesForLine;
            }

            goodsIssue.Lines = linesForGoodsIssue;
        }

        return goodsIssues;
    }
}