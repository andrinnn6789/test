using System;
using System.IO;

using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.ProcessEngine.DataLayer.Settings.Context;
using IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;
using IAG.ProcessEngine.Execution;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Startup;
using IAG.ProcessEngine.Store;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.IntegrationTest.Execution;

[Collection("SequentialTestOfProcessEngine")]
public class JobBuilderTest
{
    private static readonly string TestUser = "testUser";
    private static readonly Guid TestTenant = Guid.NewGuid();

    private readonly IJobConfigStore _store;
    private readonly IJobCatalogue _jobCatalogue;
    private readonly JobBuilder _builder;

    public JobBuilderTest()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton(Infrastructure.Startup.Startup.BuildConfig());
        services.AddScoped<IUserContext>((_) => new ExplicitUserContext(TestUser, TestTenant));
        services.AddLogging();
        new ServerConfigureServices().ConfigureServices(services, new HostingEnvironment());
        var provider = services.BuildServiceProvider();

        _jobCatalogue = provider.GetRequiredService<IJobCatalogue>();
        var context = new SettingsDbContext(new DbContextOptionsBuilder<SettingsDbContext>().UseSqlite($"Data Source={Path.GetTempFileName()}").Options, new ExplicitUserContext("test", null));
        context.Database.EnsureCreated();
        _store = new JobConfigStoreDb(context, _jobCatalogue, null, new MockILogger<JobConfigStoreDb>());
        _builder = new JobBuilder(services.BuildServiceProvider(), _jobCatalogue, _store);
    }


    [Fact]
    public void TestConstructorTest()
    {
        Assert.NotEmpty(_jobCatalogue.Catalogue);
    }

    [Fact]
    public void BuildTest()
    {
        var helloConfig = new HelloConfig
        {
            NbOfOutputs = 10,
        };

        _store.Insert(helloConfig);

        using IJobInstance jobInstance = _builder.BuildInstance(helloConfig.Id, null);
        var helloJob = Assert.IsType<HelloJob>(jobInstance?.Job);

        Assert.NotNull(jobInstance);
        Assert.NotNull(helloJob);
        Assert.Equal(10, helloJob.Config.NbOfOutputs);
        Assert.Equal("Hello-Job", helloJob.Parameter.GreetingsFrom);
        Assert.Equal(TestUser, jobInstance.State.Owner);
        Assert.Equal(TestTenant, jobInstance.State.ContextTenant);
    }

    [Fact]
    public void ReBuildTest()
    {
        var helloConfig = new HelloConfig
        {
            NbOfOutputs = 10,
        };

        var mockJobState = new Mock<IJobState>();
        mockJobState.Setup(m => m.TemplateId).Returns(helloConfig.TemplateId);
        mockJobState.Setup(m => m.JobConfigId).Returns(helloConfig.Id);
        mockJobState.Setup(m => m.Owner).Returns(TestUser);
        mockJobState.Setup(m => m.ContextTenant).Returns(TestTenant);

        _store.Insert(helloConfig);
        using IJobInstance jobInstance = _builder.ReBuildInstance(mockJobState.Object);
        var helloJob = Assert.IsType<HelloJob>(jobInstance?.Job);

        Assert.NotNull(jobInstance);
        Assert.NotNull(helloJob);
        Assert.Equal(10, helloJob.Config.NbOfOutputs);
        Assert.Equal("Hello-Job", helloJob.Parameter.GreetingsFrom);
        Assert.Equal(TestUser, jobInstance.State.Owner);
        Assert.Equal(TestTenant, jobInstance.State.ContextTenant);
    }

    [Fact]
    public void BuildInvalidIdTest()
    {
        Assert.Throws<NotFoundException>(() => _builder.BuildInstance(Guid.NewGuid(), null));
    }
}