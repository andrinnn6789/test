using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DI;

using Microsoft.Extensions.DependencyInjection;

namespace IAG.Infrastructure.CoreServer.Plugin;

[ExcludeFromCodeCoverage]
public abstract class CoreServerPlugin<TConfig> : ICoreServerPlugin
    where TConfig : class, IPluginConfig, new() 
{
    protected CoreServerPlugin()
    {
        Config = new TConfig();

        var jobInfoAttribute = PluginInfoAttribute.GetPluginInfo(GetType());
        PluginId = jobInfoAttribute.PluginId;
        Name = jobInfoAttribute.PluginName;
    }

    public Guid PluginId { get; }

    public string Name { get; }

    IPluginConfig ICoreServerPlugin.Config
    {
        get => Config;
        set => Config = value as TConfig;
    }

    public virtual void Init(IServiceCollection services)
    {
    }

    protected TConfig Config { get; set; }
}