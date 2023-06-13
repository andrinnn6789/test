using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.GoodsReceiptImport.BusinessLogic;
using IAG.VinX.CDV.Wamas.GoodsReceiptImport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Wamas.GoodsReceiptImport.ProcessEngine;

public class GoodsReceiptImportJobTest
{
    [Fact]
    public void ExecuteGoodsReceiptImportJob()
    {
        var fakeConfig = new Mock<GoodsReceiptImportJobConfig>
        {
            Object =
            {
                WamasFtpConfig = new WamasFtpConfig(),
                ConnectionString = "test"
            }
        };

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeJobResult = new WamasImportJobResult
        {
            Result = JobResultEnum.Success,
            ImportedCount = 1,
            ErrorCount = 0
        };

        var fakeGoodsReceiptImporter = new Mock<IGoodsReceiptImporter>();
        fakeGoodsReceiptImporter.Setup(exp => exp.SetConfig(fakeConfig.Object.WamasFtpConfig,
            fakeConfig.Object.ConnectionString, fakeMessageLogger.Object));
        fakeGoodsReceiptImporter.Setup(exp => exp.ImportGoodsReceipts()).Returns(fakeJobResult);

        var job = new GoodsReceiptImportJob(fakeGoodsReceiptImporter.Object)
        {
            Config = fakeConfig.Object
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        job.Execute(jobInfrastructureMock.Object);
        Assert.True(job.Result.Result == fakeJobResult.Result);
        Assert.True(job.Result.ImportedCount == fakeJobResult.ImportedCount);
        Assert.True(job.Result.ErrorCount == fakeJobResult.ErrorCount);
    }
}