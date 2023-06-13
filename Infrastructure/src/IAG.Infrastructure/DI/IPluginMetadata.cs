using System;

namespace IAG.Infrastructure.DI;

public interface IPluginMetadata
{
    /// <summary>
    /// Id of the plugin. Used to link with the different configs and logs
    /// </summary>
    Guid PluginId { get; }

    /// <summary>
    ///     Name of the plugin
    /// </summary>
    string PluginName { get; }

    /// <summary>
    /// Type of the plugin implementation
    /// </summary>
    Type PluginType { get; }

    /// <summary>
    /// Type of the plugin implementation
    /// </summary>
    Type PluginConfigType { get; }
}