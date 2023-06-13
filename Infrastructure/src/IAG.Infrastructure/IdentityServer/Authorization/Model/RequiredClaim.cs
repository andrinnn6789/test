
namespace IAG.Infrastructure.IdentityServer.Authorization.Model;

public class RequiredClaim
{
    public string ScopeName { get; }
    public string ClaimName { get; }
    public PermissionKind RequiredPermissions { get; }

    public RequiredClaim(string scopeName, string claimName, PermissionKind requiredPermissions)
    {
        ScopeName = scopeName;
        ClaimName = claimName;
        RequiredPermissions = requiredPermissions;
    }
}