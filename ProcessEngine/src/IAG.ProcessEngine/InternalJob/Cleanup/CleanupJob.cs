using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Resource;
using IAG.ProcessEngine.Store;

using JetBrains.Annotations;

namespace IAG.ProcessEngine.InternalJob.Cleanup;

[UsedImplicitly]
[JobInfo("0058A87E-463C-47FB-B086-9D10A7C443AB", JobName, true)]
public class CleanupJob : JobBase<CleanupJobConfig, JobParameter, JobResult>
{
    internal const string JobName = ResourceIds.ResourcePrefixJob + "Cleanup";

    private readonly IJobStore _jobStore;

    public CleanupJob(IJobStore jobStore)
    {
        _jobStore = jobStore;
    }

    protected override void ExecuteJob()
    {
        _jobStore.DeleteOldJobs(Config.ArchiveDays, Config.ErrorDays);

        base.ExecuteJob();
    }
}