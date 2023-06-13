using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.Test.Execution;

public class HelloJobTest
{
    private readonly HelloJob _job;
    private readonly IJobInfrastructure _infrastructure;
    private double _progress;
    private bool _heartbeat;

    public HelloJobTest()
    {
        _job = new HelloJob { Config = { Delay = 1 } };
        var infrastructureMock = new Mock<IJobInfrastructure>();

        infrastructureMock.Setup(i => i.HeartbeatAndReportProgress(It.IsAny<double>())).Callback<double>((p) => _progress = p);
        infrastructureMock.Setup(i => i.Heartbeat()).Callback(() => _heartbeat = true);

        _infrastructure = infrastructureMock.Object;
    }

    [Fact]
    public void ExecutionTest()
    {
        _job.Execute(_infrastructure);

        Assert.Equal(_job.Config.NbOfOutputs, _job.Result.NbExecutions);
        Assert.Equal(JobResultEnum.Success, _job.Result.Result);
        Assert.Equal(1.0, _progress);
        Assert.True(_heartbeat);
    }

    [Fact]
    public void FailedExecutionTest()
    {
        _job.Parameter.ThrowException = true;

        var result = _job.Execute(_infrastructure);

        Assert.False(result);
    }
}