using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.Schüwo.SV.ProcessEngine.UploadBaseData;

using Moq;

using Xunit;

namespace IAG.VinX.Schüwo.IntegrationTest.SV.ProcessEngine;

public class UploadBaseDataJobTest
{
    [Fact]
    public void ExecuteBaseDataUploadJobTest()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();

        var config = new UploadBaseDataJobConfig
        {
            VinXConnectionString = factory.ConnectionString,
            FtpEndpointConfig = ConfigHelper.FtpEndpointTest,
            FtpPathConfig = ConfigHelper.FtpPathConfigTest
        };

        var job = new UploadBaseDataJob(factory)
        {
            Config = config
        };
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);
        Assert.True(result);
        Assert.True(job.Result.ErrorCount == 0);
        Assert.True(job.Result.ResultCounts.WarningCount == 0);
    }
}