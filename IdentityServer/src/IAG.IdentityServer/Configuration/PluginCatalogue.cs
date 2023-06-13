using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.DI;
using IAG.Infrastructure.IdentityServer.Plugin;

using Microsoft.Extensions.DependencyInjection;

namespace IAG.IdentityServer.Configuration;

public class PluginCatalogue : IPluginCatalogue
{
    private readonly IPluginLoader _pluginLoader;
    private readonly IServiceProvider _serviceProvider;

    public PluginCatalogue(IPluginLoader pluginLoader, IServiceProvider serviceProvider)
    {
        _pluginLoader = pluginLoader;
        _serviceProvider = serviceProvider;

        Plugins = new List<IPluginMetadata>();

        LoadCatalogue();
    }
        
    public List<IPluginMetadata> Plugins { get; }

    public IPluginMetadata GetPluginMeta(Guid pluginId)
    {
        return Plugins.FirstOrDefault(p => p.PluginId == pluginId);
    }

    public IAuthenticationPlugin GetAuthenticationPlugin(Guid pluginId)
    {
        return GetAuthenticationPlugin(GetPluginMeta(pluginId));
    }

    public IAuthenticationPlugin GetAuthenticationPlugin(IPluginMetadata pluginMetadata)
    {
        if (pluginMetadata?.PluginType == null)
        {
            return null;
        }

        return (IAuthenticationPlugin)_serviceProvider.GetRequiredService(pluginMetadata.PluginType);
    }

    public void Reload()
    {
        Plugins.Clear();
        LoadCatalogue();
    }

    private void LoadCatalogue()
    {
        foreach (var pluginType in _pluginLoader.GetImplementations<IAuthenticationPlugin>())
        {
            Plugins.Add(PluginInfoAttribute.GetPluginMeta(pluginType));
        }
    }
}