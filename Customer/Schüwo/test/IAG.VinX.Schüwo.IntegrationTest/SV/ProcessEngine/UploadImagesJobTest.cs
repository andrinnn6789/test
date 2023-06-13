using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.Schüwo.SV.ProcessEngine.UploadImages;

using Moq;

using Xunit;

namespace IAG.VinX.Schüwo.IntegrationTest.SV.ProcessEngine;

public class UploadImagesJobTest
{
    [Fact]
    public void ExecuteImagesUploadJobTest()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();

        var config = new UploadImagesJobConfig
        {
            VinXConnectionString = factory.ConnectionString,
            ArticleImageSourcePath = ConfigHelper.ArticleRootImagePathTestIag,
            ArticleImageArchivePath = ConfigHelper.ArticleImageRootPathArchive,
            FtpEndpointConfig = ConfigHelper.FtpEndpointTest,
            FtpPathConfig = ConfigHelper.FtpPathConfigTest
        };

        var job = new UploadImagesJob(factory)
        {
            Config = config
        };
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var syncState = new SyncState();
        jobInfrastructureMock.Setup(m => m.GetJobData<SyncState>()).Returns(syncState);
        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
        Assert.True(job.Result.ErrorCount == 0);
    }
}