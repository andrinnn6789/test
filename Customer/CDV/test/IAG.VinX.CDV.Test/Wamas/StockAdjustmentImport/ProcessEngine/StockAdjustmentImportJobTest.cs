using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.StockAdjustmentImport.BusinessLogic;
using IAG.VinX.CDV.Wamas.StockAdjustmentImport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Wamas.StockAdjustmentImport.ProcessEngine;

public class StockAdjustmentImportJobTest
{
    [Fact]
    public void ExecuteStockAdjustmentImportJob()
    {
        var fakeConfig = new Mock<StockAdjustmentImportJobConfig>
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

        var fakeStockAdjustmentImporter = new Mock<IStockAdjustmentImporter>();
        fakeStockAdjustmentImporter.Setup(exp => exp.SetConfig(fakeConfig.Object.WamasFtpConfig,
            fakeConfig.Object.ConnectionString, fakeMessageLogger.Object));
        fakeStockAdjustmentImporter.Setup(exp => exp.ImportStockAdjustments()).Returns(fakeJobResult);

        var job = new StockAdjustmentImportJob(fakeStockAdjustmentImporter.Object)
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