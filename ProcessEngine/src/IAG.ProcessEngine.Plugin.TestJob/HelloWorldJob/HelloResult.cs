using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;

public class HelloResult : JobResult
{
    public int NbExecutions { get; set; }
}