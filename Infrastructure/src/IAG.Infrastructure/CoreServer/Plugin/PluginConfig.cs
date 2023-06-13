using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DI;

namespace IAG.Infrastructure.CoreServer.Plugin;

[ExcludeFromCodeCoverage]
public class PluginConfig<TPlugin> : IPluginConfig
    where TPlugin: ICoreServerPlugin
{
    public Guid Id { get; }

    public string PluginName { get; }
    public bool Active { get; set; }

    protected PluginConfig()
    {
        var pluginInfo = PluginInfoAttribute.GetPluginInfo(typeof(TPlugin));
        Id = pluginInfo.PluginId;
        PluginName = pluginInfo.PluginName;
        Active = true;
    }
}