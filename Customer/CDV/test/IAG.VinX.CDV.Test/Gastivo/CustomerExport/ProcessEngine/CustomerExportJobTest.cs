using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.Gastivo.Common.Config;
using IAG.VinX.CDV.Gastivo.Common.ProcessEngine;
using IAG.VinX.CDV.Gastivo.CustomerExport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.CustomerExport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Gastivo.CustomerExport.ProcessEngine;

public class CustomerExportJobTest
{
    [Fact]
    public void ExecuteCustomerExportJobTest()
    {
        // Arrange
        var fakeConfig = new Mock<CustomerExportJobConfig>
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

        var fakeCustomerExporter = new Mock<ICustomerExporter>();
        fakeCustomerExporter.Setup(exp => exp.SetConfig(fakeConfig.Object.GastivoFtpConfig,
            fakeConfig.Object.ConnectionString, fakeMessageLogger.Object));
        fakeCustomerExporter.Setup(exp => exp.ExportCustomers()).Returns(fakeJobResult);

        var job = new CustomerExportJob(fakeCustomerExporter.Object)
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