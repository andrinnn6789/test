using System.Collections.Generic;
using System.IO;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.Zweifel.MySign.ProcessEngine.Import;

using Moq;

using Xunit;

namespace IAG.VinX.Zweifel.IntegrationTest.MySign.ProcessEngine;

public class ImportJobTest
{
    private readonly ISybaseConnectionFactory _factory;
    private const string ImportFolder = "Import";
    private const string BackupFolder = "Import\\Sicherung";

    public ImportJobTest()
    {
        if (!Directory.Exists(ImportFolder))
            Directory.CreateDirectory(ImportFolder);
        if (!Directory.Exists(BackupFolder))
            Directory.CreateDirectory(BackupFolder);
        _factory = SybaseConnectionFactoryHelper.CreateFactory();
    }

    [Fact]
    public void ExecuteImportJobTest()
    {
        var userContext = new ExplicitUserContext("test", null);
        var job = new ImportJob(new SybaseConnectionFactory(userContext, new MockILogger<SybaseConnection>(), null, null))
        {
            Config = new ImportConfig
            {
                ConnectionString = _factory.ConnectionString,
                ImportFolder = ImportFolder,
                BackupFolder = BackupFolder,
                ShippingCostRef = 2005,
                PaymentMethodRef = 2,
                PamenyConditionRef = 3
            }
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.True(result);
        Assert.Equal(JobResultEnum.Success, job.Result.Result);
    }

    [Fact]
    public void InvalidPathTest()
    {
        var userContext = new ExplicitUserContext("test", null);
        var job = new ImportJob(new SybaseConnectionFactory(userContext, new MockILogger<SybaseConnection>(), null, null))
        {
            Config = new ImportConfig
            {
                ConnectionString = _factory.ConnectionString,
                ImportFolder = "xxx"
            }
        };

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        var messages = new List<MessageStructure>();
        jobInfrastructureMock.Setup(m => m.AddMessage(It.IsAny<MessageStructure>()))
            .Callback<MessageStructure>(msg => messages.Add(msg));

        var result = job.Execute(jobInfrastructureMock.Object);

        Assert.False(result);
        Assert.Equal(JobResultEnum.Failed, job.Result.Result);
        Assert.True(messages.Count > 0);
    }
}