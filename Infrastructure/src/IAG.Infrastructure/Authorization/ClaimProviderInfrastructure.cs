using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Authorization;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
class ClaimProviderInfrastructure : ClaimProvider
{
    public ClaimProviderInfrastructure()
    {
        AddClaimDefinition(null, ScopeNamesInfrastructure.SystemScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All);
        AddClaimDefinition(null, ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All);
        AddClaimDefinition(null, ScopeNamesInfrastructure.ReaderScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read);
        AddClaimDefinition(null, ScopeNamesInfrastructure.AtlasScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All);
        AddClaimDefinition(null, ScopeNamesInfrastructure.ProcessEngine, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All);
    }
}