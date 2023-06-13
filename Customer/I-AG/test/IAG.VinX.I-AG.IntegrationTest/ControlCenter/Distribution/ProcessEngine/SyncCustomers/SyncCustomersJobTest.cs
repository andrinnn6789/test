using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Config;
using IAG.VinX.IAG.ControlCenter.Distribution.ProcessEngine.SyncCustomers;

using Moq;

using Xunit;

namespace IAG.VinX.IAG.IntegrationTest.ControlCenter.Distribution.ProcessEngine.SyncCustomers;

public class SyncCustomersJobTest
{
    [Fact(Skip = "Only executed manually")]
    public void SyncCustomersJobWithLocalCc()
    {
        var job = BuildJob(new BackendConfig {UrlControlCenter = "http://localhost:8086/"}, ConfigHelper.GetMockedTokenRequest());
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);
        Assert.True(result);
    }

    [Fact(Skip = "Only executed manually")]
    public void SyncCustomersJobWithTestingCc()
    {
        var job = BuildJob(ConfigHelper.CcConfigTestingServer, new ControlCenterTokenRequest());
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);
        Assert.True(result);
    }

    private SyncCustomersJob BuildJob(BackendConfig backendConfig, IControlCenterTokenRequest tokenRequestHandler)
    {
        var userContext = new ExplicitUserContext("test", null);

        return new SyncCustomersJob(new SybaseConnectionFactory(userContext, new MockILogger<SybaseConnection>(), null, null), tokenRequestHandler, new MockILogger<SyncCustomersJob>())
        {
            Config =
            {
                Backend = backendConfig,
                VinX = new VinXConfig
                {
                    ConnectionString = SybaseConnectionFactoryHelper.CreateFactory().ConnectionString
                }
            },
            Parameter = new JobParameter()
        };
    }
}