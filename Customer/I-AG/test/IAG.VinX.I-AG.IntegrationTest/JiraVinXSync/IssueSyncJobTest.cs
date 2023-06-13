using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.IAG.JiraVinXSync.IssueSync.BusinessLogic;
using IAG.VinX.IAG.JiraVinXSync.IssueSync.DataAccess.VinX;
using IAG.VinX.IAG.JiraVinXSync.IssueSync.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.IAG.IntegrationTest.JiraVinXSync;

public class IssueSyncJobTest
{
    [Fact]
    public void ExecuteMainSyncJobTest()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();
        var issueSyncVinXConnector = new IssueSyncVinXConnector(factory);
        issueSyncVinXConnector.CreateConnection(factory.ConnectionString);

        var issueSyncer = new IssueSyncer(issueSyncVinXConnector);
        var job = new IssueSyncJob(issueSyncer)
        {
            Config = new IssueSyncConfig
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