using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.DI;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.Resource;

namespace IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;

public class ClaimCollector : IClaimCollector
{
    private readonly IPluginLoader _pluginLoader;
    private readonly IClaimPublisher _claimPublisher;

    public ClaimCollector(IPluginLoader pluginLoader, IClaimPublisher claimPublisher)
    {
        _pluginLoader = pluginLoader;
        _claimPublisher = claimPublisher;
    }
        
    public async Task CollectAndUpdateAsync()
    {
        var collectedClaims = new List<ClaimDefinition>();
        var duplicateCheck = new Dictionary<string, Type>();

        foreach (var providerType in _pluginLoader.GetImplementations<IClaimProvider>().ToList())
        {
            var provider = ActivatorWithCheck.CreateInstance<IClaimProvider>(providerType);

            foreach (var claimDefinition in provider.ClaimDefinitions)
            {
                var duplicateKey = $"{claimDefinition.ClaimName}@{claimDefinition.ScopeName}|{claimDefinition.TenantId}";
                if (duplicateCheck.TryGetValue(duplicateKey, out var previousProviderType))
                {
                    throw new LocalizableException(ResourceIds.ClaimProviderDuplicateExceptionMessage, claimDefinition.ClaimName, claimDefinition.ScopeName, claimDefinition.TenantId, providerType.FullName, previousProviderType.FullName);
                }

                collectedClaims.Add(claimDefinition);
                duplicateCheck.Add(duplicateKey, providerType);
            }
        }

        await _claimPublisher.PublishClaimsAsync(collectedClaims);
    }
}