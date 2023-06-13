using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.ProcessEngine.Configuration;

namespace IAG.InstallClient.ProcessEngineJob.SelfUpdate;

[ExcludeFromCodeCoverage]
public class SelfUpdateJobConfig : JobConfig<SelfUpdateJob>
{
    public SelfUpdateJobConfig()
    {
        Active = true;
        CronExpression = "* 22 * * *"; // nightly update
        HeartbeatTimeout = new(0, 0, 5, 0);
    }
}