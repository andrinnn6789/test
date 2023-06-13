using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.TestHelper.MockHost;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.IAG.ControlCenter.Config;
using IAG.VinX.IAG.ControlCenter.Mobile.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.IAG.IntegrationTest.ControlCenter.Mobile.ProcessEngine;

public class LicenceSyncerJobTest
{
    [Fact]
    public void ExecuteMobileSyncJobOkTest()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(GetType().Namespace + ".RequestMock.json", testPort);
        var job = BuildJob(testPort);
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var syncState = new SyncState
        {
            SyncCounter = 3
        };
        jobInfrastructureMock.Setup(m => m.GetJobData<SyncState>()).Returns(syncState);
        var result = job.Execute(jobInfrastructureMock.Object);
        Assert.True(result);
        Assert.Equal(nameof(LicenceSyncerJob), job.Result.SyncName);
        Assert.Equal(JobResultEnum.Success, job.Result.Result);
        Assert.Equal(0, job.Result.ErrorCount);
        Assert.Equal(1, job.Result.SuccessCount);
    }

    [Fact]
    public void ExecuteMobileSyncJobFailTest()
    {
        var job = BuildJob(60042); // port does not matter for this test
        job.Config.BackendConfig.UrlAuth = "/badUrl";
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var syncState = new SyncState();
        jobInfrastructureMock.Setup(m => m.GetJobData<SyncState>()).Returns(syncState);
        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
        Assert.Equal(nameof(LicenceSyncerJob), job.Result.SyncName);
        Assert.Equal(JobResultEnum.Failed, job.Result.Result);
        Assert.Equal(1, job.Result.ErrorCount);
        Assert.Equal(0, job.Result.SuccessCount);
    }

    private static LicenceSyncerJob BuildJob(int port)
    {
        var userContext = new ExplicitUserContext("test", null);
        return new LicenceSyncerJob(new SybaseConnectionFactory(userContext, new MockILogger<SybaseConnection>(), null, null), new MockILogger<LicenceSyncerJob>(), ConfigHelper.GetMockedTokenRequest())
        {
            Config =
            {
                VinXConfig = new VinXConfig
                {
                    ConnectionString = SybaseConnectionFactoryHelper.CreateFactory().ConnectionString
                },
                BackendConfig = new BackendConfig
                {
                    UrlAuth = $"http://localhost:{port}/",
                    UrlControlCenter = $"http://localhost:{port}/"
                },
                DiffSyncsPerFullSync = 3
            }
        };
    }
}