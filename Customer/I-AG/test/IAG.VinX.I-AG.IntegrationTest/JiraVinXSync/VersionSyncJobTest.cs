using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.IAG.JiraVinXSync.VersionSync.BusinessLogic;
using IAG.VinX.IAG.JiraVinXSync.VersionSync.DataAccess.VinX;
using IAG.VinX.IAG.JiraVinXSync.VersionSync.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.IAG.IntegrationTest.JiraVinXSync;

public class VersionSyncJobTest
{
    [Fact]
    public void ExecuteMainSyncJobTest()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();
        var versionSyncVinXConnector = new VersionSyncVinXConnector(factory);
        versionSyncVinXConnector.CreateConnection(factory.ConnectionString);
            
        var versionSyncer = new VersionSyncer(versionSyncVinXConnector);
        var job = new VersionSyncJob(versionSyncer)
        {
            Config = new VersionSyncConfig()
            {
                VinXConnectionString = factory.ConnectionString,
                JiraRestConfig = ConfigHelper.JiraRestConfigTest
            }
        };
            
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);
        Assert.True(result);
    }
}