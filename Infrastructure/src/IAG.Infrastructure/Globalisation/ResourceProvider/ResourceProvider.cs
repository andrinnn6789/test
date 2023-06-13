using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Resource;

namespace IAG.Infrastructure.Globalisation.ResourceProvider;

public abstract class ResourceProvider : IResourceProvider
{
    private readonly List<IResourceTemplate> _resTemplates = new();

    public IEnumerable<IResourceTemplate> ResourceTemplates => _resTemplates;

    public static string GetEnumResourceId(System.Enum enumValue)
    {
        return GetEnumResourceId(enumValue.GetType(), enumValue.ToString());
    }

    public void AddTemplate(string name, string language, string translation)
    {
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        if (_resTemplates.Any(t => t.Name == name && t.Language == language))
        {
            throw new LocalizableException(ResourceIds.ResourceTemplateDuplicateExceptionMessage, name, language);
        }

        _resTemplates.Add(new ResourceTemplate(name, language, translation));
    }

    public void AddEnumTemplates(Type enumType)
    {
        if (!typeof(System.Enum).IsAssignableFrom(enumType))
        {
            throw new ArgumentException("Not of type 'Enum'", nameof(enumType));
        }

        foreach (string enumName in System.Enum.GetNames(enumType))
        {
            _resTemplates.Add(new ResourceTemplate(GetEnumResourceId(enumType, enumName), "en", enumName));
        }
    }

    private static string GetEnumResourceId(Type enumType, string enumName)
    {
        return enumType.FullName + "." + enumName;
    }
}