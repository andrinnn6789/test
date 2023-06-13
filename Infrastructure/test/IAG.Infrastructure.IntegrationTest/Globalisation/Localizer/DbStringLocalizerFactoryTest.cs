using System;
using System.Globalization;
using System.Linq;

using IAG.Infrastructure.Globalisation;
using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Globalisation.ResourceProvider;
using IAG.Infrastructure.IdentityServer.Authentication;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Localization;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.Globalisation.Localizer;

public class DbStringLocalizerFactoryTest: IClassFixture<DbStringLocalizerFactoryFixture>
{
    private DbStringLocalizerFactoryFixture Fixture { get; }

    public DbStringLocalizerFactoryTest(DbStringLocalizerFactoryFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void TestResoureExact()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-CH");
        var localizer = Fixture.Factory.Create(GetType());
        Assert.Equal("de-CH", localizer.GetString("test"));
    }

    [Fact]
    public void TestResourePrefixed()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-CH");
        var localizer = Fixture.Factory.Create(GetType());
        Assert.Equal("sys", localizer.GetString("testSys"));
    }

    [Fact]
    public void TestResoureFallback()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-AT");
        var localizer = Fixture.Factory.Create(GetType());
        Assert.Equal("de", localizer.GetString("test"));
    }

    [Fact]
    public void TestResoureNotFound()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-CH");
        var localizer = Fixture.Factory.Create(GetType());
        Assert.Equal("test_not_there", localizer.GetString("test_not_there"));
    }

    [Fact]
    public void TestResoureReload()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var localizer = Fixture.Factory.Create(GetType());
        Assert.Equal("test_reload", localizer.GetString("test_reload"));
        Fixture.ResourceContext.Translations.AddRange(
            new Translation
            {
                CultureId = Fixture.ResourceContext.Cultures.First(c => c.Name == "en").Id,
                ResourceId = Fixture.ResourceId,
                Value = "test_reload_found"
            });
        Fixture.ResourceContext.SaveChanges();
        Fixture.Factory.Reload();
        localizer = Fixture.Factory.Create(GetType());
        Assert.Equal("test_reload_found", localizer.GetString("test"));
    }
}

[UsedImplicitly]
public class DbStringLocalizerFactoryFixture
{
    public readonly ResourceContext ResourceContext;

    public readonly IStringLocalizerFactoryReloadable Factory;

    public readonly Guid ResourceId;

    public DbStringLocalizerFactoryFixture()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton(Startup.Startup.BuildConfig());
        services.AddScoped<IUserContext>(_ => new ExplicitUserContext("test", null));
        var resConfig = new ResourceConfigureServices();
        resConfig.ConfigureServices(services, new HostingEnvironment());
        var provider = services.BuildServiceProvider();
        ResourceContext = provider.GetRequiredService<ResourceContext>();
        ResourceContext.Resources.AddRange(
            new Infrastructure.Globalisation.Model.Resource { Name = "test" },
            new Infrastructure.Globalisation.Model.Resource { Name = ResourceCollector.SystemResourcePrefix + "testSys" });
        ResourceContext.Cultures.AddRange(
            new Culture {Name = "de"},
            new Culture {Name = "de-CH"},
            new Culture {Name = "fr"},
            new Culture {Name = "fr-CH"},
            new Culture {Name = "en"});
        ResourceContext.SaveChanges();
        ResourceId = ResourceContext.Resources.First(r => r.Name == "test").Id;
        ResourceContext.Translations.AddRange(
            new Translation
            {
                CultureId = ResourceContext.Cultures.First(c => c.Name == "de").Id,
                ResourceId = ResourceId,
                Value = "de"
            },
            new Translation
            {
                CultureId = ResourceContext.Cultures.First(c => c.Name == "de-CH").Id,
                ResourceId = ResourceId,
                Value = "de-CH"
            },
            new Translation
            {
                CultureId = ResourceContext.Cultures.First(c => c.Name == "de-CH").Id,
                ResourceId = ResourceContext.Resources.First(r => r.Name == ResourceCollector.SystemResourcePrefix + "testSys").Id,
                Value = "sys"
            },
            new Translation
            {
                CultureId = ResourceContext.Cultures.First(c => c.Name == "fr").Id,
                ResourceId = ResourceId,
                Value = "fr"
            });
        ResourceContext.SaveChanges();
        Factory = provider.GetRequiredService<IStringLocalizerFactoryReloadable>();
        Assert.NotNull(Factory);
    }
}