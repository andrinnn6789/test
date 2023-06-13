using System.Collections.Generic;
using System.IO;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.Zweifel.MySign.ProcessEngine.Export;

using Moq;

using Xunit;

namespace IAG.VinX.Zweifel.IntegrationTest.MySign.ProcessEngine;

public class ExportAllJobTest
{
    private readonly ISybaseConnectionFactory _factory;
    private const string ExportFolder = "Export";

    public ExportAllJobTest()
    {
        if (!Directory.Exists(ExportFolder))
            Directory.CreateDirectory(ExportFolder);
        _factory = SybaseConnectionFactoryHelper.CreateFactory();
    }

    [Fact]
    public void ExecuteBaseDataJobTest()
    {
        var userContext = new ExplicitUserContext("test", null);
        var job = new ExportBaseDataJob(new SybaseConnectionFactory(userContext, new MockILogger<SybaseConnection>(), null, null))
        {
            Config = new ExportBaseDataConfig
            {
                Config = new ExportBaseConfig
                {
                    ConnectionString = _factory.ConnectionString,
                    ExportFolder = ExportFolder
                }
            }
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
        Assert.True(job.Result.SyncResult.Count > 0);
        Assert.Equal(JobResultEnum.Success, job.Result.Result);
    }

    [Fact]
    public void ExecuteStockJobTest()
    {
        var userContext = new ExplicitUserContext("test", null);
        var job = new ExportStockJob(new SybaseConnectionFactory(userContext, new MockILogger<SybaseConnection>(), null, null))
        {
            Config = new ExportStockConfig
            {
                Config = new ExportBaseConfig
                {
                    ConnectionString = _factory.ConnectionString,
                    ExportFolder = ExportFolder
                }
            }
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
        Assert.True(job.Result.SyncResult.Count > 0);
        Assert.Equal(JobResultEnum.Success, job.Result.Result);
    }

    [Fact]
    public void ExecuteCustomerJobTest()
    {
        var userContext = new ExplicitUserContext("test", null);
        var job = new ExportCustomerJob(new SybaseConnectionFactory(userContext, new MockILogger<SybaseConnection>(), null, null))
        {
            Config = new ExportCustomerConfig
            {
                Config = new ExportBaseConfig
                {
                    ConnectionString = _factory.ConnectionString,
                    ExportFolder = ExportFolder
                }
            }
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
        Assert.True(job.Result.SyncResult.Count > 0);
        Assert.Equal(JobResultEnum.Success, job.Result.Result);
    }

    [Fact]
    public void InvalidPathTest()
    {
        var userContext = new ExplicitUserContext("test", null);
        var job = new ExportBaseDataJob(new SybaseConnectionFactory(userContext, new MockILogger<SybaseConnection>(), null, null))
        {
            Config = new ExportBaseDataConfig
            {
                Config = new ExportBaseConfig
                {
                    ConnectionString = _factory.ConnectionString,
                    ExportFolder = "xxx"
                }
            }
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var messages = new List<MessageStructure>();
        jobInfrastructureMock.Setup(m => m.AddMessage(It.IsAny<MessageStructure>()))
            .Callback<MessageStructure>(msg => messages.Add(msg));

        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
        Assert.Equal(JobResultEnum.Failed, job.Result.Result);
        Assert.True(messages.Count > 0);
    }
}