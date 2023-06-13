using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

using JetBrains.Annotations;

namespace IAG.PerformX.ibW.Authorization;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
class ClaimProviderInfrastructure : ClaimProvider
{
    public ClaimProviderInfrastructure()
    {
        AddClaimDefinition(null, Scopes.RestScopeAzure, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All);
        AddClaimDefinition(null, Scopes.RestScopeWeb, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All);
    }
}