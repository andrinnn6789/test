using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.PerformX.HGf.ProcessEngine.LGAV.EventsJob;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVWorkflowJob;

using Moq;

using Xunit;

namespace IAG.PerformX.HGf.IntegrationTest.ProcessEngine.LGAV;

public class EventsJobTest
{
    [Fact]
    public void ExecuteJobTest()
    {
        var job = new EventsJob(new MockILogger<EventsJob>())
        {
            Config = new CommonConfigMapper<EventsConfig>().NewDestination(ConfigHelper<EventsJob>.CommonConfig)
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();

        job.Execute(jobInfrastructureMock.Object);
    }
}