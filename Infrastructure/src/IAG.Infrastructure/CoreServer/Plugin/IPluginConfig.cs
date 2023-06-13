using System;

namespace IAG.Infrastructure.CoreServer.Plugin;

public interface IPluginConfig
{
    /// <summary>
    ///     id of the config, preferably a descriptive id
    /// </summary>
    Guid Id { get; }

    string PluginName { get; }

    bool Active { get; }
}