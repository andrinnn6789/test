using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.IntegrationTest.Wamas.Common;
using IAG.VinX.CDV.Wamas.GoodsReceiptImport.BusinessLogic;
using IAG.VinX.CDV.Wamas.GoodsReceiptImport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.IntegrationTest.Wamas.GoodsReceiptImport.ProcessEngine;

public class GoodsReceiptImportJobTest
{
    [Fact]
    public void ExecuteGoodsReceiptImportJob()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();
        var ftpConfig = FtpHelper.CreateFtpConfig();
        var ftpConnector = FtpHelper.CreateFtpConnector();
        var goodsReceiptImporter = new GoodsReceiptImporter(factory, ftpConnector);

        var job = new GoodsReceiptImportJob(goodsReceiptImporter)
        {
            Config = new GoodsReceiptImportJobConfig
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