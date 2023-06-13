using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;
using IAG.VinX.CDV.Wamas.PickListExport.BusinessLogic;
using IAG.VinX.CDV.Wamas.PickListExport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Wamas.PickListExport.ProcessEngine;

public class PickListExportJobTest
{
    [Fact]
    public void ExecuteArticleExportJob()
    {
        var fakeConfig = new Mock<PickListExportJobConfig>
        {
            Object =
            {
                WamasFtpConfig = new WamasFtpConfig(),
                ConnectionString = "test",
                ExportDayOffset = "10",
                LeadTimeMinutesOffset = "30"
            }
        };

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeJobResult = new WamasExportJobResult
        {
            Result = JobResultEnum.Success,
            ExportedCount = 1,
            ErrorCount = 0
        };

        var fakePicklistExporter = new Mock<IPickListExporter>();
        fakePicklistExporter.Setup(exp => exp.SetConfig(fakeConfig.Object.WamasFtpConfig,
            fakeConfig.Object.ConnectionString, fakeMessageLogger.Object));
        fakePicklistExporter.Setup(exp => exp.ExportPickLists(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(fakeJobResult);

        var job = new PickListExportJob(fakePicklistExporter.Object)
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