using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.PartnerExport.BusinessLogic;
using IAG.VinX.CDV.Wamas.PartnerExport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Wamas.PartnerExport.ProcessEngine;

public class PartnerExportJobTest
{
    [Fact]
    public void ExecutePartnerExportJobTest()
    {
        // Arrange
        var fakeConfig = new Mock<PartnerExportJobConfig>
        {
            Object =
            {
                WamasFtpConfig = new WamasFtpConfig(),
                ConnectionString = "test"
            }
        };

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeJobResult = new WamasExportJobResult
        {
            Result = JobResultEnum.Success,
            ExportedCount = 1,
            ErrorCount = 0
        };

        var fakeAddressExporter = new Mock<IPartnerExporter>();
        fakeAddressExporter.Setup(exp => exp.SetConfig(fakeConfig.Object.WamasFtpConfig,
            fakeConfig.Object.ConnectionString, fakeMessageLogger.Object));
        fakeAddressExporter.Setup(exp => exp.ExportPartner(It.IsAny<DateTime>())).Returns(fakeJobResult);

        var job = new PartnerExportJob(fakeAddressExporter.Object)
        {
            Config = fakeConfig.Object
        };
        
        var jobState = new PartnerExportJobState();
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        jobInfrastructureMock.Setup(m => m.GetJobData<PartnerExportJobState>()).Returns(jobState);

        // Act
        job.Execute(jobInfrastructureMock.Object);

        // Assert
        Assert.True(job.Result.Result == fakeJobResult.Result);
        Assert.True(job.Result.ExportedCount == fakeJobResult.ExportedCount);
        Assert.True(job.Result.ErrorCount == fakeJobResult.ErrorCount);
    }
}