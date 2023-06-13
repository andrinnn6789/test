using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.Gastivo.Common.Config;
using IAG.VinX.CDV.Gastivo.Common.ProcessEngine;
using IAG.VinX.CDV.Gastivo.PriceExport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.PriceExport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Gastivo.PriceExport.ProcessEngine;

public class PriceExportJobTest
{
    [Fact]
    public void ExecutePriceExportJobTest()
    {
        // Arrange
        var fakeConfig = new Mock<PriceExportJobConfig>
        {
            Object =
            {
                GastivoFtpConfig = new GastivoFtpConfig(),
                ConnectionString = "test"
            }
        };

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeJobResult = new GastivoExportJobResult()
        {
            Result = JobResultEnum.Success,
            ExportedCount = 1,
            ErrorCount = 0
        };

        var fakePriceExporter = new Mock<IPriceExporter>();
        fakePriceExporter.Setup(exp => exp.SetConfig(fakeConfig.Object.GastivoFtpConfig,
            fakeConfig.Object.ConnectionString, fakeMessageLogger.Object));
        fakePriceExporter.Setup(exp => exp.ExportPrices()).Returns(fakeJobResult);

        var job = new PriceExportJob(fakePriceExporter.Object)
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