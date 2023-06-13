using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.ResourceProvider;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace IAG.Infrastructure.Globalisation.Localizer;

[UsedImplicitly]
public class DbStringLocalizerFactory : IStringLocalizerFactoryReloadable
{
    private SortedList<string, string> _translations;
    private readonly ResourceContext _context;

    public static string CultureKeyToken => "@";

    public DbStringLocalizerFactory(ResourceContext context)
    {
        _context = context;
        Reload();
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        return new DbStringLocalizer(_translations);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        return new DbStringLocalizer(_translations);
    }

    public void Reload()
    {
        var translationsTemp = new SortedList<string, string>();
        var translationList = _context.Translations.AsNoTracking().Include(trans => trans.Culture).Include(trans => trans.Resource).ToList();
        foreach (var translation in translationList)
        {
            var resName = translation.Resource.Name.StartsWith(ResourceCollector.SystemResourcePrefix)
                ? translation.Resource.Name.Substring(ResourceCollector.SystemResourcePrefix.Length)
                : translation.Resource.Name;
            translationsTemp.Add(translation.Culture.Name + CultureKeyToken + resName, translation.Value);
        }

        _translations = translationsTemp;
    }
}