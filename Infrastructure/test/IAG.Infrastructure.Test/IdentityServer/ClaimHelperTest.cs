using IAG.Infrastructure.IdentityServer.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

using Xunit;

namespace IAG.Infrastructure.Test.IdentityServer;

public class ClaimHelperTest
{
    [Fact]
    public void SimpleClaimHelperTest()
    {
        var testScope = "TestScope";
        var testClaim = "TestClaim";
        var testPermissions = PermissionKind.Read | PermissionKind.Update;

        var claimStringWithScope = ClaimHelper.ToString(testScope, testClaim, testPermissions);
        var claimStringWithoutScope = ClaimHelper.ToString(null, testClaim, PermissionKind.None);
        ClaimHelper.FromString(claimStringWithScope, out string outScope, out string outClaim, out PermissionKind outPermissions);
        ClaimHelper.FromString(claimStringWithoutScope, out string outScopeEmpty, out string outClaimWithoutScope, out PermissionKind outPermissionsWithoutScope);

        Assert.Equal(testScope, outScope);
        Assert.Equal(testClaim, outClaim);
        Assert.Empty(outScopeEmpty);
        Assert.Equal(testClaim, outClaimWithoutScope);
        Assert.Equal(testPermissions, outPermissions);
        Assert.Equal(PermissionKind.None, outPermissionsWithoutScope);
        Assert.True(ClaimHelper.MatchesClaimAndScope(claimStringWithScope, testScope, testClaim));
        Assert.False(ClaimHelper.MatchesClaimAndScope(claimStringWithScope, "AnotherScope", testClaim));
        Assert.False(ClaimHelper.MatchesClaimAndScope(claimStringWithScope, testScope, "AnotherClaim"));
        Assert.False(ClaimHelper.MatchesClaimAndScope(claimStringWithScope, "AnotherScope", "AnotherClaim"));
    }

    [Fact]
    public void MatchesClaimAndScopeTest()
    {
        var testScope = "TestScope";
        var testClaim = "TestClaim";
        var testPermissions = PermissionKind.Read | PermissionKind.Update;

        var claimString = ClaimHelper.ToString(testScope, testClaim, testPermissions);

        Assert.True(ClaimHelper.MatchesClaimAndScope(claimString, testScope, testClaim));
        Assert.False(ClaimHelper.MatchesClaimAndScope(claimString, "AnotherScope", testClaim));
        Assert.False(ClaimHelper.MatchesClaimAndScope(claimString, testScope, "AnotherClaim"));
        Assert.False(ClaimHelper.MatchesClaimAndScope(claimString, "AnotherScope", "AnotherClaim"));
    }

    [Fact]
    public void GetPermissionsTest()
    {
        var testScope = "TestScope";
        var testClaim = "TestClaim";
        var testPermissions = PermissionKind.Read | PermissionKind.Update;

        var claimStringWithScope = ClaimHelper.ToString(testScope, testClaim, testPermissions);
        var claimStringWithoutScope = ClaimHelper.ToString(null, testClaim, PermissionKind.None);

        var permissionsReadWrite = ClaimHelper.GetPermissions(claimStringWithScope);
        var permissionsNone = ClaimHelper.GetPermissions(claimStringWithoutScope);
        var permissionsEmpty = ClaimHelper.GetPermissions(string.Empty);

        Assert.Equal(testPermissions, permissionsReadWrite);
        Assert.Equal(PermissionKind.None, permissionsNone);
        Assert.Equal(PermissionKind.None, permissionsEmpty);
    }
}