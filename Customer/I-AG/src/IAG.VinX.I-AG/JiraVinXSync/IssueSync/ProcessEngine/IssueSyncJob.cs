using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.IAG.JiraVinXSync.IssueSync.BusinessLogic;
using IAG.VinX.IAG.Resource;

namespace IAG.VinX.IAG.JiraVinXSync.IssueSync.ProcessEngine;

[JobInfo("B68D5FFF-7361-44DB-80DC-85555A6BD33C", JobName)]
public class IssueSyncJob : JobBase<IssueSyncConfig, JobParameter, IssueSyncResult>
{
    private const string JobName = ResourceIds.VinXJiraIssueSyncJobName;
    private readonly IIssueSyncer _issueSyncer;

    public IssueSyncJob(IIssueSyncer issueSyncer)
    {
        _issueSyncer = issueSyncer;
    }

    protected override void ExecuteJob()
    {
        _issueSyncer.SetConfig(Config.VinXConnectionString, Config.JiraRestConfig, this);

        var result = _issueSyncer.SyncIssues();
        Result.Result = result.Result;
        Result.ErrorCount = result.ErrorCount;

        base.ExecuteJob();
    }
}