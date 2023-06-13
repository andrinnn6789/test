using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using IAG.Infrastructure.Globalisation.Localizer;

using Microsoft.Extensions.Localization;

using Xunit;

namespace IAG.Infrastructure.Test.Globalisation.Localizer;

public class CustomStringLocalizerTest
{
    private readonly SortedList<string, string> _translations;

    public CustomStringLocalizerTest()
    {
        _translations = new SortedList<string, string>
        {
            { "de" + DbStringLocalizerFactory.CultureKeyToken + "test", "de" },
            { "de" + DbStringLocalizerFactory.CultureKeyToken + "testWithParam", "de {0}" },
            { "de" + DbStringLocalizerFactory.CultureKeyToken + "testWithInvalidFormatParam", "de {1}" },
            { "de" + DbStringLocalizerFactory.CultureKeyToken + "localizableParam", "testDe" },
            { "de" + DbStringLocalizerFactory.CultureKeyToken + "localizableParamWithParam", "testWithParam {0}" },
            { "de-CH" + DbStringLocalizerFactory.CultureKeyToken + "test", "de-CH" },
            { "fr" + DbStringLocalizerFactory.CultureKeyToken + "test", "fr" },
        };
    }

    [Fact]
    public void ResourceExactTest()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-CH");
        var localizer = new DbStringLocalizer(_translations);
        Assert.Equal("de-CH", localizer.GetString("test"));
        Assert.Equal("de-CH", localizer.GetString("test", string.Empty));
    }

    [Fact]
    public void ResourceWithParamTest()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-CH");
        var localizer = new DbStringLocalizer(_translations);
        Assert.Equal("de param", localizer.GetString("testWithParam", "param"));
    }

    [Fact]
    public void ResourceWithInvalidFormatParamTest()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-CH");
        var localizer = new DbStringLocalizer(_translations);
        Assert.Equal("de {1}", localizer.GetString("testWithInvalidFormatParam", "param"));
    }

    [Fact]
    public void ResourceWithLocalizableParamTest()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de");
        var localizer = new DbStringLocalizer(_translations);
        Assert.Equal("de testDe", localizer.GetString("testWithParam", new LocalizableParameter("localizableParam")));
    }

    [Fact]
    public void ResourceWithLocalizableParamWithParamTest()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de");
        var localizer = new DbStringLocalizer(_translations);
        Assert.Equal("de testWithParam 42", localizer.GetString("testWithParam", new LocalizableParameter("localizableParamWithParam", 42)));
    }

    [Fact]
    public void WithCultureTest()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("fr");
        IStringLocalizer localizer = new DbStringLocalizer(_translations);
        Assert.Equal("fr", localizer.GetString("test"));
    }

    [Fact]
    public void ResourceExactByStringStringCreateTest()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-CH");
        var localizer = new DbStringLocalizer(_translations);
        Assert.Equal("de-CH", localizer.GetString("test"));
    }

    [Fact]
    public void ResourceFallbackTest()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-AT");
        var localizer = new DbStringLocalizer(_translations);
        Assert.Equal("de", localizer.GetString("test"));
    }

    [Fact]
    public void ResourceNotFoundTest()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-CH");
        var localizer = new DbStringLocalizer(_translations);
        Assert.Equal("test_not_there", localizer.GetString("test_not_there"));
    }

    [Fact]
    public void GetAllStringsTest()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-CH");
        var localizer = new DbStringLocalizer(_translations);
        var count = _translations.Count(t => t.Key.StartsWith("de"));
        Assert.Single(localizer.GetAllStrings(false));
        Assert.Equal(count, localizer.GetAllStrings(true).Count());
    }
}