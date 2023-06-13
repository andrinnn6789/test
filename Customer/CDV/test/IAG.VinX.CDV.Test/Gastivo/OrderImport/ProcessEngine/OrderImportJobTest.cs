using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.Gastivo.Common.Config;
using IAG.VinX.CDV.Gastivo.Common.ProcessEngine;
using IAG.VinX.CDV.Gastivo.OrderImport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.OrderImport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Gastivo.OrderImport.ProcessEngine;

public class OrderImportJobTest
{
    [Fact]
    public void ExecutePriceExportJobTest()
    {
        // Arrange
        var fakeConfig = new Mock<OrderImportJobConfig>
        {
            Object =
            {
                GastivoFtpConfig = new GastivoFtpConfig(),
                ConnectionString = "test"
            }
        };

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeJobResult = new GastivoImportJobResult()
        {
            Result = JobResultEnum.Success,
            ImportedCount = 1,
            ErrorCount = 0
        };

        var fakePriceExporter = new Mock<IOrderImporter>();
        fakePriceExporter.Setup(exp => exp.SetConfig(fakeConfig.Object.GastivoFtpConfig,
            fakeConfig.Object.ConnectionString, fakeMessageLogger.Object));
        fakePriceExporter.Setup(exp => exp.ImportOrders()).Returns(fakeJobResult);

        var job = new OrderImportJob(fakePriceExporter.Object)
        {
            Config = fakeConfig.Object
        };
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();

        // Act
        job.Execute(jobInfrastructureMock.Object);

        // Assert
        Assert.True(job.Result.Result == fakeJobResult.Result);
        Assert.True(job.Result.ImportedCount == fakeJobResult.ImportedCount);
        Assert.True(job.Result.ErrorCount == fakeJobResult.ErrorCount);
    }
}