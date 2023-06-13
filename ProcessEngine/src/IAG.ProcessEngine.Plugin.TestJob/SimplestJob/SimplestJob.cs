using System;

using IAG.Infrastructure.ProcessEngine.JobModel;

using JetBrains.Annotations;

namespace IAG.ProcessEngine.Plugin.TestJob.SimplestJob;

[UsedImplicitly]
[JobInfo("99CEF6EB-8960-4293-85EB-5719C9675F8F", JobName)]
public class SimplestJob : JobBase<SimplestConfig, JobParameter, JobResult>
{
    public const string JobName = "Test.SimplestJob";

    protected override void ExecuteJob()
    {
        Console.WriteLine("Hello simplest job");
        base.ExecuteJob();
    }
}