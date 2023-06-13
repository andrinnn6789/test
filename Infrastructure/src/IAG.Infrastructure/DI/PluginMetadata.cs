using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.Infrastructure.DI;

[ExcludeFromCodeCoverage]
public class PluginMetadata : IPluginMetadata
{
    public Guid PluginId { get; set; }

    public string PluginName { get; set; }

    public Type PluginType { get; set; }

    public Type PluginConfigType { get; set; }
}