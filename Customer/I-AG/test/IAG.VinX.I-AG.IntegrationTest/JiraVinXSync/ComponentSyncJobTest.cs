using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.IAG.JiraVinXSync.ComponentSync.BusinessLogic;
using IAG.VinX.IAG.JiraVinXSync.ComponentSync.DataAccess.VinX;
using IAG.VinX.IAG.JiraVinXSync.ComponentSync.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.IAG.IntegrationTest.JiraVinXSync;

public class ComponentSyncJobTest
{
    [Fact]
    public void ExecuteMainSyncJobTest()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();
        var componentSyncVinXConnector = new ComponentSyncVinXConnector(factory);
        componentSyncVinXConnector.CreateConnection(factory.ConnectionString);
            
        var componentSyncer = new ComponentSyncer(componentSyncVinXConnector);
        var job = new ComponentSyncJob(componentSyncer)
        {
            Config = new ComponentSyncConfig()
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