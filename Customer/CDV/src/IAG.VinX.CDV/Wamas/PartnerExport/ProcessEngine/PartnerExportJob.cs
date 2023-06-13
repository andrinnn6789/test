using System;

using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.PartnerExport.BusinessLogic;

namespace IAG.VinX.CDV.Wamas.PartnerExport.ProcessEngine;

[JobInfo("CE9A20AF-D855-41C5-9262-53271E9FF7EE", JobName)]
public class PartnerExportJob : JobBase<PartnerExportJobConfig, JobParameter, WamasExportJobResult>
{
    private const string JobName = Resource.ResourceIds.WamasPartnerExportJobName;

    private readonly IPartnerExporter _partnerExporter;

    public PartnerExportJob(IPartnerExporter partnerExporter)
    {
        _partnerExporter = partnerExporter;
    }

    protected override void ExecuteJob()
    {
        var state = Infrastructure.GetJobData<PartnerExportJobState>();

        var timestampStart = DateTime.Now;
        
        _partnerExporter.SetConfig(Config.WamasFtpConfig, Config.ConnectionString, this);

        var result = _partnerExporter.ExportPartner(state.LastSync);

        Result.Result = result.Result;
        Result.ExportedCount = result.ExportedCount;
        Result.ErrorCount = result.ErrorCount;
        
        state.LastSync = timestampStart;

        if(Result.Result != JobResultEnum.Failed)
            Infrastructure.SetJobData(state);

        base.ExecuteJob();
    }
}