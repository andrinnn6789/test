using System;

using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.IntegrationTest.Wamas.Common;
using IAG.VinX.CDV.Wamas.PartnerExport.BusinessLogic;
using IAG.VinX.CDV.Wamas.PartnerExport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.IntegrationTest.Wamas.PartnerExport.ProcessEngine;

public class PartnerExportJobTest
{
    [Fact]
    public void ExecutePartnerExportJob()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();
        var ftpConfig = FtpHelper.CreateFtpConfig();
        var ftpConnector = FtpHelper.CreateFtpConnector();
        var partnerExporter = new PartnerExporter(factory, ftpConnector);

        var job = new PartnerExportJob(partnerExporter)
        {
            Config = new PartnerExportJobConfig
            {
                ConnectionString = factory.ConnectionString,
                WamasFtpConfig = ftpConfig
            }
        };

        var jobState = new PartnerExportJobState()
        {
            Id = Guid.NewGuid(),
            LastSync = DateTime.Now.AddMinutes(-10)
        };
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        jobInfrastructureMock.Setup(m => m.GetJobData<PartnerExportJobState>()).Returns(jobState);
        
        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
    }
}