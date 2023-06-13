using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;

using Microsoft.Extensions.Localization;

namespace IAG.Infrastructure.Globalisation.Localizer;

/// <summary>
///   Custom localizers with db-backend
/// </summary>
public class DbStringLocalizer : IStringLocalizer
{
    private readonly SortedList<string, string> _translations;
    private CultureInfo _myCulture;

    public DbStringLocalizer(SortedList<string, string> translations)
    {
        _translations = translations;
        _myCulture = Thread.CurrentThread.CurrentUICulture;
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = GetString(name);
            return new LocalizedString(name, value ?? name, value == null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var value = GetString(name, arguments);
            return new LocalizedString(name, value, name == value);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures)
    {
        var culturePrefix = includeAncestorCultures ? 
            _myCulture.Name.Split("-")[0] : 
            _myCulture.Name + DbStringLocalizerFactory.CultureKeyToken;
        return (from key in _translations.Keys
            where key.StartsWith(culturePrefix)
            select key.Substring(culturePrefix.Length)
            into name
            select new LocalizedString(name, GetString(name))).ToList();
    }

    [ExcludeFromCodeCoverage]  // Interface needs it
    public IStringLocalizer WithCulture(CultureInfo culture)
    {
        return new DbStringLocalizer(_translations)
        {
            _myCulture = culture
        };
    }

    private string GetString(string name, params object[] arguments)
    {
        for (var i = 0; i < arguments?.Length; i++)
        {
            if (arguments[i] is ILocalizableObject localizableArg)
            {
                arguments[i] = GetString(localizableArg.ResourceId, localizableArg.Params);
            }
        }

        var translated = GetString(name);
        if (arguments?.Length > 0)
        {
            try
            {
                translated = string.Format(translated, arguments);
            }
            catch (FormatException)
            {
                // replacement of placeholders failed for some reason. Just return string without replacement...
            }
        }

        return translated;
    }

    private string GetString(string name)
    {
        var culturePrefix = _myCulture.Name + DbStringLocalizerFactory.CultureKeyToken;
        var culturePrefixBase = _myCulture.Name.Split("-")[0] + DbStringLocalizerFactory.CultureKeyToken;
        var culturePrefixFallback = "en" + DbStringLocalizerFactory.CultureKeyToken;

        var key = culturePrefix + name;
        if (_translations.ContainsKey(key)) return _translations[key];
        key = culturePrefixBase + name;
        if (_translations.ContainsKey(key)) return _translations[key];
        key = culturePrefixFallback + name;
        return _translations.ContainsKey(key) ? _translations[key] : name;
    }
}

public class DbStringLocalizer<T> : DbStringLocalizer, IStringLocalizer<T>
{
    public DbStringLocalizer(SortedList<string, string> translations) : base(translations)
    {
    }
}