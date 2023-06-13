using System;

using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.IntegrationTest.Wamas.Common;
using IAG.VinX.CDV.Wamas.ArticleExport.BusinessLogic;
using IAG.VinX.CDV.Wamas.ArticleExport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.IntegrationTest.Wamas.ArticleExport.ProcessEngine;

public class ArticleExportJobTest
{
    [Fact]
    public void ExecuteArticleExportJob()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();
        var ftpConfig = FtpHelper.CreateFtpConfig();
        var ftpConnector = FtpHelper.CreateFtpConnector();
        var articleExporter = new ArticleExporter(factory, ftpConnector);

        var job = new ArticleExportJob(articleExporter)
        {
            Config = new ArticleExportJobConfig
            {
                ConnectionString = factory.ConnectionString,
                WamasFtpConfig = ftpConfig
            }
        };

        var jobState = new ArticleExportJobState()
        {
            Id = Guid.NewGuid(),
            LastSync = DateTime.Now.AddMinutes(-10)
        };
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        jobInfrastructureMock.Setup(m => m.GetJobData<ArticleExportJobState>()).Returns(jobState);
        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
    }
}