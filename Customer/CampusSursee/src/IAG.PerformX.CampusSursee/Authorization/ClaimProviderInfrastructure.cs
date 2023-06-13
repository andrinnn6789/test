using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

using JetBrains.Annotations;

namespace IAG.PerformX.CampusSursee.Authorization;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
class ClaimProviderInfrastructure : ClaimProvider
{
    public ClaimProviderInfrastructure()
    {
        AddClaimDefinition(null, Scopes.RestScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All);
    }
}