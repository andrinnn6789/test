using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.IntegrationTest.Gastivo.Common;
using IAG.VinX.CDV.Gastivo.CustomerExport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.CustomerExport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.IntegrationTest.Gastivo.CustomerExport.ProcessEngine;

public class CustomerExportJobTest
{
    [Fact]
    public void ExecuteCustomerExportJob()
    {
        var factory = NHibernateSessionContextFactoryHelper.CreateFactory();
        var ftpConfig = FtpHelper.CreateFtpConfig();
        var ftpConnector = FtpHelper.CreateFtpConnector();
        var customerExporter = new CustomerExporter(factory, ftpConnector);

        var job = new CustomerExportJob(customerExporter)
        {
            Config = new CustomerExportJobConfig
            {
                ConnectionString = factory.ConnectionString,
                GastivoFtpConfig = ftpConfig
            }
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
    }
}