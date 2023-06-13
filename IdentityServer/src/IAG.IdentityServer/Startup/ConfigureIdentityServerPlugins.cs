using System;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using IAG.IdentityServer.Configuration;
using IAG.IdentityServer.Configuration.Model.Realm;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;
using IAG.Infrastructure.Startup.Extensions;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

namespace IAG.IdentityServer.Startup;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Export(typeof(IPluginConfigureServices))]
public class ConfigureIdentityServerPlugins : IPluginConfigureServices
{
    public void PluginConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        if (AuthenticationExtensions.IsAuthenticationDisabled(Infrastructure.Startup.Startup.BuildConfig()))
        {
            return;
        }

        using var scope = services.BuildServiceProvider().CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<ConfigureIdentityServerPlugins>>();

        try
        {
            var pluginCatalogue = scope.ServiceProvider.GetRequiredService<IPluginCatalogue>();
            var realmCatalogue = scope.ServiceProvider.GetRequiredService<IRealmCatalogue>();
            var reloadRealmCatalogue = false;

            // Init default realms
            foreach (var pluginMetadata in pluginCatalogue.Plugins)
            {
                var plugin = pluginCatalogue.GetAuthenticationPlugin(pluginMetadata);
                if (plugin == null)
                    throw new Exception($"cannot instantiate {pluginMetadata.PluginType.FullName}");

                if (realmCatalogue.Realms.Any(r => r.Realm == plugin.DefaultRealmName))
                    continue;

                var realmConfig = new RealmConfig
                {
                    Realm = plugin.DefaultRealmName,
                    AuthenticationPluginId = plugin.PluginId,
                    AuthenticationPluginConfig = plugin.Config,
                    AuthenticationPluginConfigJObject = JObject.FromObject(plugin.Config),
                    PasswordPolicy = new PasswordOptions()
                };
                realmCatalogue.Save(realmConfig);
                reloadRealmCatalogue = true;

            }

            if (reloadRealmCatalogue)
            {
                // load configuration with macro replacement
                realmCatalogue.Reload();
            }

            foreach (var realmConfig in realmCatalogue.Realms)
            {
                try
                {
                    var plugin = pluginCatalogue.GetAuthenticationPlugin(realmConfig.AuthenticationPluginId);
                    plugin.Config = realmConfig.AuthenticationPluginConfig;
                    plugin.Init(services.BuildServiceProvider().CreateScope().ServiceProvider);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to initialize authentication plugin for realm {realmConfig.Realm}: {ex.Message}", ex);
                }
            }

            // Collect and publish claims. Will only publish IdentityServer's claims if stand alone!
            scope.ServiceProvider.GetService<IClaimCollector>()?.CollectAndUpdateAsync().Wait();
        }
        catch (Exception ex)
        {
            logger?.LogCritical(ex, "Failed to init IdentityServer plugins");
            throw;
        }
    }
}