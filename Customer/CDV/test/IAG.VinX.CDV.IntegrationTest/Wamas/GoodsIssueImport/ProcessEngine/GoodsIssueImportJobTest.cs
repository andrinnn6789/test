using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.IntegrationTest.Wamas.Common;
using IAG.VinX.CDV.Wamas.GoodsIssueImport.BusinessLogic;
using IAG.VinX.CDV.Wamas.GoodsIssueImport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.IntegrationTest.Wamas.GoodsIssueImport.ProcessEngine;

public class GoodsIssueImportJobTest
{
    [Fact]
    public void ExecuteGoodsIssueImportJob()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();
        var ftpConfig = FtpHelper.CreateFtpConfig();
        var ftpConnector = FtpHelper.CreateFtpConnector();
        var goodsIssueImporter = new GoodsIssueImporter(factory, ftpConnector);

        var job = new GoodsIssueImportJob(goodsIssueImporter)
        {
            Config = new GoodsIssueImportJobConfig
            {
                ConnectionString = factory.ConnectionString,
                WamasFtpConfig = ftpConfig
            }
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
    }
}