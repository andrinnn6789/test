using IAG.VinX.CDV.Wamas.Common.ProcessEngine;

namespace IAG.VinX.CDV.Wamas.PurchaseOrderExport.ProcessEngine;

public class PurchaseOrderExportJobConfig : WamasBaseJobConfig<PurchaseOrderExportJob>
{
    public string ExportBeforeDeliveryDayOffset { get; set; } = "$$exportBeforeDeliveryDayOffset$";
}