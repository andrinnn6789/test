using IAG.Infrastructure.ProcessEngine.JobModel;

using JetBrains.Annotations;

namespace IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;

[UsedImplicitly]
public class HelloParameter : JobParameter
{
    public HelloParameter()
    {
        GreetingsFrom = "Hello-Job";
    }

    public string GreetingsFrom { get; set; }

    public bool IgnoreJobCancel { get; set; }

    public bool WithFollowUp { get; set; }

    public bool ThrowException { get; set; }

    public bool ThrowLocalizableException { get; set; }
}