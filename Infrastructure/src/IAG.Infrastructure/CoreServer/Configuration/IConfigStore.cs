using System;

using IAG.Infrastructure.CoreServer.Plugin;

namespace IAG.Infrastructure.CoreServer.Configuration;

public interface IConfigStore
{
    void Update<T>(IPluginConfig config)
        where T : IPluginConfig;

    T Read<T>(Guid id)
        where T : IPluginConfig;

    void Delete(IPluginConfig config);

    void Insert<T>(IPluginConfig config)
        where T : IPluginConfig;
}