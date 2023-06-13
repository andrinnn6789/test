using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Globalisation.ResourceProvider;

using JetBrains.Annotations;

namespace IAG.PerformX.CampusSursee.Authorization;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class ResourceProviderScopesAndClaims : ResourceProvider
{
    public ResourceProviderScopesAndClaims()
    {
        // Scopes
        AddTemplate(Scopes.RestScope, "en", "Scope 'Webintegration' for Campus Sursee");
    }
}