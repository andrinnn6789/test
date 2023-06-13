using System;

using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;

using IAG.VinX.IAG.JiraVinXSync.WorklogSync.BusinessLogic;
using IAG.VinX.IAG.Resource;

namespace IAG.VinX.IAG.JiraVinXSync.WorklogSync.ProcessEngine;

[JobInfo("1F4575B2-FBD7-4E48-A254-0F3C33A5B76F", JobName)]
public class WorklogSyncJob : JobBase<WorklogSyncConfig, JobParameter, JobResult>
{
    private const string JobName = ResourceIds.VinXJiraWorklogSyncJobName;
    private readonly IWorklogSyncer _worklogSyncer;

    public WorklogSyncJob(IWorklogSyncer worklogSyncer)
    {
        _worklogSyncer = worklogSyncer;
    }

    protected override void ExecuteJob()
    {
        var state = Infrastructure.GetJobData<WorklogSyncState>();
        var timestampStart = DateTime.Now;
            
        _worklogSyncer.SetConfig(Config.VinXConnectionString, Config.JiraRestConfig, this);
            
        var result = _worklogSyncer.SyncWorklogs(state.LastSync);
        Result.Result = result.Result;
        Result.ErrorCount = result.ErrorCount;
            
        state.LastSync = timestampStart;

        if(Result.Result != JobResultEnum.Failed)
            Infrastructure.SetJobData(state);

        base.ExecuteJob();
    }
}