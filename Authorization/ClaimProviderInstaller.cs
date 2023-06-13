using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

using JetBrains.Annotations;

namespace IAG.InstallClient.Authorization;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
class ClaimProviderInstaller : ClaimProvider
{
    public ClaimProviderInstaller()
    {
        AddClaimDefinition(null, Scopes.InstallerScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All);
    }
}