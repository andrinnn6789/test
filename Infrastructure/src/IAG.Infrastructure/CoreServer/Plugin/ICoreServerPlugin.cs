using System;

using Microsoft.Extensions.DependencyInjection;

namespace IAG.Infrastructure.CoreServer.Plugin;

public interface ICoreServerPlugin
{
    Guid PluginId { get; }

    string Name { get; }

    IPluginConfig Config { get; set; }

    void Init(IServiceCollection services);
}