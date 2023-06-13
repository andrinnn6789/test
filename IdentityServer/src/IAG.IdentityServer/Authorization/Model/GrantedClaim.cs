using System.Security.Claims;

using IAG.Infrastructure.IdentityServer.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

using JetBrains.Annotations;

namespace IAG.IdentityServer.Authorization.Model;

[UsedImplicitly]
public class GrantedClaim
{
    public string ScopeName { get; set; }
    public string ClaimName { get; set; }
    public PermissionKind AllowedPermissions { get; set; }

    public Claim ToTokenClaim()
    {
        return new(ClaimTypes.Role, ClaimHelper.ToString(ScopeName, ClaimName, AllowedPermissions));
    }
}