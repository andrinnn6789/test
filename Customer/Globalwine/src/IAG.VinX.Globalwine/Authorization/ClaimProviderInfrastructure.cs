using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

using JetBrains.Annotations;

namespace IAG.VinX.Globalwine.Authorization;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
class ClaimProviderInfrastructure : ClaimProvider
{
    public ClaimProviderInfrastructure()
    {
        AddClaimDefinition(null, Scopes.ShopScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All);
    }
}