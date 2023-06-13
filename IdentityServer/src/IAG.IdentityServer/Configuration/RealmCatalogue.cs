using System.Collections.Generic;
using System.Linq;

using IAG.IdentityServer.Configuration.Model.Realm;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Exception.HttpException;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace IAG.IdentityServer.Configuration;

[UsedImplicitly]
public class RealmCatalogue : IRealmCatalogue
{
    private readonly IRealmConfigStore _configStore;
    private readonly IPluginCatalogue _pluginCatalogue;
    private readonly ILogger<RealmCatalogue> _logger;

    private readonly Dictionary<string, IRealmConfig> _realms;

    public RealmCatalogue(IRealmConfigStore configStore, IPluginCatalogue pluginCatalogue, ILogger<RealmCatalogue> logger)
    {
        _configStore = configStore;
        _pluginCatalogue = pluginCatalogue;
        _logger = logger;

        _realms = new Dictionary<string, IRealmConfig>();
        Load();
    }

    public List<IRealmConfig> Realms => _realms.Values.ToList();

    public void Reload()
    {
        _pluginCatalogue.Reload();
        _realms.Clear();
        Load();
    }

    public IRealmConfig GetRealm(string realm)
    {
        return _realms.ContainsKey(realm) ? _realms[realm] : null;
    }

    public void Save(IRealmConfig config)
    {
        try
        {
            _configStore.Update(config);
        }
        catch (NotFoundException)
        {
            _configStore.Insert(config);
        }

        _realms[config.Realm] = config;
    }

    public void Delete(string realm)
    {
        _configStore.Delete(realm);
        _realms.Remove(realm);
    }

    private void Load()
    {
        foreach (IRealmConfig realmConfig in _configStore.GetAll())
        {
            IPluginMetadata authenticationPlugin = _pluginCatalogue.GetPluginMeta(realmConfig.AuthenticationPluginId);
            if (authenticationPlugin == null)
            {
                var errorMessage = $"Authentication plugin '{realmConfig.AuthenticationPluginId}' for realm '{realmConfig.Realm}' not found";
                _logger.LogError(errorMessage);
                _realms[realmConfig.Realm] = new FaultyRealmConfig()
                {
                    Realm = realmConfig.Realm,
                    AuthenticationPluginId = realmConfig.AuthenticationPluginId,
                    ErrorMessage = errorMessage
                };
            }
            else
            {
                _realms[realmConfig.Realm] = _configStore.LoadConfig(realmConfig, authenticationPlugin.PluginConfigType);
            }
        }
    }
}