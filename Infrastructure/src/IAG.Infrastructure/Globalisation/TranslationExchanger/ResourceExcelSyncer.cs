using System;
using System.Linq;

using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Rest;

using Microsoft.AspNetCore.Mvc;

namespace IAG.Infrastructure.Globalisation.TranslationExchanger;

public class ResourceExcelSyncer
{
    private readonly ResourceContext _resContext;

    public ResourceExcelSyncer(ResourceContext resContext)
    {
        _resContext = resContext;
    }

    public FileContentResult DownloadResources()
    {
        var query = _resContext.Translations
            .Select(

                s => new TranslationView
                {
                    Id = s.Id,
                    Translation = s.Value,
                    CultureName = s.Culture.Name,
                    ResourceName = s.Resource.Name
                }
            ).ToList();
        return new FileContentResult(
            new TranslationViewToExcel().BuildExcel(query.ToArray()),
            ContentTypes.ApplicationVndMsExcel) {FileDownloadName = "Resourcen.xlsx"};
    }

    public void UploadResources(byte[] data)
    {
        var translationViews = new TranslationViewFromExcel().GetTranslations(data);
        foreach (var cultureView in new TranslationExcelStructure().ExtractCultures(translationViews.ToArray())
                     .Where(cultureView => _resContext.Cultures.FirstOrDefault(r => r.Name == cultureView) == null))
        {
            _resContext.Cultures.Add(new Culture {Name = cultureView});
        }

        foreach (var resourceView in new TranslationExcelStructure().ExtractResources(translationViews.ToArray())
                     .Where(resourceView => _resContext.Resources.FirstOrDefault(r => r.Name == resourceView) == null))
        {
            _resContext.Resources.Add(new Model.Resource {Name = resourceView});
        }

        _resContext.SaveChanges();

        foreach (var transViewExcel in translationViews)
        {
            var transContext = _resContext.Translations.FirstOrDefault(t => t.Id == transViewExcel.Id);
            if (transContext == null)
            {
                _resContext.Translations.Add(new Translation
                {
                    Id = Guid.NewGuid(),
                    Culture = _resContext.Cultures.First(c => c.Name == transViewExcel.CultureName),
                    Resource = _resContext.Resources.First(c => c.Name == transViewExcel.ResourceName),
                    Value = transViewExcel.Translation
                });
                continue;
            }

            if (transContext.Culture.Name == transViewExcel.CultureName &&
                transContext.Resource.Name == transViewExcel.ResourceName &&
                transContext.Value == transViewExcel.Translation)
                continue;
            transContext.Value = transViewExcel.Translation;
        }

        _resContext.SaveChanges();
    }
}