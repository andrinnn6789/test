using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Microsoft.Extensions.Localization;

namespace IAG.Infrastructure.TestHelper.Globalization.Mocks;

[ExcludeFromCodeCoverage]
public class MockLocalizer<T> : IStringLocalizer<T>
{
    LocalizedString IStringLocalizer.this[string name] => new(name, name, false);

    LocalizedString IStringLocalizer.this[string name, params object[] arguments]
    {
        get
        {
            var value = name;
            if (arguments != null)
            {
                value = string.Format(name, arguments);
            }

            return new LocalizedString(name, value, true);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        throw new NotImplementedException();
    }

    public IStringLocalizer WithCulture(CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}