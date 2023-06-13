using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVWorkflowJob;
using IAG.PerformX.HGf.ProcessEngine.LGAV.RegistrationsJob;

using Moq;

using Xunit;

namespace IAG.PerformX.HGf.IntegrationTest.ProcessEngine.LGAV;

public class RegistrationsJobTest
{
    [Fact]
    public void ExecuteJobTest()
    {
        var job = new RegistrationsJob(new MockILogger<RegistrationsJob>())
        {
            Config = new CommonConfigMapper<RegistrationsConfig>().NewDestination(ConfigHelper<RegistrationsJob>.CommonConfig)
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();

        job.Execute(jobInfrastructureMock.Object);
    }
}