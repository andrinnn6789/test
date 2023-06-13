using System;
using System.Linq;

using IAG.IdentityServer.Configuration;
using IAG.IdentityServer.Configuration.Model.Realm;
using IAG.IdentityServer.Resource;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json.Linq;

namespace IAG.IdentityServer.SeedImportExport;

public class RealmSeedImporterExporter : IRealmSeedImporterExporter
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly string SeedNameFormat = "Seed.Realm.{0}.json";

    private readonly IRealmCatalogue _realmCatalogue;
    private readonly IPluginCatalogue _pluginCatalogue;
    private readonly IUserContext _userContext;

    public string SeedFilePattern => SeedNameFormat.Replace("{0}", "*");

    public RealmSeedImporterExporter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _realmCatalogue = serviceProvider.GetRequiredService<IRealmCatalogue>();
        _pluginCatalogue = serviceProvider.GetRequiredService<IPluginCatalogue>();
        _userContext = serviceProvider.GetRequiredService<IUserContext>();
    }

    public object Export(string id, out string fileName)
    {
        if (_realmCatalogue.GetRealm(id) is not RealmConfig realmConfig)
        {
            throw new NotFoundException(ResourceIds.RealmNotFoundException, id);
        }

        var plugin = _pluginCatalogue.GetAuthenticationPlugin(realmConfig.AuthenticationPluginId);
        if (plugin == null)
        {
            throw new NotFoundException(ResourceIds.AuthenticationPluginNotFoundException, id);
        }

        plugin.Config = realmConfig.AuthenticationPluginConfig;
        var realmExport = new RealmImportExport()
        {
            RealmConfig = realmConfig,
            AuthenticationPluginData = plugin.GetExportData(_userContext)
        };

        fileName = string.Format(SeedNameFormat, id);

        return realmExport;
    }

    public void Import(JObject data)
    {
        ImportRealm(data.ToObject<RealmImportExport>());
    }

    public void ImportRealm(RealmImportExport importData)
    {
        importData.CheckType();

        bool newRealm = false;
        var realmConfig = _realmCatalogue.Realms.FirstOrDefault(r => r.Realm == importData.RealmConfig.Realm);
        if (realmConfig == null)
        {
            importData.RealmConfig.SetPluginConfig(_pluginCatalogue);
            _realmCatalogue.Save(importData.RealmConfig);
            newRealm = true;
            realmConfig = importData.RealmConfig;
        }

        var plugin = _pluginCatalogue.GetAuthenticationPlugin(realmConfig.AuthenticationPluginId);
        if (plugin == null)
        {
            throw new LocalizableException(ResourceIds.AuthenticationPluginNotFoundException, realmConfig.AuthenticationPluginId);
        }

        plugin.Config = realmConfig.AuthenticationPluginConfig;
        plugin.Init(_serviceProvider);
        if (newRealm)
        {
            // Collect and publish claims. Will only publish IdentityServer's claims if stand alone!
            _serviceProvider.GetRequiredService<IClaimCollector>().CollectAndUpdateAsync().Wait();
        }
        plugin.ImportData(importData.AuthenticationPluginData, _userContext);
    }
}