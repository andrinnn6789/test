using System.Composition;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.CoreServer.Configuration;
using IAG.Infrastructure.CoreServer.Configuration.DataLayer;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Settings;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IAG.Infrastructure.Startup.CoreServer;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Export(typeof(IConfigureServices))]
public class ServerConfigureServices : IConfigureServices
{
    private const string CoreServer = "CoreServer";

    public void ConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        services.AddDbContext<CoreServerDataStoreDbContext>(services.GetDbOption(CoreServer, SettingsConst.SettingsDb));
        services.AddScoped<IConfigStore, ConfigStoreDb>();
        services.AddScoped<IPluginConfigStore, ConfigStoreDb>();
        services.AddSingleton<IPluginCatalogue, PluginCatalogue>();
        var builder = services.BuildServiceProviderForExplicitUser(SchemaMigrator.MigratorUser);
        using var scope = builder.CreateScope();
        var settingsDbContext = scope.ServiceProvider.GetRequiredService<CoreServerDataStoreDbContext>();
        new SchemaMigrator(scope.ServiceProvider).DoMigration(settingsDbContext);
    }
}