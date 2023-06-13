using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.InstallClient.ProcessEngineJob.Installation;

public class InstallJobResult: JobResult
{
    public string InstanceName { get; set; }
}