using System;
using System.Collections.Generic;

using IAG.Infrastructure.DI;
using IAG.Infrastructure.IdentityServer.Plugin;

namespace IAG.IdentityServer.Configuration;

public interface IPluginCatalogue
{
    List<IPluginMetadata> Plugins { get; }

    IPluginMetadata GetPluginMeta(Guid pluginId);

    IAuthenticationPlugin GetAuthenticationPlugin(Guid pluginId);

    IAuthenticationPlugin GetAuthenticationPlugin(IPluginMetadata pluginMetadata);

    void Reload();
}