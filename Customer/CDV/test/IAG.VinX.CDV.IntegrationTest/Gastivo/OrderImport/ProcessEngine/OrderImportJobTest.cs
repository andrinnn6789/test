using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.IntegrationTest.Gastivo.Common;
using IAG.VinX.CDV.Gastivo.OrderImport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.OrderImport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.IntegrationTest.Gastivo.OrderImport.ProcessEngine;

public class OrderImportJobTest
{
    [Fact]
    public void ExecuteOrderImportJob()
    {
        var factory = NHibernateSessionContextFactoryHelper.CreateFactory();
        var ftpConfig = FtpHelper.CreateFtpConfig();
        var ftpConnector = FtpHelper.CreateFtpConnector();
        var orderImporter = new OrderImporter(factory, ftpConnector);

        var job = new OrderImportJob(orderImporter)
        {
            Config = new OrderImportJobConfig
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