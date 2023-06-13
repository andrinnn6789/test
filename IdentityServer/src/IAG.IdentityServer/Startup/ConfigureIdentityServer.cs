using System;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using IAG.IdentityServer.Authentication;
using IAG.IdentityServer.Authorization;
using IAG.IdentityServer.Configuration;
using IAG.IdentityServer.Configuration.DataLayer.Settings;
using IAG.IdentityServer.Configuration.DataLayer.State;
using IAG.IdentityServer.Configuration.Model.Config;
using IAG.IdentityServer.Mail;
using IAG.IdentityServer.Security;
using IAG.IdentityServer.SeedImportExport;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Configuration.Macro;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.IdentityServer;
using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;
using IAG.Infrastructure.ImportExport;
using IAG.Infrastructure.Settings;

using JetBrains.Annotations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace IAG.IdentityServer.Startup;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Export(typeof(IConfigureServices))]
public class ConfigureIdentityServer : IConfigureServices
{
    private const string IdentityServer = "IdentityServer";
    private const string IdentityServerState = "IdentityServer.State";
    private const string CertificateName = "IAG-IdentityServer Certificate";
    private const string CertificateFileName = "SigningCertificate.pfx";
    private const string CertificatePassword = "kRY!ZWOxtwIs.AhcAYYLJwS-0gDCwsZ4fS*IJyaELnXc";
    private const StoreName CertificateStore = StoreName.Root;
    private const StoreLocation CertificateLocation = StoreLocation.LocalMachine;
    private ILogger<ConfigureIdentityServer> _logger;

    public void ConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        services.AddDbContext<IdentityDataStoreDbContext>(services.GetDbOption(IdentityServer, SettingsConst.SettingsDb));
        services.AddDbContext<IdentityStateDbContext>(services.GetDbOption(IdentityServerState, $"{IdentityServerState}.db"));
        services.AddScoped<IRealmConfigStore, RealmConfigStoreDb>();
        services.AddSingleton<ITokenHandler, TokenHandler>();
        services.AddScoped<IPasswordGenerator, PasswordGenerator>();
        services.AddScoped<IPasswordChecker, PasswordChecker>();
        services.AddScoped<IAttackDetection, AttackDetection>();
        services.AddSingleton<IPluginCatalogue, PluginCatalogue>();
        services.AddSingleton<IRealmCatalogue, RealmCatalogue>();
        services.AddScoped<IMailSender, SmtpMailSender>();
        services.AddScoped<ITemplateHandler, TemplateHandler>();
        services.AddScoped<IClaimPublisher, ClaimPublisher>();
        services.AddSingleton<ICertificateManager, CertificateManager>();
        services.AddScoped<IRealmSeedImporterExporter, RealmSeedImporterExporter>();
        services.AddScoped<ISeedImporter, RealmSeedImporterExporter>();
        services.AddScoped<IRealmHandler, RealmHandler>();
        services.AddScoped<IRefreshTokenManager, RefreshTokenManager>();

        var builder = services.BuildServiceProviderForExplicitUser(SchemaMigrator.MigratorUser);
        using (var scope = builder.CreateScope())
        {
            var configDbContext = scope.ServiceProvider.GetRequiredService<IdentityDataStoreDbContext>();
            var stateDbContext = scope.ServiceProvider.GetRequiredService<IdentityStateDbContext>();
            var migrator = new SchemaMigrator(scope.ServiceProvider);
            migrator.DoMigration(configDbContext);
            migrator.DoMigration(stateDbContext);
        }

        builder = services.BuildServiceProvider();
        using (var scope = builder.CreateScope())
        {
            var configDbContext = scope.ServiceProvider.GetRequiredService<IdentityDataStoreDbContext>();
            var configRoot = scope.ServiceProvider.GetRequiredService<IConfigurationRoot>();
            var macroReplacer = new MacroReplacer(new MacroValueSource(configDbContext, configRoot));

            _logger = scope.ServiceProvider.GetService<ILogger<ConfigureIdentityServer>>();
            services.AddSingleton<ITokenGenerationConfig>(
                GetConfig<TokenGenerationConfig>(configDbContext, macroReplacer, TokenGenerationConfig.ConfigName));
            services.AddSingleton<IAttackDetectionConfig>(
                GetConfig<AttackDetectionConfig>(configDbContext, macroReplacer, AttackDetectionConfig.ConfigName));
            services.AddSingleton<ISmtpConfig>(
                GetConfig<SmtpConfig>(configDbContext, macroReplacer, SmtpConfig.ConfigName));
            services.AddSingleton<IRefreshTokenConfig>(
                GetConfig<RefreshTokenConfig>(configDbContext, macroReplacer, RefreshTokenConfig.ConfigName));
            services.AddSingleton(GetCertificateConfig(configRoot));

            var pluginCatalogue = scope.ServiceProvider.GetRequiredService<IPluginCatalogue>();
            foreach (var jobMeta in pluginCatalogue.Plugins)
            {
                _logger.LogInformation("Registering identity-plugin " + jobMeta.PluginName);
                services.AddTransient(jobMeta.PluginType);
            }
        }
    }

    private T GetConfig<T>(IdentityDataStoreDbContext dbContext, IMacroReplacer macroReplacer, string name) where T : new()
    {
        string data;
        try
        {
            data = dbContext.ConfigEntries.Any()
                ? dbContext.ConfigEntries.Single(c => c.Name == name).Data
                : JsonConvert.SerializeObject(new T());
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Failed to init IdentityServer config: {name}");
            data = JsonConvert.SerializeObject(new T());
        }
        data = macroReplacer.ReplaceMacros(data);
        return JsonConvert.DeserializeObject<T>(data);
    }

    private ICertificateManagerConfig GetCertificateConfig(IConfigurationRoot configRoot)
    {
        var storeName = configRoot["Kestrel:Certificates:Default:Store"];
        var storeLocation = configRoot["Kestrel:Certificates:Default:Location"];
        var certificateConfig = new CertificateManagerConfig
        {
            CertificateName = configRoot["Kestrel:Certificates:Default:Subject"]?? CertificateName,
            CertificateFileName = configRoot["Kestrel:Certificates:Default:Path"]?? 
                                  Path.Combine(new SettingsFinder().GetSettingsPath(), CertificateFileName),
            CertificatePassword = configRoot["Kestrel:Certificates:Default:Password"]?? CertificatePassword,
            StoreName = storeName == null ? CertificateStore : Enum.Parse<StoreName>(storeName),
            StoreLocation = storeName == null ? CertificateLocation : Enum.Parse<StoreLocation>(storeLocation)
        };
        return certificateConfig;
    }
}