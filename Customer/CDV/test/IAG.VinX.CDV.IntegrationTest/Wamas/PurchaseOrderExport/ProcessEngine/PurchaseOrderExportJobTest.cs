using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.IntegrationTest.Wamas.Common;
using IAG.VinX.CDV.Wamas.PurchaseOrderExport.BusinessLogic;
using IAG.VinX.CDV.Wamas.PurchaseOrderExport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.IntegrationTest.Wamas.PurchaseOrderExport.ProcessEngine;

public class PurchaseOrderExportJobTest
{
    [Fact]
    public void ExecutePurchaseOrderExportJob()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();
        var ftpConfig = FtpHelper.CreateFtpConfig();
        var ftpConnector = FtpHelper.CreateFtpConnector();
        var purchaseOrderExporter = new PurchaseOrderExporter(factory, ftpConnector);

        var job = new PurchaseOrderExportJob(purchaseOrderExporter)
        {
            Config = new PurchaseOrderExportJobConfig
            {
                ConnectionString = factory.ConnectionString,
                WamasFtpConfig = ftpConfig,
                ExportBeforeDeliveryDayOffset = "1"
            }
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
    }
}