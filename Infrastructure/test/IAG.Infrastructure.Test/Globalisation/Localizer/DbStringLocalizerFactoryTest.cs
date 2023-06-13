using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.TestHelper.Globalization.ResourceProvider;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

using Xunit;

namespace IAG.Infrastructure.Test.Globalisation.Localizer;

public class DbStringLocalizerFactoryTest
{
    private readonly IStringLocalizerFactoryReloadable _factory;

    private readonly ResourceContext _resourceContext;

    private readonly Guid _resourceId;

    public DbStringLocalizerFactoryTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ResourceContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        _resourceContext = ResourceContextBuilder.GetNewContext();
        _resourceContext.Resources.AddRange(new Infrastructure.Globalisation.Model.Resource { Name = "test" });
        _resourceContext.Cultures.AddRange(
            new Culture { Name = "de" },
            new Culture { Name = "de-CH" },
            new Culture { Name = "fr" },
            new Culture { Name = "fr-CH" },
            new Culture { Name = "en" });
        _resourceContext.SaveChanges();
        _resourceId = _resourceContext.Resources.First(r => r.Name == "test").Id;
        _resourceContext.Translations.AddRange(
            new Translation
            {
                CultureId = _resourceContext.Cultures.First(c => c.Name == "de").Id,
                ResourceId = _resourceId,
                Value = "de"
            },
            new Translation
            {
                CultureId = _resourceContext.Cultures.First(c => c.Name == "de-CH").Id,
                ResourceId = _resourceId,
                Value = "de-CH"
            },
            new Translation
            {
                CultureId = _resourceContext.Cultures.First(c => c.Name == "fr").Id,
                ResourceId = _resourceId,
                Value = "fr"
            });
        _resourceContext.SaveChanges();
        _factory = new DbStringLocalizerFactory(_resourceContext);
        Debug.Assert(_factory != null, nameof(_factory) + " != null");
    }

    [Fact]
    public void TestResoureReload()
    {
        CultureInfo.CurrentCulture = new CultureInfo("en");
        var localizer = _factory.Create(GetType());
        Assert.Equal("test_reload", localizer.GetString("test_reload"));
        _resourceContext.Translations.AddRange(
            new Translation
            {
                CultureId = _resourceContext.Cultures.First(c => c.Name == "en").Id,
                ResourceId = _resourceId,
                Value = "test_reload_found"
            });
        _resourceContext.SaveChanges();
        _factory.Reload();
        localizer = _factory.Create(string.Empty, string.Empty);
        Assert.Equal("test_reload_found", localizer.GetString("test"));
        localizer = _factory.Create(GetType());
        Assert.Equal("test_reload_found", localizer.GetString("test"));
    }
}