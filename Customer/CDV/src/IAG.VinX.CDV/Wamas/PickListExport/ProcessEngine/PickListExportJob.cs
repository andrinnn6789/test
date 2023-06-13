using System;

using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.PickListExport.BusinessLogic;

namespace IAG.VinX.CDV.Wamas.PickListExport.ProcessEngine;

[JobInfo("3D6235D1-EE3B-46A3-B29C-F296AAC607E2", JobName)]
public class PickListExportJob : JobBase<PickListExportJobConfig, JobParameter, WamasExportJobResult>
{
    private const string JobName = Resource.ResourceIds.WamasPickListExportJobName;

    private readonly IPickListExporter _pickListExporter;

    public PickListExportJob(IPickListExporter pickListExporter)
    {
        _pickListExporter = pickListExporter;
    }

    protected override void ExecuteJob()
    {
        // Offset days is used to configure the number of days in future to check for picking lists.
        var exportDaysOffset = !string.IsNullOrEmpty(Config.ExportDayOffset)
            ? Convert.ToInt32(Config.ExportDayOffset)
            : 10;
        var exportUntil = DateTime.Now.AddDays(exportDaysOffset);

        // Lead time offset is used to configure the number of minutes a picking list won't be exported after creation 
        var leadTimeMinutesOffset = !string.IsNullOrEmpty(Config.LeadTimeMinutesOffset)
            ? Convert.ToInt32(Config.LeadTimeMinutesOffset)
            : 30;
        var minimumLeadTime = DateTime.Now.AddMinutes(-leadTimeMinutesOffset);
        
        _pickListExporter.SetConfig(Config.WamasFtpConfig, Config.ConnectionString, this);
        var result = _pickListExporter.ExportPickLists(exportUntil, minimumLeadTime);

        Result.Result = result.Result;
        Result.ExportedCount = result.ExportedCount;
        Result.ErrorCount = result.ErrorCount;

        base.ExecuteJob();
    }
}