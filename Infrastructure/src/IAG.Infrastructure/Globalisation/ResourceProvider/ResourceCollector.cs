using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using IAG.Infrastructure.DI;
using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Logging;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IAG.Infrastructure.Globalisation.ResourceProvider;

public class ResourceCollector : IResourceCollector
{
    public const string SystemResourcePrefix = "System.";

    private readonly IPluginLoader _pluginLoader;
    private readonly ILogger<ResourceCollector> _logger;
    private Dictionary<string, Model.Resource> _resourcesInDb;
    private Dictionary<string, Culture> _culturesInDb;
    private Dictionary<string, Translation> _translationsInDb;

    public ResourceCollector(ResourceContext context, IPluginLoader pluginLoader, ILogger<ResourceCollector> logger)
    {
        Context = context;
        _pluginLoader = pluginLoader;
        _logger = logger;
    }

    public ResourceContext Context { get; }

    public void CollectAndUpdate()
    {
        try
        {
            var collectedTemplates = new List<IResourceTemplate>();

            foreach (var provider in _pluginLoader.GetImplementations<IResourceProvider>()
                         .Select(ActivatorWithCheck.CreateInstance<IResourceProvider>))
            {
                collectedTemplates.AddRange(provider.ResourceTemplates.Select(t =>
                    new ResourceTemplate(SystemResourcePrefix + t.Name, t.Language, t.Translation)));
            }

            SyncTemplatesToResources(SystemResourcePrefix, collectedTemplates);
        }
        catch (System.Exception e)
        {
            new ErrorLogger().LogException(_logger, e);
        }
    }

    public void SyncTemplatesToResources(string prefix, IList<IResourceTemplate> templates)
    {
        if (Context.Database.IsInMemory())
            return;

        LoadResourcesFromDb(prefix);

        foreach (var resource in templates)
        {
            CreateOrUpdateTranslation(resource);
        }

        RemoveOrphanedResources(templates);
        Context.SaveChanges();
    }

    private void LoadResourcesFromDb(string prefix)
    {
        _resourcesInDb = new Dictionary<string, Model.Resource>
        (
            Context
                .Resources
                .Where(r => r.Name.StartsWith(prefix))
                .AsNoTracking()
                .ToDictionary(res => res.Name, res => res)
        );
        _culturesInDb = new Dictionary<string, Culture>
        (
            Context
                .Cultures
                .AsNoTracking()
                .ToDictionary(c => c.Name, c => c)
        );
        _translationsInDb = new Dictionary<string, Translation>
        (
            Context
                .Translations
                .Include(trans => trans.Culture)
                .Include(trans => trans.Resource)
                .Where(r => r.Resource.Name.StartsWith(prefix))
                .ToDictionary(tr => tr.Culture.Name + tr.Resource.Name, tr => tr)
        );
    }

    private void CreateOrUpdateTranslation(IResourceTemplate resource)
    {
        var currentResource = GetCurrentResource(resource);
        var translationKey = resource.Language + resource.Name;

        if (_translationsInDb.ContainsKey(translationKey))
        {
            if (_translationsInDb[translationKey].Value != resource.Translation)
                _translationsInDb[translationKey].Value = resource.Translation;
        }
        else
        {
            if (!AssureCulture(resource))
                return;

            var translation = new Translation
            {
                ResourceId = currentResource.Id,
                CultureId = _culturesInDb[resource.Language].Id,
                Value = resource.Translation
            };
            Context.Translations.Add(translation);
            _translationsInDb.Add(translationKey, translation);
        }
    }

    private void RemoveOrphanedResources(IEnumerable<IResourceTemplate> resourceTemplates)
    {
        var orphanKeys = _resourcesInDb.Keys.Except(resourceTemplates.Select(t => t.Name));
        var orphans = Context.Resources.Where(r => orphanKeys.Contains(r.Name));
        Context.Resources.RemoveRange(orphans);
    }

    private Model.Resource GetCurrentResource(IResourceTemplate resource)
    {
        Model.Resource currentResource;
        if (_resourcesInDb.ContainsKey(resource.Name))
        {
            currentResource = _resourcesInDb[resource.Name];
        }
        else
        {
            currentResource = new Model.Resource
            {
                Id = Guid.NewGuid(),
                Name = resource.Name
            };
            Context.Resources.Add(currentResource);
            _resourcesInDb.Add(currentResource.Name, currentResource);
        }

        return currentResource;
    }

    private bool AssureCulture(IResourceTemplate resource)
    {
        if (_culturesInDb.ContainsKey(resource.Language))
            return true;

        if (CultureInfo.GetCultures(CultureTypes.AllCultures).All(c => c.Name != resource.Language))
        {
            _logger.LogWarning($"Invalid culture {resource.Language} received in {resource.Name}");
            return false;
        }

        var culture = new Culture
        {
            Id = Guid.NewGuid(),
            Name = resource.Language
        };
        Context.Cultures.Add(culture);
        _culturesInDb.Add(culture.Name, culture);
        Context.SaveChanges();
        return true;
    }
}