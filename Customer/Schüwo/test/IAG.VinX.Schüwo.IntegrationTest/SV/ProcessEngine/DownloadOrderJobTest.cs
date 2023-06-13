using System;
using System.IO;
using System.Reflection;
using System.Text;

using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.Ftp;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.Schüwo.SV.ProcessEngine.DownloadOrder;

using Moq;

using Xunit;

namespace IAG.VinX.Schüwo.IntegrationTest.SV.ProcessEngine;

public class DownloadOrderJobTest
{
    [Fact]
    public void ExecuteDownloadOrderJobTest()
    {
        var factory = SybaseConnectionFactoryHelper.CreateFactory();
        var config = new DownloadOrderJobConfig
        {
            VinXConnectionString = factory.ConnectionString,
            FtpEndpointConfig = ConfigHelper.FtpEndpointTest,
            FtpPathConfig = ConfigHelper.FtpPathConfigTest
        };
        var ftpClient = new SimpleFtpClient(
            config.FtpEndpointConfig.Url, 
            config.FtpEndpointConfig.User, 
            config.FtpEndpointConfig.Password);
        ftpClient.Connect();
        var orderdata = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(GetType().Namespace + ".orderfile.txt")!);
        var orderfile = new MemoryStream(Encoding.UTF8.GetBytes(orderdata.ReadToEnd().Replace("<orderid>", Guid.NewGuid().ToString())));
        ftpClient.Upload(
            orderfile,
            config.FtpPathConfig.DownloadOrderDir + "/testorder.txt",
            true, 
            false);
        ftpClient.Disconnect();

        var job = new DownloadOrderJob(factory)
        {
            Config = config
        };
        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);
        Assert.True(result);
        Assert.True(job.Result.ErrorCount == 0);
    }
}