using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.Smith.BossExport.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.Smith.IntegrationTest.BossExport.ProcessEngine;

public class BossExportJobTest
{
    [Fact]
    public void ExecuteJobTest()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();
        var mailConfig = new Common.TestHelper.MailSender.ConfigHelper();
        var job = new BossExportJob(factory, new MockILogger<BossExportJob>())
        {
            Config = new BossExportJobConfig
            {
                VinXConnectionString = factory.ConnectionString,
                MailReceiver = mailConfig.MailSenderJobConfig.MailReceiverTest,
                MailConfig = mailConfig.MailSenderConfig
            },
            Parameter = new BossExportJobParameter
            {
                ExportAll = true,
                UpdateExported = true
            }
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
        Assert.True(job.Result.ExportCount > 0);
    }
}