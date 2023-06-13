using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.IdentityServer.Configuration;
using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

namespace IAG.IdentityServer.Authorization;

public class ClaimPublisher : IClaimPublisher
{
    private readonly IPluginCatalogue _pluginCatalogue;
    private readonly IRealmCatalogue _realmCatalogue;

    public ClaimPublisher(IPluginCatalogue pluginCatalogue, IRealmCatalogue realmCatalogue)
    {
        _pluginCatalogue = pluginCatalogue;
        _realmCatalogue = realmCatalogue;
    }

    public Task PublishClaimsAsync(IEnumerable<ClaimDefinition> claimDefinitions)
    {
        var claimDefinitionsAsList = claimDefinitions.ToList();

        foreach (var realm in _realmCatalogue.Realms)
        {
            var plugin = _pluginCatalogue.GetAuthenticationPlugin(realm.AuthenticationPluginId);
            plugin.Config = realm.AuthenticationPluginConfig;
            if (plugin.Config.PublishClaims && plugin.Config.Active)
            {
                plugin.AddClaimDefinitions(claimDefinitionsAsList);
            }
        }

        return Task.CompletedTask;
    }
}