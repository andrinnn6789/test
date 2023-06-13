using System;
using System.Composition;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.CoreServer.Configuration;
using IAG.Infrastructure.CoreServer.Plugin;
using IAG.Infrastructure.DI;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IAG.Infrastructure.Startup.CoreServer;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Export(typeof(IPluginConfigureServices))]
public class ServerPluginConfigureServices : IPluginConfigureServices
{
    private IServiceCollection _services;
    private ILogger _logger;

    public void PluginConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        _services = services;
        var builder = _services.BuildServiceProvider();
        using (var scope = builder.CreateScope())
        {
            _logger = scope.ServiceProvider.GetService<ILogger<ServerConfigureServices>>();
            try
            {
                RegisterPlugins();
                InitPluginConfigs();
            }
            catch (System.Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ForegroundColor = ConsoleColor.White;
                _logger?.LogError(e, "Init config-store core");
            }
        }
    }

    private void RegisterPlugins()
    {
        var builder = _services.BuildServiceProvider();
        using (var scope = builder.CreateScope())
        {
            var pluginCatalogue = scope.ServiceProvider.GetRequiredService<IPluginCatalogue>();

            foreach (IPluginMetadata pluginMetadata in pluginCatalogue.Plugins)
            {
                var configStore = scope.ServiceProvider.GetRequiredService<IPluginConfigStore>();
                var config = configStore.Get(pluginMetadata.PluginId, pluginMetadata.PluginConfigType);
                if (!config.Active)
                {
                    _logger.LogInformation($"Core-plugin {pluginMetadata.PluginName} is disabled");

                }
                else
                {
                    _logger.LogInformation($"Registering core-plugin {pluginMetadata.PluginName}");
                    _services.AddScoped(pluginMetadata.PluginConfigType,
                        sp =>
                        {
                            var scopedConfigStore = sp.GetRequiredService<IPluginConfigStore>();
                            return scopedConfigStore.Get(pluginMetadata.PluginId, pluginMetadata.PluginConfigType);
                        });
                }
            }
        }
    }

    private void InitPluginConfigs()
    {
        using var scope = _services.BuildServiceProvider().CreateScope();
        var pluginCatalogue = scope.ServiceProvider.GetRequiredService<IPluginCatalogue>();
        var configStore = scope.ServiceProvider.GetRequiredService<IPluginConfigStore>();

        foreach (IPluginMetadata pluginMetadata in pluginCatalogue.Plugins)
        {
            var config = configStore.Get(pluginMetadata.PluginId, pluginMetadata.PluginConfigType);
            if (!config.Active)
            {
                continue;
            }

            var plugin = (ICoreServerPlugin)Activator.CreateInstance(pluginMetadata.PluginType);
            if (plugin == null)
                throw new System.Exception($"cannot instantiate {pluginMetadata.PluginType.FullName}");

            plugin.Init(_services);
        }
    }
}