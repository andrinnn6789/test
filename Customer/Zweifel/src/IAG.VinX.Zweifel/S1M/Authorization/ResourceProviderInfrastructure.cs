using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Globalisation.ResourceProvider;

using JetBrains.Annotations;

namespace IAG.VinX.Zweifel.S1M.Authorization;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class ResourceProviderScopesAndClaims : ResourceProvider
{
    public ResourceProviderScopesAndClaims()
    {
        AddTemplate(Scopes.RestScope, "en", "Scope 'S1M Mobile API' for Zweifel Getraenke");
    }
}