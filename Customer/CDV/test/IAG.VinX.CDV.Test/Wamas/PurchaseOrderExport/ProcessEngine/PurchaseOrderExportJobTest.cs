using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.PurchaseOrderExport.BusinessLogic;
using IAG.VinX.CDV.Wamas.PurchaseOrderExport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Wamas.PurchaseOrderExport.ProcessEngine;

public class PurchaseOrderExportJobTest
{
    [Fact]
    public void ExecuteArticleExportJob()
    {
        var fakeConfig = new Mock<PurchaseOrderExportJobConfig>
        {
            Object =
            {
                WamasFtpConfig = new WamasFtpConfig(),
                ConnectionString = "test",
                ExportBeforeDeliveryDayOffset = "1"
            }
        };

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeJobResult = new WamasExportJobResult
        {
            Result = JobResultEnum.Success,
            ExportedCount = 1,
            ErrorCount = 0
        };

        var fakePurchaseOrderExporter = new Mock<IPurchaseOrderExporter>();
        fakePurchaseOrderExporter.Setup(exp => exp.SetConfig(fakeConfig.Object.WamasFtpConfig,
            fakeConfig.Object.ConnectionString, fakeMessageLogger.Object));
        fakePurchaseOrderExporter.Setup(exp => exp.ExportPurchaseOrders(It.IsAny<DateTime>())).Returns(fakeJobResult);

        var job = new PurchaseOrderExportJob(fakePurchaseOrderExporter.Object)
        {
            Config = fakeConfig.Object
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        job.Execute(jobInfrastructureMock.Object);
        Assert.True(job.Result.Result == fakeJobResult.Result);
        Assert.True(job.Result.ExportedCount == fakeJobResult.ExportedCount);
        Assert.True(job.Result.ErrorCount == fakeJobResult.ErrorCount);
    }
}