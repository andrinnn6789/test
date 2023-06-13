using System;

using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.PurchaseOrderExport.BusinessLogic;

namespace IAG.VinX.CDV.Wamas.PurchaseOrderExport.ProcessEngine;

[JobInfo("C6E3097E-85D7-482B-834F-62F0272C3520", JobName)]
public class PurchaseOrderExportJob : JobBase<PurchaseOrderExportJobConfig, JobParameter, WamasExportJobResult>
{
    private const string JobName = Resource.ResourceIds.WamasPurchaseOrderExportJobName;

    private readonly IPurchaseOrderExporter _purchaseOrderExporter;

    public PurchaseOrderExportJob(IPurchaseOrderExporter purchaseOrderExporter)
    {
        _purchaseOrderExporter = purchaseOrderExporter;
    }

    protected override void ExecuteJob()
    {
        // "ExportBeforeDeliveryDayOffset" is used to configure the number of days a purchase order can be exported before it's delivery-date.
        var exportBeforeDeliveryDayOffset = !string.IsNullOrEmpty(Config.ExportBeforeDeliveryDayOffset)
            ? Convert.ToInt32(Config.ExportBeforeDeliveryDayOffset)
            : 1;
        var exportUntil = DateTime.Now.AddDays(exportBeforeDeliveryDayOffset);
        
        _purchaseOrderExporter.SetConfig(Config.WamasFtpConfig, Config.ConnectionString, this);

        var result = _purchaseOrderExporter.ExportPurchaseOrders(exportUntil);

        Result.Result = result.Result;
        Result.ExportedCount = result.ExportedCount;
        Result.ErrorCount = result.ErrorCount;

        base.ExecuteJob();
    }
}