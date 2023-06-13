using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.InstallClient.BusinessLogic.Model;

namespace IAG.InstallClient.ProcessEngineJob.Installation;

public class InstallJobParameter: JobParameter
{
    public InstallationSetup Setup { get; set; }
    public string ServiceToStart { get; set; }
}