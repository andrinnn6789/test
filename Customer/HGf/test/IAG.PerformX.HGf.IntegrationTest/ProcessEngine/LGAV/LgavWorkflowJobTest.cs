using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVWorkflowJob;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace IAG.PerformX.HGf.IntegrationTest.ProcessEngine.LGAV;

public class LgavWorkflowJobTest
{
    [Fact]
    public void ExecuteLgavWorkflowJobTest()
    {
        var loggerFactory = new Mock<ILoggerFactory>();
        loggerFactory.Setup(m => m.CreateLogger(It.IsAny<string>())).Returns(new MockILogger<object>());

        var job = new LgavWorkflowJob(loggerFactory.Object)
        {
            Config = new CommonConfigMapper<LgavWorkflowConfig>().NewDestination(ConfigHelper<LgavWorkflowJob>.CommonConfig)
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();

        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
    }
}