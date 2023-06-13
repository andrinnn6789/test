using System;
using System.Linq;
using System.Reflection;

namespace IAG.Infrastructure.DI;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PluginInfoAttribute : Attribute
{
    public Guid PluginId { get; }

    public string PluginName { get; }

    public PluginInfoAttribute(string pluginId, string pluginName)
    {
        PluginId = new Guid(pluginId);
        PluginName = pluginName;
    }

    public static PluginInfoAttribute GetPluginInfo(Type type)
    {
        var pluginInfoAttribute = type.GetCustomAttributes(typeof(PluginInfoAttribute)).FirstOrDefault() as PluginInfoAttribute;
        if (pluginInfoAttribute == null)
        {
            throw new System.Exception($"No plugin info attribute defined for {type}");
        }

        return pluginInfoAttribute;
    }

    public static IPluginMetadata GetPluginMeta(Type pluginType)
    {
        var pluginInfo = GetPluginInfo(pluginType);
        var meta = new PluginMetadata
        {
            PluginId = pluginInfo.PluginId,
            PluginName = pluginInfo.PluginName,
            PluginType = pluginType,
        };
        var loopType = pluginType;
        while (loopType != null)
        {
            var genArguments = loopType.GetGenericArguments();
            if (genArguments.Length > 0)
            {
                meta.PluginConfigType = genArguments[0];
                break;
            }

            loopType = loopType.BaseType;
        }

        return meta;
    }

    public static Guid GetPluginId(Type type)
    {
        return GetPluginInfo(type).PluginId;
    }

    public static string GetPluginName(Type type)
    {
        return GetPluginInfo(type).PluginName;
    }
}