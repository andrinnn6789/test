using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.CoreServer.Plugin;
using IAG.Infrastructure.DI;

namespace IAG.Infrastructure.CoreServer.Configuration;

public class PluginCatalogue : IPluginCatalogue
{
    private readonly IPluginLoader _pluginLoader;

    public PluginCatalogue(IPluginLoader pluginLoader)
    {
        _pluginLoader = pluginLoader;

        Plugins = new List<IPluginMetadata>();

        LoadCatalogue();
    }
        
    public List<IPluginMetadata> Plugins { get; }

    public IPluginMetadata GetPluginMeta(Guid pluginId)
    {
        return Plugins.FirstOrDefault(p => p.PluginId == pluginId);
    }

    public void Reload()
    {
        Plugins.Clear();
        LoadCatalogue();
    }

    private void LoadCatalogue()
    {
        foreach (var pluginType in _pluginLoader.GetImplementations<ICoreServerPlugin>())
        {
            Plugins.Add(PluginInfoAttribute.GetPluginMeta(pluginType));
        }
    }
}