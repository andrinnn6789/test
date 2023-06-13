using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.IAG.JiraVinXSync.WorklogSync.BusinessLogic;
using IAG.VinX.IAG.JiraVinXSync.WorklogSync.DataAccess.VinX;
using IAG.VinX.IAG.JiraVinXSync.WorklogSync.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.IAG.IntegrationTest.JiraVinXSync;

public class WorklogSyncJobTest
{
    [Fact]
    public void ExecuteMainSyncJobTest()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();
        var worklogSyncVinXConnector = new WorklogSyncVinXConnector(factory);
        worklogSyncVinXConnector.CreateConnection(factory.ConnectionString);
            
        var worklogSyncer = new WorklogSyncer(worklogSyncVinXConnector);
        var job = new WorklogSyncJob(worklogSyncer)
        {
            Config = new WorklogSyncConfig()
            {
                VinXConnectionString = factory.ConnectionString,
                JiraRestConfig = ConfigHelper.JiraRestConfigTest
            }
        };
            
        var syncState = new WorklogSyncState();
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        jobInfrastructureMock.Setup(m => m.GetJobData<WorklogSyncState>()).Returns(syncState);
            
        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
    }
}