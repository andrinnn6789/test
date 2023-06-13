using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.StockReportImport.BusinessLogic;
using IAG.VinX.CDV.Wamas.StockReportImport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Wamas.StockReportImport.ProcessEngine;

public class StockReportImportJobTest
{
    [Fact]
    public void ExecuteStockReportImportJob()
    {
        var fakeConfig = new Mock<StockReportImportJobConfig>
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

        var fakeStockReportImporter = new Mock<IStockReportImporter>();
        fakeStockReportImporter.Setup(exp => exp.SetConfig(fakeConfig.Object.WamasFtpConfig,
            fakeConfig.Object.ConnectionString, fakeMessageLogger.Object));
        fakeStockReportImporter.Setup(exp => exp.ImportStockReports()).Returns(fakeJobResult);

        var job = new StockReportImportJob(fakeStockReportImporter.Object)
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