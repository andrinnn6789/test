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
using IAG.VinX.CDV.Wamas.StockReportImport.Dto;

namespace IAG.VinX.CDV.Wamas.StockReportImport.BusinessLogic;

public class StockReportImporter : BaseImporter, IStockReportImporter
{
    public StockReportImporter(
        ISybaseConnectionFactory databaseConnectionFactory, IFtpConnector ftpConnector)
        : base(databaseConnectionFactory, ftpConnector)
    {
    }
    
    protected override Tuple<Type, string>[] RecordTypeMappings => new[]
    {
        new Tuple<Type, string>(typeof(StockReportBegin), ResourceIds.WamasStockReportBeginRecordType),
        new Tuple<Type, string>(typeof(StockReportAcknowledge), ResourceIds.WamasStockReportAcknowledgeRecordType),
        new Tuple<Type, string>(typeof(StockReportEnd), ResourceIds.WamasStockReportEndRecordType)
    };

    public new void SetConfig(
        WamasFtpConfig wamasFtpConfig,
        string connectionString,
        IMessageLogger messageLogger)
    {
        base.SetConfig(wamasFtpConfig, connectionString, messageLogger);
    }

    public WamasImportJobResult ImportStockReports()
    {
        var jobResult = new WamasImportJobResult();

        try
        {
            var filesToImport = SearchFilesToImport(ResourceIds.WamasStockReportBeginRecordType);

            foreach (var file in filesToImport)
            {
                try
                {
                    var records = DownloadAndDeserialize(file);
                    var stockReports = CastRecords(records);
                    var partialJobResult = Import(stockReports, file);

                    if (partialJobResult.Result != JobResultEnum.PartialSuccess)
                        ArchiveFile(file, partialJobResult.Result == JobResultEnum.Success);

                    jobResult.ImportedCount += partialJobResult.ImportedCount;
                    jobResult.ErrorCount += partialJobResult.ErrorCount;
                }
                catch (Exception e)
                {
                    jobResult.ErrorCount++;

                    MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasStockReportsImportFileError, 
                        file, e.Message);
                    ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasStockReportImportErrorTitle, 
                        string.Format(ResourceIds.WamasStockReportsImportFileError, file, e.Message));
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
            
            MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasStockReportsImportError, e.Message);
            ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasStockReportImportErrorTitle,
                string.Format(ResourceIds.WamasStockReportsImportError, e.Message));
        }
        finally
        {
            Dispose();
        }

        return jobResult;
    }

    private static List<StockReportBegin> CastRecords(List<GenericWamasRecord> records)
    {
        var stockReportBegins = new List<StockReportBegin>();
        var stockReportAcknowledges = new List<StockReportAcknowledge>();
        var stockReportEnds = new List<StockReportEnd>();

        foreach (var record in records)
        {
            if (record.RecordType == typeof(StockReportBegin))
                stockReportBegins.Add((StockReportBegin)record.RecordValue);
            else if (record.RecordType == typeof(StockReportAcknowledge))
                stockReportAcknowledges.Add((StockReportAcknowledge)record.RecordValue);
            else if (record.RecordType == typeof(StockReportEnd))
                stockReportEnds.Add((StockReportEnd)record.RecordValue);
        }

        foreach (var stockReportBegin in stockReportBegins)
        {
            var relatedStockReportAcknowledges = stockReportAcknowledges
                .Where(l => l.RequestId == stockReportBegin.RequestId)
                .ToList();

            foreach (var stockReportAcknowledge in relatedStockReportAcknowledges)
            {
                stockReportBegin.StockReportAcknowledges.Add(stockReportAcknowledge);
            }

            var relatedStockReportEnd = stockReportEnds
                .First(e => e.RequestId == stockReportBegin.RequestId);
            stockReportBegin.StockReportEnd = relatedStockReportEnd;
        }

        return stockReportBegins;
    }

    private WamasImportJobResult Import(List<StockReportBegin> stockReportsBegins, string file)
    {
        var jobResult = new WamasImportJobResult();

        foreach (var stockReportBegin in stockReportsBegins)
        {
            try
            {
                var batch = $"WAMAS-{stockReportBegin.RequestId}";
                var userId = GetUserIdByName("Support");
                var alreadyImportedReportsWithSameBatch = DatabaseConnector.GetQueryable<InventoryLog>()
                    .Where(g => g.Batch == batch).ToList();

                if (alreadyImportedReportsWithSameBatch.Any())
                    continue;
                
                var warehouseId = GetWarehouseIdByName(stockReportBegin.WarehouseLocation);

                DatabaseConnector.BeginTransaction();

                if (stockReportBegin.StockReportAcknowledges.Any())
                {
                    foreach (var stockReportAcknowledge in stockReportBegin.StockReportAcknowledges)
                    {
                        var articleId = GetArticleIdByNumber(stockReportAcknowledge.ItemNumber);
                        var storageLocationId = GetOrCreateStorageLocationIdFromDescription(stockReportAcknowledge.StorageLocation, articleId, warehouseId);
                        var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };
                        var quantity = decimal.ToInt32(decimal.Parse(stockReportAcknowledge.BaseQuantity.Replace("+", ""), numberFormatInfo));
                        var hasPackagingUnit = stockReportAcknowledge.NumberOfBaseAndWarehouseQuantityUnit > 1;
                        var packageLevel = hasPackagingUnit ? PackageLevel.BulkPackage : PackageLevel.Filling;
                        var packageUnitQuantity = quantity / stockReportAcknowledge.NumberOfBaseAndWarehouseQuantityUnit;

                        var inventoryLog = new InventoryLog
                        {
                            Batch = batch,
                            UserId = userId,
                            ArticleId = articleId,
                            StorageLocationId = storageLocationId,
                            PackageLevel = packageLevel,
                            LevelFactor = stockReportAcknowledge.NumberOfBaseAndWarehouseQuantityUnit,
                            BaseUnitCount = quantity,
                            Lot = string.Empty,
                            Timestamp = stockReportAcknowledge.RecordDate,
                            ProcessingStatus = ProcessingStatus.Registered,
                            AreaId = 1,
                            Guid = Guid.NewGuid()
                        };

                        if (hasPackagingUnit)
                            inventoryLog.PackageCount = packageUnitQuantity;

                        DatabaseConnector.Insert(inventoryLog);
                    }
                }

                DatabaseConnector.Commit();

                jobResult.ImportedCount++;
            }
            catch (Exception e)
            {
                jobResult.ErrorCount++;
                DatabaseConnector.Rollback();
                
                MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasStockReportImportError,
                    stockReportBegin.RequestId, stockReportBegin.RecordDate.ToString("dd.MM.yyy hh:mm:ss"), file, e.Message);
                ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasStockReportImportErrorTitle,
                    string.Format(
                        ResourceIds.WamasStockReportImportError, stockReportBegin.RequestId,
                        stockReportBegin.RecordDate.ToString("dd.MM.yyy hh:mm:ss"), file, e.Message));
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
        try
        {
            var articles = DatabaseConnector.GetQueryable<Article>()
                .Where(a => a.ArticleNumber == decimal.Parse(articleNumber)).ToList();
            return articles.First().Id;
        }
        catch (Exception e)
        {
            var exception = new Exception($"Article: {articleNumber} not found", e);
            throw exception;
        }
    }

    private int GetUserIdByName(string userName)
    {
        try
        {
            return DatabaseConnector.GetQueryable<Employee>().First(e => e.Uref == userName).Id;
        }
        catch (Exception e)
        {
            var exception = new Exception($"Employee: {userName} not found", e);
            throw exception;
        }
    }
    
    private int GetWarehouseIdByName(string warehouseName)
    {
        var warehouse = DatabaseConnector.GetQueryable<Warehouse>().FirstOrDefault(w => w.Name == warehouseName);
        var warehouseId = warehouse?.Id ?? 9; // Fallback: ID von Lager Rupperswil = 9
        return warehouseId;
    }

    private int GetOrCreateStorageLocationIdFromDescription(string storageLocationDescription, int articleId, int warehouseId)
    {
        var storageLocation = DatabaseConnector.GetQueryable<StorageLocation>()
                                  .FirstOrDefault(w => w.Description == storageLocationDescription) ??
                              CreateStorageLocation(storageLocationDescription, articleId, warehouseId);
        return storageLocation.Id;
    }

    private StorageLocation CreateStorageLocation(string storageLocationDescription, int articleId, int warehouseId)
    {
        try
        {
            var storageLocation = new StorageLocation
            {
                WarehouseId = warehouseId,
                ArticleId = articleId,
                Description = storageLocationDescription
            };

            DatabaseConnector.Insert(storageLocation);

            return storageLocation;
        }
        catch (Exception e)
        {
            var exception = new Exception($"StorageLocation: {storageLocationDescription} not found and failed to create", e);
            throw exception;
        }
    }
}