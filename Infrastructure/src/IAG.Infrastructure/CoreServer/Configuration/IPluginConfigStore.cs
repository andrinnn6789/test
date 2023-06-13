using System;

using IAG.Infrastructure.CoreServer.Plugin;

namespace IAG.Infrastructure.CoreServer.Configuration;

public interface IPluginConfigStore
{
    T Get<T>(Guid id)
        where T : class, IPluginConfig, new();

    IPluginConfig Get(Guid id, Type configType);

    void Write<T>(IPluginConfig config)
        where T : IPluginConfig;

    void Write(IPluginConfig config, Type configType);
}