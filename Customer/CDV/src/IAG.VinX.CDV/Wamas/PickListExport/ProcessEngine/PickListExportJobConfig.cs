using IAG.VinX.CDV.Wamas.Common.ProcessEngine;

namespace IAG.VinX.CDV.Wamas.PickListExport.ProcessEngine;

public class PickListExportJobConfig : WamasBaseJobConfig<PickListExportJob>
{
    public string ExportDayOffset { get; set; } = "$$exportDayOffset$";
    
    public string LeadTimeMinutesOffset { get; set; } = "$$leadTimeMinutesOffset$";
}