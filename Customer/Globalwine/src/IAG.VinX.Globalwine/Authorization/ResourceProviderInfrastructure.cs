using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Globalisation.ResourceProvider;

using JetBrains.Annotations;

namespace IAG.VinX.Globalwine.Authorization;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class ResourceProviderScopesAndClaims : ResourceProvider
{
    public ResourceProviderScopesAndClaims()
    {
        // Scopes
        AddTemplate(Scopes.ShopScope, "en", "Scope 'Webintegration' for Globalwine, Shop with Next AG");
    }
}