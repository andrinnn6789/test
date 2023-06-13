using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.IntegrationTest.Wamas.Common;
using IAG.VinX.CDV.Wamas.StockAdjustmentImport.BusinessLogic;
using IAG.VinX.CDV.Wamas.StockAdjustmentImport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.IntegrationTest.Wamas.StockAdjustmentImport.ProcessEngine;

public class StockAdjustmentImportJobTest
{
    [Fact]
    public void ExecuteStockAdjustmentImportJob()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();
        var ftpConfig = FtpHelper.CreateFtpConfig();
        var ftpConnector = FtpHelper.CreateFtpConnector();
        var stockAdjustmentImporter = new StockAdjustmentImporter(factory, ftpConnector);

        var job = new StockAdjustmentImportJob(stockAdjustmentImporter)
        {
            Config = new StockAdjustmentImportJobConfig
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