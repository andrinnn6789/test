using System.Collections.Generic;

using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

using Xunit;

namespace IAG.Infrastructure.Test.IdentityServer.Authorization.PolicyAuthorization;

public class ClaimAuthorizationRequirementTest
{
    [Fact]
    public void SimpleClaimAuthorizationRequirementTest()
    {
        var testScope = "TestScope";
        var testClaim = "TestClaim";
        var testPermissions = PermissionKind.Read | PermissionKind.Update;

        var requirement = new ClaimAuthorizationRequirement(new List<RequiredClaim>() { new(testScope, testClaim, testPermissions) } );
        var requirementAsString = requirement.ToString();
        var requirementFromString = ClaimAuthorizationRequirement.FromString(requirementAsString);

        var requiredClaim = Assert.Single(requirement.RequiredClaims);
        var requiredClaimFromString = Assert.Single(requirementFromString.RequiredClaims);
        Assert.NotNull(requiredClaim);
        Assert.NotNull(requiredClaimFromString);
        Assert.Equal(testScope, requiredClaim.ScopeName);
        Assert.Equal(testClaim, requiredClaim.ClaimName);
        Assert.Equal(testPermissions, requiredClaim.RequiredPermissions);
        Assert.Equal(testScope, requiredClaimFromString.ScopeName);
        Assert.Equal(testClaim, requiredClaimFromString.ClaimName);
        Assert.Equal(testPermissions, requiredClaimFromString.RequiredPermissions);
    }

    [Fact]
    public void MultipleClaimAuthorizationRequirementTest()
    {
        var testScope = "TestScope";
        var testClaim = "TestClaim";
        var testPermissions = PermissionKind.Read | PermissionKind.Update;

        var requirement = new ClaimAuthorizationRequirement(new List<RequiredClaim>()
        {
            new(testScope, testClaim, testPermissions),
            new(testScope, testClaim, testPermissions)
        });
        var requirementAsString = requirement.ToString();
        var requirementFromString = ClaimAuthorizationRequirement.FromString(requirementAsString);

        Assert.NotEmpty(requirement.RequiredClaims);
        Assert.NotEmpty(requirementFromString.RequiredClaims);
        Assert.Equal(2, requirement.RequiredClaims.Count);
        Assert.Equal(2, requirementFromString.RequiredClaims.Count);
        Assert.All(requirement.RequiredClaims, c => Assert.Equal(testScope, c.ScopeName));
        Assert.All(requirement.RequiredClaims, c => Assert.Equal(testClaim, c.ClaimName));
        Assert.All(requirement.RequiredClaims, c => Assert.Equal(testPermissions, c.RequiredPermissions));
        Assert.All(requirementFromString.RequiredClaims, c => Assert.Equal(testScope, c.ScopeName));
        Assert.All(requirementFromString.RequiredClaims, c => Assert.Equal(testClaim, c.ClaimName));
        Assert.All(requirementFromString.RequiredClaims, c => Assert.Equal(testPermissions, c.RequiredPermissions));
    }
}