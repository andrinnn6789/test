using System;
using System.Linq;

using IAG.Infrastructure.Configuration.Model;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.ProcessEngine.DataLayer.Settings.Context;
using IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;
using IAG.ProcessEngine.Startup;
using IAG.ProcessEngine.Store;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;

using Xunit;

namespace IAG.ProcessEngine.IntegrationTest.Configuration;

[Collection("SequentialTestOfProcessEngine")]
public class ConfigStoreDbTest
{
    private readonly IJobConfigStore _store;
    private readonly SettingsDbContext _context;

    public ConfigStoreDbTest()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton(Infrastructure.Startup.Startup.BuildConfig());
        services.AddScoped<IUserContext>((_) => new ExplicitUserContext("test", null));
        services.AddLogging();
        new ServerConfigureServices().ConfigureServices(services, new HostingEnvironment());
        var provider = services.BuildServiceProvider();
        var jobCatalogue = provider.GetRequiredService<IJobCatalogue>();
        _context = provider.GetRequiredService<SettingsDbContext>();
        _store = new JobConfigStoreDb(_context, jobCatalogue, null, new MockILogger<JobConfigStoreDb>());
    }

    [Fact]
    public void TestConfigJobBaseCrud()
    {
        var configIn = new HelloConfig
        {
            RetryIntervals = Enumerable.Repeat(new TimeSpan(0, 2, 0), 3).ToArray()
        };

        // insert
        _store.Insert(configIn);
        Assert.Throws<DuplicateKeyException>(() => _store.Insert(configIn));

        // read
        var configRead = _store.Read(configIn.Id);
        var configReadJob = (HelloConfig)configRead;
        Assert.Equal(configIn.Id, configRead.Id);
        Assert.Equal(configIn.Name, configReadJob.Name);
        Assert.Equal(3, configReadJob.RetryIntervals.Length);
        Assert.Equal(2, configReadJob.RetryIntervals[1].Minutes);

        // update
        configIn.Name = "Hallo 2";
        _store.Update(configIn);
        configRead = _store.Read(configIn.Id);
        Assert.Equal("Hallo 2", configRead.Name);

        // delete
        _store.Delete(configIn);
        Assert.Throws<NotFoundException>(() => _store.Read(configIn.Id));
        Assert.Throws<NotFoundException>(() => _store.Update(configIn));
        Assert.Throws<NotFoundException>(() => _store.Delete(configIn));
    }

    [Fact]
    public void ConfigJobReplacementTest()
    {
        var configIn = new HelloConfig
        {
            Description = "Test $$Text$"
        };

        _store.Insert(configIn);
        _context.ConfigCommonEntries.Add(new ConfigCommon { Name = "Text", Data = "42" });
        _context.SaveChanges();

        var configRead = _store.Read(configIn.Id);
        var configReadJob = Assert.IsType<HelloConfig>(configRead);
        Assert.Equal("Test 42", configReadJob.Description);
    }
}