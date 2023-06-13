using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.IntegrationTest.Wamas.Common;
using IAG.VinX.CDV.Wamas.PickListExport.BusinessLogic;
using IAG.VinX.CDV.Wamas.PickListExport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.IntegrationTest.Wamas.PickListExport.ProcessEngine;

public class PickListExportJobTest
{
    [Fact]
    public void ExecutePickListExportJob()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();
        var ftpConfig = FtpHelper.CreateFtpConfig();
        var ftpConnector = FtpHelper.CreateFtpConnector();
        var pickListExporter = new PickListExporter(factory, ftpConnector);

        var job = new PickListExportJob(pickListExporter)
        {
            Config = new PickListExportJobConfig
            {
                ConnectionString = factory.ConnectionString,
                WamasFtpConfig = ftpConfig,
                ExportDayOffset = "10",
                LeadTimeMinutesOffset = "30"
            }
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
    }
}