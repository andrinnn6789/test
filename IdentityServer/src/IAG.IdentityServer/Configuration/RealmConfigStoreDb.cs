using System;
using System.Collections.Generic;
using System.Linq;

using IAG.IdentityServer.Configuration.DataLayer.Settings;
using IAG.IdentityServer.Configuration.Model.Realm;
using IAG.Infrastructure.Configuration.Macro;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Plugin;

using JetBrains.Annotations;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

namespace IAG.IdentityServer.Configuration;

[UsedImplicitly]
public class RealmConfigStoreDb : IRealmConfigStore
{
    private const string RealmPrefix = "RealmConfig.";

    private readonly IdentityDataStoreDbContext _context;
    private readonly IMacroReplacer _macroReplacer;

    public RealmConfigStoreDb(IdentityDataStoreDbContext context, IConfigurationRoot configuration)
    {
        _context = context;
        _macroReplacer = new MacroReplacer(new MacroValueSource(context, configuration));

    }

    public IEnumerable<IRealmConfig> GetAll()
    {
        return _context.ConfigEntries.
            Where(c => c.Name.StartsWith(RealmPrefix)).
            Select(c => JsonConvert.DeserializeObject<RealmConfig>(_macroReplacer.ReplaceMacros(c.Data)));
    }

    public IRealmConfig LoadConfig(IRealmConfig config, Type configType)
    {
        if (!(config is RealmConfig realmConfig))
        {
            return Read(config.Realm, configType);
        }

        config.AuthenticationPluginConfig = (IAuthenticationPluginConfig)realmConfig.AuthenticationPluginConfigJObject.ToObject(configType);

        return config;
    }

    public IRealmConfig Read(string realm, Type configType)
    {
        var item = _context.ConfigEntries.FirstOrDefault(t => t.Name == RealmPrefix + realm);
        if (item == null)
        {
            throw new NotFoundException(realm);
        }

        var data = _macroReplacer.ReplaceMacros(item.Data);

        IRealmConfig config = JsonConvert.DeserializeObject<RealmConfig>(data);
        config!.AuthenticationPluginConfig = (IAuthenticationPluginConfig)JsonConvert.DeserializeObject(data, configType);

        return config;
    }

    public void Insert(IRealmConfig config)
    {
        if (_context.ConfigEntries.FirstOrDefault(t => t.Name == RealmPrefix + config.Realm) != null)
        {
            throw new DuplicateKeyException(config.Realm);
        }

        _context.ConfigEntries.Add(new ConfigDb
        {
            Id = Guid.NewGuid(),
            Name = RealmPrefix + config.Realm,
            Data = JsonConvert.SerializeObject(config)
        });
        _context.SaveChanges();
    }

    public void Update(IRealmConfig config)
    {
        var item = _context.ConfigEntries.FirstOrDefault(t => t.Name == RealmPrefix + config.Realm);
        if (item == null)
        {
            throw new NotFoundException(config.Realm);
        }
            
        item.Data = JsonConvert.SerializeObject(config);
        _context.ConfigEntries.Update(item);
        _context.SaveChanges();
    }

    public void Delete(string realm)
    {
        var item = _context.ConfigEntries.FirstOrDefault(t => t.Name == RealmPrefix + realm);
        if (item == null)
        {
            throw new NotFoundException(realm);
        }

        _context.ConfigEntries.Remove(item);
        _context.SaveChanges();
    }
}