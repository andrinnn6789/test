using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.ProcessEngine.InternalJob.Monitoring;

public class MonitoringResult : JobResult
{
    public int SendCount { get; set; }
}