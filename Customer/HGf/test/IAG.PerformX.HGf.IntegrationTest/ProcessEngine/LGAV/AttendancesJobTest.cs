using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AttendancesJob;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVWorkflowJob;

using Moq;

using Xunit;

namespace IAG.PerformX.HGf.IntegrationTest.ProcessEngine.LGAV;

public class AttendancesJobTest
{
    [Fact]
    public void ExecuteJobTest()
    {
        var job = new AttendancesJob(new MockILogger<AttendancesJob>())
        {
            Config = new CommonConfigMapper<AttendancesConfig>().NewDestination(ConfigHelper<AttendancesJob>.CommonConfig)
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();

        job.Execute(jobInfrastructureMock.Object);
    }
}