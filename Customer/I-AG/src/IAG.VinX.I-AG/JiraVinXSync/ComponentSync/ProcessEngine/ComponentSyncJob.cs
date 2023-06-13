using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.IAG.JiraVinXSync.ComponentSync.BusinessLogic;
using IAG.VinX.IAG.Resource;

namespace IAG.VinX.IAG.JiraVinXSync.ComponentSync.ProcessEngine;

[JobInfo("A1AD6DE7-4348-4256-A0CB-08569229532F", JobName)]
public class ComponentSyncJob : JobBase<ComponentSyncConfig, JobParameter, JobResult>
{
    private const string JobName = ResourceIds.VinXJiraComponentSyncJobName;
    private readonly IComponentSyncer _componentSyncer;

    public ComponentSyncJob(IComponentSyncer componentSyncer)
    {
        _componentSyncer = componentSyncer;
    }

    protected override void ExecuteJob()
    {
        _componentSyncer.SetConfig(Config.VinXConnectionString, Config.JiraRestConfig, this);
            
        var result = _componentSyncer.SyncComponents();
        Result.Result = result.Result;
        Result.ErrorCount = result.ErrorCount;

        base.ExecuteJob();
    }
}