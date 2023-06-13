using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.Smith.SalesPdf;
using IAG.VinX.Smith.SalesPdf.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.Smith.IntegrationTest.SalesPdf.ProcessEngine;

public class ExtractorJobTest
{
    [Fact]
    public void ExecuteExtractorJob()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();
        var config = new ExtractorJobConfig<ExtractorJob>
        {
            VinXConnectionString = factory.ConnectionString,
            ExtractorWodConfig = new ExtractorWodConfig()
        };
        var job = new ExtractorJob(factory, ConfigHelper.WodConnector,  new MockILogger<ExtractorJob>())
        {
            Config = config
        };
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var syncState = new SyncState();
        jobInfrastructureMock.Setup(m => m.GetJobData<SyncState>()).Returns(syncState);
        var result = job.Execute(jobInfrastructureMock.Object);
        Assert.True(result);
        Assert.Equal(2, job.Result.SyncResults.Count);
        foreach (var syncResult in job.Result.SyncResults)
        {
            Assert.NotNull(syncResult.SyncName);
            Assert.True(syncResult.ErrorCount == 0);
        }
    }
}