using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

using Xunit;

namespace IAG.Infrastructure.Test.IdentityServer.Authorization.PolicyAuthorization;

public class ClaimAuthorizationAttributeTest
{
    [Fact]
    public void SimpleClaimAuthorizationAttributeTest()
    {
        var testScope = "TestScope";
        var testClaim = "TestClaim";
        var testPermissions = PermissionKind.Read | PermissionKind.Update;

        var attribute = new ClaimAuthorizationAttribute(testScope, testClaim, testPermissions);
        var requirement = attribute.Requirement;

        var requiredScope = Assert.Single(requirement.RequiredClaims);
        Assert.NotNull(requiredScope);
        Assert.Equal(testScope, requiredScope.ScopeName);
        Assert.Equal(testClaim, requiredScope.ClaimName);
        Assert.Equal(testPermissions, requiredScope.RequiredPermissions);
    }

    [Fact]
    public void MultipleClaimAuthorizationAttributeTest()
    {
        var testScope = "TestScope";
        var testClaim = "TestClaim";
        var testPermissions = PermissionKind.Read | PermissionKind.Update;

        var attribute1 = new ClaimAuthorizationAttribute(testScope, testClaim, testPermissions);
        var attribute2 = new ClaimAuthorizationAttribute(testScope, testClaim, testPermissions, testScope, testClaim, testPermissions);
        var attribute3 = new ClaimAuthorizationAttribute(testScope, testClaim, testPermissions, testScope, testClaim, testPermissions, testScope, testClaim, testPermissions);
        var attribute4 = new ClaimAuthorizationAttribute(testScope, testClaim, testPermissions, testScope, testClaim, testPermissions, testScope, testClaim, testPermissions, testScope, testClaim, testPermissions);

        Assert.Single(attribute1.Requirement.RequiredClaims);
        Assert.Equal(2, attribute2.Requirement.RequiredClaims.Count);
        Assert.Equal(3, attribute3.Requirement.RequiredClaims.Count);
        Assert.Equal(4, attribute4.Requirement.RequiredClaims.Count);
    }

    [Fact]
    public void WrongClaimAuthorizationAttributeTest()
    {
        var attribute = new ClaimAuthorizationAttribute(null, null, PermissionKind.None)
        {
            Policy = "Wrong Policy"
        };

        Assert.Null(attribute.Requirement);
    }
}