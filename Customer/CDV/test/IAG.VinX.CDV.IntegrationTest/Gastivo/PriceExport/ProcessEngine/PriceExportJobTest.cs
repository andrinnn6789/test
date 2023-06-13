using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.CDV.IntegrationTest.Gastivo.Common;
using IAG.VinX.CDV.Gastivo.PriceExport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.PriceExport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.IntegrationTest.Gastivo.PriceExport.ProcessEngine;

public class PriceExportJobTest
{
    [Fact]
    public void ExecutePriceExportJob()
    {
        var factory = NHibernateSessionContextFactoryHelper.CreateFactory();
        var ftpConfig = FtpHelper.CreateFtpConfig();
        var ftpConnector = FtpHelper.CreateFtpConnector();
        var priceExporter = new PriceExporter(factory, ftpConnector);

        var job = new PriceExportJob(priceExporter)
        {
            Config = new PriceExportJobConfig
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