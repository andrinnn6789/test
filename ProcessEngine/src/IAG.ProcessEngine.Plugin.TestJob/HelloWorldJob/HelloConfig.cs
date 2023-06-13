using IAG.Infrastructure.ProcessEngine.Configuration;

using JetBrains.Annotations;

namespace IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;

[UsedImplicitly]
public class HelloConfig : JobConfig<HelloJob>
{
    public HelloConfig()
    {
        NbOfOutputs = 5;
        Delay = 10;
        LogActivity = true;
        AllowConcurrentInstances = true;
    }

    public int NbOfOutputs { get; set; }

    public int Delay { get; set; }
}