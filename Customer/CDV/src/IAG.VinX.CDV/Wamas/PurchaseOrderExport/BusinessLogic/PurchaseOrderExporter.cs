using System;
using System.Collections.Generic;
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
using IAG.VinX.CDV.Wamas.PurchaseOrderExport.Dto;

namespace IAG.VinX.CDV.Wamas.PurchaseOrderExport.BusinessLogic;

public class PurchaseOrderExporter : BaseExporter, IPurchaseOrderExporter
{
    private List<PurchaseOrder> _purchaseOrdersToExport = new();

    public PurchaseOrderExporter(
        ISybaseConnectionFactory databaseConnectionFactory, IFtpConnector ftpConnector)
        : base(databaseConnectionFactory, ftpConnector)
    {
    }
    
    protected override Type[] RecordTypes => new[]
    {
        typeof(PurchaseOrder),
        typeof(PurchaseOrderLine), 
        typeof(PurchaseOrderPartner),
        typeof(PurchaseOrderReference)
    };

    public new void SetConfig(
        WamasFtpConfig wamasFtpConfig,
        string connectionString,
        IMessageLogger messageLogger)
    {
        base.SetConfig(wamasFtpConfig, connectionString, messageLogger);
    }

    public WamasExportJobResult ExportPurchaseOrders(DateTime exportUntil)
    {
        var jobResult = new WamasExportJobResult();

        try
        {
            var records = GetRecords(exportUntil);

            if (records.Any())
            {
                SerializeAndUpload(records, ResourceIds.WamasPurchaseOrderRecordType);
                var updateResult = UpdateLogisticStates(LogisticState.TransmittedToLogistics);
                
                jobResult.ExportedCount = records.Count;
                jobResult.ErrorCount = updateResult.ErrorCount;
            }

            jobResult.Result = jobResult.ErrorCount == 0 && jobResult.ExportedCount >= 0
                ? JobResultEnum.Success
                : JobResultEnum.PartialSuccess;
        }
        catch (Exception e)
        {
            jobResult.ErrorCount++;
            jobResult.Result = JobResultEnum.Failed;
            UpdateLogisticStates(LogisticState.ErrorTryAgain);
            
            MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasPurchaseOrderExportError, e.Message);
            ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasPurchaseOrderExportErrorTitle,
                string.Format(ResourceIds.WamasPurchaseOrderExportError, e.Message));
        }
        finally
        {
            Dispose();
        }

        return jobResult;
    }

    private List<GenericWamasRecord> GetRecords(DateTime exportUntil)
    {
        _purchaseOrdersToExport = DatabaseConnector.GetQueryable<PurchaseOrder>().Where(p => p.DeliveryTimeSlotFrom <= exportUntil).ToList();
        var orderReferences = DatabaseConnector.GetQueryable<PurchaseOrderReference>().Where(p => p.DeliveryTimeSlotFrom <= exportUntil).ToList();
        var orderLines = DatabaseConnector.GetQueryable<PurchaseOrderLine>().Where(p => p.DeliveryTimeSlotFrom <= exportUntil).ToList();
        var orderPartners = DatabaseConnector.GetQueryable<PurchaseOrderPartner>().Where(p => p.DeliveryTimeSlotFrom <= exportUntil).ToList();

        var recordList = new List<GenericWamasRecord>();

        foreach (var order in _purchaseOrdersToExport)
        {
            recordList.Add(new GenericWamasRecord(order.GetType(), order));

            var orderReferenceFromOrder = orderReferences.First(p => p.Id == order.Id);
            recordList.Add(new GenericWamasRecord(orderReferenceFromOrder.GetType(), orderReferenceFromOrder));

            var orderLinesFromOrder = orderLines.Where(p => p.Id == order.Id).ToList();
            foreach (var orderLine in orderLinesFromOrder)
            {
                recordList.Add(new GenericWamasRecord(orderLine.GetType(), orderLine));
            }

            var orderPartnersFromOrder = orderPartners.Where(p => p.Id == order.Id).ToList();
            foreach (var orderPartner in orderPartnersFromOrder)
            {
                recordList.Add(new GenericWamasRecord(orderPartner.GetType(), orderPartner));
            }
        }

        return recordList;
    }

    private WamasExportJobResult UpdateLogisticStates(LogisticState logisticState)
    {
        var jobResult = new WamasExportJobResult();

        foreach (var purchaseOrder in _purchaseOrdersToExport)
        {
            try
            {
                var purchaseOrderDbModel = DatabaseConnector.GetQueryable<Document>()
                    .First(g => g.Id == int.Parse(purchaseOrder.Id));
                purchaseOrderDbModel.LogisticState = logisticState;
                DatabaseConnector.Update(purchaseOrderDbModel);
            }
            catch (Exception e)
            {
                jobResult.ErrorCount++;

                MessageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasPurchaseOrderConfirmError,
                    logisticState, purchaseOrder.Id, e.Message);
                ProcessErrorLogger.Log(MessageLogger, DatabaseConnector, ResourceIds.WamasPurchaseOrderExportErrorTitle,
                    string.Format(ResourceIds.WamasPurchaseOrderConfirmError, logisticState, purchaseOrder.Id,
                        e.Message));
            }
        }

        return jobResult;
    }
}