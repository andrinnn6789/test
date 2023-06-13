using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.Gastivo.Common.Config;
using IAG.VinX.CDV.Gastivo.Common.ProcessEngine;
using IAG.VinX.CDV.Gastivo.ArticleExport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.ArticleExport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Gastivo.ArticleExport.ProcessEngine;

public class ArticleExportJobTest
{
    [Fact]
    public void ExecuteArticleExportJobTest()
    {
        // Arrange
        var fakeConfig = new Mock<ArticleExportJobConfig>
        {
            Object =
            {
                GastivoFtpConfig = new GastivoFtpConfig(),
                ConnectionString = "test",
                ImageUrlTemplate = "https://www.casadelvino.ch/ShopImage/artikel/list/{0}/0/{0}.jpg?width=1140&height=1140"
            }
        };

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeJobResult = new GastivoExportJobResult()
        {
            Result = JobResultEnum.Success,
            ExportedCount = 1,
            ErrorCount = 0
        };

        var fakeArticleExporter = new Mock<IArticleExporter>();
        fakeArticleExporter.Setup(exp => exp.SetConfig(fakeConfig.Object.GastivoFtpConfig,
            fakeConfig.Object.ConnectionString, fakeConfig.Object.ImageUrlTemplate, fakeMessageLogger.Object));
        fakeArticleExporter.Setup(exp => exp.ExportArticles()).Returns(fakeJobResult);

        var job = new ArticleExportJob(fakeArticleExporter.Object)
        {
            Config = fakeConfig.Object
        };
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();

        // Act
        job.Execute(jobInfrastructureMock.Object);

        // Assert
        Assert.True(job.Result.Result == fakeJobResult.Result);
        Assert.True(job.Result.ExportedCount == fakeJobResult.ExportedCount);
        Assert.True(job.Result.ErrorCount == fakeJobResult.ErrorCount);
    }
}