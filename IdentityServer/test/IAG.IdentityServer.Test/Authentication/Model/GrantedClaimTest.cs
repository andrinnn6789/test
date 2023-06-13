using System.Security.Claims;

using IAG.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

using Xunit;

namespace IAG.IdentityServer.Test.Authentication.Model;

public class GrantedClaimTest
{
    [Fact]
    public void ToTokenClaimTest()
    {
        var grantedClaim = new GrantedClaim() {ScopeName = "Scope", ClaimName = "Claim"};
        var grantedNone = grantedClaim.ToTokenClaim();
        grantedClaim.AllowedPermissions = PermissionKind.Read;
        var grantedRead = grantedClaim.ToTokenClaim();
        grantedClaim.AllowedPermissions = PermissionKind.Read | PermissionKind.Update;
        var grantedReadUpdate = grantedClaim.ToTokenClaim();
        grantedClaim.AllowedPermissions = PermissionKind.All;
        var grantedAll = grantedClaim.ToTokenClaim();

        Assert.Equal(ClaimTypes.Role, grantedNone.Type);
        Assert.Equal("None:Claim@Scope", grantedNone.Value);
        Assert.Equal("Read:Claim@Scope", grantedRead.Value);
        Assert.Equal("Read|Update:Claim@Scope", grantedReadUpdate.Value);
        Assert.Equal("Create|Read|Update|Delete|Execute:Claim@Scope", grantedAll.Value);
    }
}