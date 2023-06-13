using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.Wamas.ArticleExport.BusinessLogic;
using IAG.VinX.CDV.Wamas.ArticleExport.ProcessEngine;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Wamas.ArticleExport.ProcessEngine;

public class ArticleExportJobTest
{
    [Fact]
    public void ExecuteArticleExportJob()
    {
        var fakeConfig = new Mock<ArticleExportJobConfig>
        {
            Object =
            {
                WamasFtpConfig = new WamasFtpConfig(),
                ConnectionString = "test"
            }
        };

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeJobResult = new WamasExportJobResult
        {
            Result = JobResultEnum.Success,
            ExportedCount = 1,
            ErrorCount = 0
        };

        var fakeArticleExporter = new Mock<IArticleExporter>();
        fakeArticleExporter.Setup(exp => exp.SetConfig(fakeConfig.Object.WamasFtpConfig,
            fakeConfig.Object.ConnectionString, fakeMessageLogger.Object));
        fakeArticleExporter.Setup(exp => exp.ExportArticles(It.IsAny<DateTime>())).Returns(fakeJobResult);

        var job = new ArticleExportJob(fakeArticleExporter.Object)
        {
            Config = fakeConfig.Object
        };

        var jobState = new ArticleExportJobState();
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        jobInfrastructureMock.Setup(m => m.GetJobData<ArticleExportJobState>()).Returns(jobState);
        job.Execute(jobInfrastructureMock.Object);
        Assert.True(job.Result.Result == fakeJobResult.Result);
        Assert.True(job.Result.ExportedCount == fakeJobResult.ExportedCount);
        Assert.True(job.Result.ErrorCount == fakeJobResult.ErrorCount);
    }
}