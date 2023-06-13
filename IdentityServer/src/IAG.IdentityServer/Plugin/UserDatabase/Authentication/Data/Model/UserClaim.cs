using IAG.Infrastructure.IdentityServer.Authorization.Model;

namespace IAG.IdentityServer.Plugin.UserDatabase.Authentication.Data.Model;

public class UserClaim
{
    public string UserName { get; set; }
    public string ScopeName { get; set; }
    public string ClaimName { get; set; }
    public PermissionKind AllowedPermissions { get; set; }
}