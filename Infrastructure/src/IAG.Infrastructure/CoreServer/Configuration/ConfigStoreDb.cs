using System;
using System.Linq;

using IAG.Infrastructure.Configuration.Macro;
using IAG.Infrastructure.CoreServer.Configuration.DataLayer;
using IAG.Infrastructure.CoreServer.Plugin;
using IAG.Infrastructure.Exception.HttpException;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

namespace IAG.Infrastructure.CoreServer.Configuration;

public class ConfigStoreDb : IConfigStore, IPluginConfigStore
{
    private readonly CoreServerDataStoreDbContext _context;
    private readonly IMacroReplacer _macroReplacer;

    public ConfigStoreDb(CoreServerDataStoreDbContext context, IConfigurationRoot configurationRoot)
    {
        _context = context;
        _macroReplacer = new MacroReplacer(new MacroValueSource(context, configurationRoot));  // Cannot be injected since ConfigCommonStoreDbContext is unknown!
    }

    public void Delete(IPluginConfig config)
    {
        var item = _context.ConfigEntries.FirstOrDefault(t => t.Id == config.Id);
        if (item == null)
        {
            throw new NotFoundException(config.Id.ToString());
        }
            
        _context.ConfigEntries.Remove(item);
        _context.SaveChanges();
    }

    public void Insert<T>(IPluginConfig config)
        where T : IPluginConfig
    {
        if (_context.ConfigEntries.FirstOrDefault(t => t.Id == config.Id) != null)
        {
            throw new DuplicateKeyException(config.Id.ToString());
        }
            
        _context.ConfigEntries.Add(new ConfigDb
        {
            Id = config.Id,
            Name = config.PluginName,
            Data = JsonConvert.SerializeObject(config, typeof(T), null)
        });
        _context.SaveChanges();
    }

    public void Insert(IPluginConfig config, Type configType)
    {
        if (!configType.IsInstanceOfType(config))
        {
            throw new ArgumentException($"'{nameof(config)}' is not of type '{configType}'", nameof(config));
        }

        if (_context.ConfigEntries.FirstOrDefault(t => t.Id == config.Id) != null)
        {
            throw new DuplicateKeyException(config.Id.ToString());
        }

        _context.ConfigEntries.Add(new ConfigDb
        {
            Id = config.Id,
            Name = config.PluginName,
            Data = JsonConvert.SerializeObject(config, configType, null)
        });
        _context.SaveChanges();
    }

    public T Read<T>(Guid id)
        where T : IPluginConfig
    {
        var item = _context.ConfigEntries.FirstOrDefault(t => t.Id == id);
        if (item == null)
        {
            throw new NotFoundException(id.ToString());
        }

        var itemData = _macroReplacer.ReplaceMacros(item.Data);

        return JsonConvert.DeserializeObject<T>(itemData);
    }

    public IPluginConfig Read(Guid id, Type configType)
    {
        var item = _context.ConfigEntries.FirstOrDefault(t => t.Id == id);
        if (item == null)
        {
            throw new NotFoundException(id.ToString());
        }

        var itemData = _macroReplacer.ReplaceMacros(item.Data);

        return JsonConvert.DeserializeObject(itemData, configType) as IPluginConfig;
    }

    public void Update<T>(IPluginConfig config)
        where T : IPluginConfig
    {
        var item = _context.ConfigEntries.FirstOrDefault(t => t.Id == config.Id);
        if (item == null)
        {
            throw new NotFoundException(config.Id.ToString());
        }
            
        item.Data = JsonConvert.SerializeObject(config, typeof(T), null);
        _context.ConfigEntries.Update(item);
        _context.SaveChanges();
    }

    public void Update(IPluginConfig config, Type configType)
    {
        if (!configType.IsInstanceOfType(config))
        {
            throw new ArgumentException($"'{nameof(config)}' is not of type '{configType}'", nameof(config));
        }

        var item = _context.ConfigEntries.FirstOrDefault(t => t.Id == config.Id);
        if (item == null)
        {
            throw new NotFoundException(config.Id.ToString());
        }

        item.Data = JsonConvert.SerializeObject(config, configType, null);
        _context.ConfigEntries.Update(item);
        _context.SaveChanges();
    }

    public T Get<T>(Guid id)
        where T : class, IPluginConfig, new()
    {
        try
        {
            return Read<T>(id);
        }
        catch (NotFoundException)
        {
            var config = new T();
            Insert<T>(config);
            return config;
        }
    }

    public IPluginConfig Get(Guid id, Type configType)
    {
        if (!typeof(IPluginConfig).IsAssignableFrom(configType))
        {
            throw new ArgumentException("Is not assignable from 'IPluginConfig'", nameof(configType));
        }

        try
        {
            return Read(id, configType);
        }
        catch (NotFoundException)
        {
            var config = Activator.CreateInstance(configType) as IPluginConfig;

            if (config?.Id != id)
            {
                throw new ArgumentException($"ID of instance of '{configType}' is not equal to '{id}'", nameof(configType));
            }

            Insert(config, configType);
            return config;
        }
    }

    public void Write<T>(IPluginConfig config)
        where T : IPluginConfig
    {
        try
        {
            Update<T>(config);
        }
        catch (NotFoundException)
        {
            Insert<T>(config);
        }
    }

    public void Write(IPluginConfig config, Type configType)
    {
        try
        {
            Update(config, configType);
        }
        catch (NotFoundException)
        {
            Insert(config, configType);
        }
    }
}