using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.Schüwo.SV.ProcessEngine.UploadOrderData;

using Moq;

using Xunit;

namespace IAG.VinX.Schüwo.IntegrationTest.SV.ProcessEngine;

public class UploadOrderDataJobTest
{
    [Fact]
    public void ExecuteOrderDataUploadJobTest()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();

        var config = new UploadOrderDataJobConfig
        {
            VinXConnectionString = factory.ConnectionString,
            FtpEndpointConfig = ConfigHelper.FtpEndpointTest,
            FtpPathConfig = ConfigHelper.FtpPathConfigTest
        };

        var job = new UploadOrderDataJob(factory)
        {
            Config = config
        };
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);
        Assert.True(result);
        Assert.True(job.Result.ErrorCount == 0);
    }
}