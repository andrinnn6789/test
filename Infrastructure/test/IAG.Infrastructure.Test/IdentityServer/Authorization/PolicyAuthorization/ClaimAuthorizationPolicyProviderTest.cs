using System;
using System.Threading.Tasks;

using IAG.Infrastructure.IdentityServer.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using Moq;

using Xunit;

namespace IAG.Infrastructure.Test.IdentityServer.Authorization.PolicyAuthorization;

public class ClaimAuthorizationPolicyProviderTest
{
    [Fact]
    public async Task SimpleClaimAuthorizationPolicyProviderTest()
    {
        var testScope = "TestScope";
        var testClaim = "TestClaim";
        var testPermissions = PermissionKind.Read | PermissionKind.Update;
        var claimString = ClaimHelper.ToString(testScope, testClaim, testPermissions);

        var optionsMock = new Mock<IOptions<AuthorizationOptions>>();
        optionsMock.Setup(m => m.Value).Returns(new AuthorizationOptions());
        var policyProvider = new ClaimAuthorizationPolicyProvider(optionsMock.Object);

        var claimPolicy = await policyProvider.GetPolicyAsync($"{ClaimAuthorizationAttribute.PolicyPrefix}{claimString}");
        var unknownPolicy = await policyProvider.GetPolicyAsync("UnknownPolicy");
        await policyProvider.GetDefaultPolicyAsync();
        await policyProvider.GetFallbackPolicyAsync();

        Assert.NotNull(claimPolicy);
        Assert.Null(unknownPolicy);
            
        var claimRequirement = Assert.IsType<ClaimAuthorizationRequirement>(Assert.Single(claimPolicy.Requirements));
        var requiredClaim = Assert.Single(claimRequirement.RequiredClaims);
        Assert.NotNull(requiredClaim);
        Assert.Equal(testScope, requiredClaim.ScopeName);
        Assert.Equal(testClaim, requiredClaim.ClaimName);
        Assert.Equal(testPermissions, requiredClaim.RequiredPermissions);
    }

    [Fact]
    public async Task MultipleClaimAuthorizationPolicyProviderTest()
    {
        var testScope = "TestScope";
        var testClaim = "TestClaim";
        var testPermissions = PermissionKind.Read | PermissionKind.Update;
        var claimString = ClaimHelper.ToString(testScope, testClaim, testPermissions);

        var optionsMock = new Mock<IOptions<AuthorizationOptions>>();
        optionsMock.Setup(m => m.Value).Returns(new AuthorizationOptions());
        var policyProvider = new ClaimAuthorizationPolicyProvider(optionsMock.Object);

        var claimPolicy = await policyProvider.GetPolicyAsync($"{ClaimAuthorizationAttribute.PolicyPrefix}{claimString}{Environment.NewLine}{claimString}");
        var unknownPolicy = await policyProvider.GetPolicyAsync("UnknownPolicy");
        await policyProvider.GetDefaultPolicyAsync();
        await policyProvider.GetFallbackPolicyAsync();

        Assert.NotNull(claimPolicy);
        Assert.Null(unknownPolicy);

        var claimRequirement = Assert.IsType<ClaimAuthorizationRequirement>(Assert.Single(claimPolicy.Requirements));
        Assert.NotEmpty(claimRequirement.RequiredClaims);
        Assert.Equal(2, claimRequirement.RequiredClaims.Count);
        Assert.All(claimRequirement.RequiredClaims, c => Assert.Equal(testScope, c.ScopeName));
        Assert.All(claimRequirement.RequiredClaims, c => Assert.Equal(testClaim, c.ClaimName));
        Assert.All(claimRequirement.RequiredClaims, c => Assert.Equal(testPermissions, c.RequiredPermissions));
    }
}