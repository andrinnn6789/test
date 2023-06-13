using System;
using System.Collections.Generic;

using IAG.Infrastructure.DI;

namespace IAG.Infrastructure.CoreServer.Configuration;

public interface IPluginCatalogue
{
    List<IPluginMetadata> Plugins { get; }

    IPluginMetadata GetPluginMeta(Guid pluginId);

    void Reload();
}