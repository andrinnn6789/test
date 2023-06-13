using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.IAG.JiraVinXSync.VersionSync.BusinessLogic;
using IAG.VinX.IAG.Resource;

namespace IAG.VinX.IAG.JiraVinXSync.VersionSync.ProcessEngine;

[JobInfo("6BAB9433-C207-442D-96E5-7827FEB89AAF", JobName)]
public class VersionSyncJob : JobBase<VersionSyncConfig, JobParameter, JobResult>
{
    private const string JobName = ResourceIds.VinXJiraVersionSyncJobName;
    private readonly IVersionSyncer _versionSyncer;

    public VersionSyncJob(IVersionSyncer versionSyncer)
    {
        _versionSyncer = versionSyncer;
    }

    protected override void ExecuteJob()
    {
        _versionSyncer.SetConfig(Config.VinXConnectionString, Config.JiraRestConfig, this);
            
        var result = _versionSyncer.SyncVersions();
        Result.Result = result.Result;
        Result.ErrorCount = result.ErrorCount;

        base.ExecuteJob();
    }
}