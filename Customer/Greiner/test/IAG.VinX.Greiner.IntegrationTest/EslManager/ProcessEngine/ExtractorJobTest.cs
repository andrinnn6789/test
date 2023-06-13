using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.Greiner.EslManager.Config;
using IAG.VinX.Greiner.EslManager.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.Greiner.IntegrationTest.EslManager.ProcessEngine;

public class ExtractorJobTest
{
    [Fact]
    public void ExecuteEslExportJobTest()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();

        var config = new ExtractorJobConfig
        {
            VinXConnectionString = factory.ConnectionString,
            EslExportConfig = new EslExportConfig
            {
                ExportRoot = "Export"
            }
        };
        var job = new ExtractorJob(factory, new MockILogger<ExtractorJob>())
        {
            Config = config
        };
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);
        Assert.True(result);
        Assert.True(job.Result.ErrorCount == 0);
    }
}