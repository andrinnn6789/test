using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.CDV.Resource;
using IAG.VinX.CDV.Wamas.Common.BusinessLogic;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.DataAccess;
using IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.StockAdjustmentImport.Dto;

namespace IAG.VinX.CDV.Wamas.StockAdjustmentImport.BusinessLogic;

public class StockAdjustmentImporter : BaseImporter, IStockAdjustmentImporter
{
    private const string InternalShiftOfGoodsKey = "2";
    
    protected override Tuple<Type, string>[] RecordTypeMappings => new[]
    {
        new Tuple<Type, string>(typeof(StockAdjustment), ResourceIds.WamasStockAdjustmentRecordType)
    };

    public StockAdjustmentImporter(
        ISybaseConnectionFactory databaseConnectionFactory, IFtpConnector ftpConnector)
        : base(databaseConnectionFactory, ftpConnector)
    {
    }

    public new void SetConfig(
        WamasFtpConfig wamasFtpConfig,
        string connectionString,
        IMessageLogger messageLogger)
    {
        base.SetConfig(wamasFtpConfig, connectionString, messageLogger);
    }

    public WamasImportJobResult ImportStockAdjustments()
    {
        var jobResult = new WamasImportJobResult();

        try
        {
            var filesToImport = SearchFilesToImport(ResourceIds.WamasStockAdjustmentRecordType);

            foreach (var file in filesToImport)
            {
                try
                {
                    var records = DownloadAndDeserialize(file);
                    var stockAdjustments = CastRecords(records);
                    var partialJobResult = Import(stockAdjustments, file);

                    if (partialJobResult.Result != JobResultEnum.PartialSuccess)
                        ArchiveFile(file, partialJobResult.Result == JobResultEnum.Success);

                    jobResult.ImportedCount += partialJobResult.ImportedCount;
                    jobResult.ErrorCount += partialJobResult.ErrorCount;
                }
                catch (Exception e)
                {
                    jobResult.ErrorCount++;
                    
                    MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasStockAdjustmentsImportFileError,
                        file, e.Message);
                    ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasStockAdjustmentImportErrorTitle,
                        string.Format(ResourceIds.WamasStockAdjustmentsImportFileError, file, e.Message));
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
            
            MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasStockAdjustmentsImportError, e.Message);
            ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasStockAdjustmentImportErrorTitle,
                string.Format(ResourceIds.WamasStockAdjustmentsImportError, e.Message));
        }
        finally
        {
            Dispose();
        }

        return jobResult;
    }

    private WamasImportJobResult Import(List<StockAdjustment> stockAdjustments, string file)
    {
        var jobResult = new WamasImportJobResult();

        foreach (var stockAdjustment in stockAdjustments)
        {
            // Interne Warenverschiebungen ignorieren
            if (stockAdjustment.PostingKey == InternalShiftOfGoodsKey)
                continue;
            
            var generatedStockAdjustmentId =
                $"WAMAS-{stockAdjustment.RecordDate:yyyyMMddhhmmss}{stockAdjustment.SerialNumber}{stockAdjustment.ItemNumber}";

            try
            {
                var articleId = GetArticleIdByNumber(stockAdjustment.ItemNumber);
                var warehouseId = GetWarehouseIdFromName(stockAdjustment.WarehouseLocation);
                var storageLocationId = GetStorageLocationIdFromDescription(stockAdjustment.StorageLocation);

                var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };
                var quantity = Math.Abs(decimal.Parse(stockAdjustment.BaseQuantity.Replace("+", ""), numberFormatInfo));
                var hasPackagingUnit = stockAdjustment.NumberOfBaseAndWarehouseQuantityUnit > 1;
                var packageUnitQuantity = quantity / stockAdjustment.NumberOfBaseAndWarehouseQuantityUnit;
                var stockMovementType = stockAdjustment.BaseQuantity.Contains('-')
                    ? StockMovementType.InventoryOutgoing
                    : StockMovementType.InventoryIncoming;
                var alreadyImportedAdjustments = DatabaseConnector.GetQueryable<StockMovement>().Where(s =>
                        s.ArticleId == articleId && s.MovementType == stockMovementType && s.Description == generatedStockAdjustmentId).ToList();

                if (alreadyImportedAdjustments.Any())
                    continue;

                var stockAdjustmentDbModel = new StockMovement
                {
                    ArticleId = decimal.ToInt32(articleId),
                    WarehouseId = decimal.ToInt32(warehouseId),
                    Date = stockAdjustment.RecordDate,
                    MovementType = stockMovementType,
                    Quantity = quantity,
                    Description = generatedStockAdjustmentId,
                    Chargeable = 0,
                    User = "Support",
                    ChangeDate = DateTime.Now,
                    ForOrderProposal = -1,
                    SectionId = 1,
                    ClientId = 1
                };

                if (storageLocationId.HasValue)
                    stockAdjustmentDbModel.StorageLocationId = storageLocationId.Value;

                if (hasPackagingUnit)
                    stockAdjustmentDbModel.PackagingQuantity = packageUnitQuantity;

                DatabaseConnector.BeginTransaction();
                DatabaseConnector.Insert(stockAdjustmentDbModel);
                DatabaseConnector.Commit();

                jobResult.ImportedCount++;
            }
            catch (Exception e)
            {
                jobResult.ErrorCount++;
                DatabaseConnector.Rollback();
                
                MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasStockAdjustmentImportError,
                    stockAdjustment.ItemNumber, stockAdjustment.RecordDate.ToString("dd.MM.yyy hh:mm:ss"), file, e.Message);
                ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasStockAdjustmentImportErrorTitle,
                    string.Format(ResourceIds.WamasStockAdjustmentImportError, stockAdjustment.ItemNumber,
                        stockAdjustment.RecordDate.ToString("dd.MM.yyy hh:mm:ss"), file, e.Message));
            }
        }

        if (jobResult.ErrorCount > 0 && jobResult.ImportedCount == 0)
            jobResult.Result = JobResultEnum.Failed;
        else
            jobResult.Result = jobResult.ErrorCount > 0 ? JobResultEnum.PartialSuccess : JobResultEnum.Success;

        return jobResult;
    }

    private int GetArticleIdByNumber(string articleNumber)
    {
        var articles = DatabaseConnector.GetQueryable<Article>()
            .Where(a => a.ArticleNumber == decimal.Parse(articleNumber)).ToList();
        return articles.First().Id;
    }

    private int GetWarehouseIdFromName(string warehouseName)
    {
        return DatabaseConnector.GetQueryable<Warehouse>().First(w => w.Name == warehouseName).Id;
    }

    private int? GetStorageLocationIdFromDescription(string storageLocationDescription)
    {
        return DatabaseConnector.GetQueryable<StorageLocation>().FirstOrDefault(w => w.Description == storageLocationDescription)?.Id;
    }
    
    private static List<StockAdjustment> CastRecords(List<GenericWamasRecord> records)
    {
        return records.Select(record => (StockAdjustment)record.RecordValue).ToList();
    }
}