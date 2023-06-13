using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.IntegrationTest.Gastivo.Common;
using IAG.VinX.CDV.Gastivo.ArticleExport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.ArticleExport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.IntegrationTest.Gastivo.ArticleExport.ProcessEngine;

public class ArticleExportJobTest
{
    [Fact]
    public void ExecuteArticleExportJob()
    {
        var factory = NHibernateSessionContextFactoryHelper.CreateFactory();
        var ftpConfig = FtpHelper.CreateFtpConfig();
        var ftpConnector = FtpHelper.CreateFtpConnector();
        var articleExporter = new ArticleExporter(factory, ftpConnector);

        var job = new ArticleExportJob(articleExporter)
        {
            Config = new ArticleExportJobConfig
            {
                ConnectionString = factory.ConnectionString,
                GastivoFtpConfig = ftpConfig,
                ImageUrlTemplate = "https://www.casadelvino.ch/ShopImage/artikel/list/{0}/0/{0}.jpg?width=1140&height=1140"
            }
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
    }
}