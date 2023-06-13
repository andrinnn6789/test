using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using IAG.Infrastructure.IdentityServer.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

using Microsoft.AspNetCore.Authorization;

using Moq;

using Xunit;

namespace IAG.Infrastructure.Test.IdentityServer.Authorization.PolicyAuthorization;

public class ClaimAuthorizationHandlerTest
{
    [Fact]
    public async Task SimpleClaimAuthorizationHandlerTest()
    {
        var testScope1 = "TestScope1";
        var testScope2 = "TestScope2";
        var testClaim = "TestClaim";
        var testPermissions = PermissionKind.Read;
        var claimRequirement = new ClaimAuthorizationRequirement(new List<RequiredClaim>() {
            new(testScope1, testClaim, testPermissions),
            new(testScope2, testClaim, testPermissions)
        });

        var claimStringTestRead1 = ClaimHelper.ToString(testScope1, testClaim, PermissionKind.Read);
        var claimStringTestRead2 = ClaimHelper.ToString(testScope2, testClaim, PermissionKind.Read);
        var claimStringTestWrite = ClaimHelper.ToString(testScope1, testClaim, PermissionKind.Update);
        var claimStringOtherScope = ClaimHelper.ToString("OtherScope", testClaim, PermissionKind.Read);
        var claimStringOtherClaim = ClaimHelper.ToString(testScope1, "OtherClaim", PermissionKind.Read);

        var handler = new ClaimAuthorizationHandler();
        var contextRead1Ok = CreateContext(claimRequirement, claimStringTestRead1, true);
        var contextRead2Ok = CreateContext(claimRequirement, claimStringTestRead2, true);
        var contextInsufficientPermission = CreateContext(claimRequirement, claimStringTestWrite, true);
        var contextOtherScope = CreateContext(claimRequirement, claimStringOtherScope, true);
        var contextOtherClaim = CreateContext(claimRequirement, claimStringOtherClaim, true);
        var contextAnonymous = CreateContext(claimRequirement, claimStringTestRead1, false);

        await handler.HandleAsync(contextRead1Ok);
        await handler.HandleAsync(contextRead2Ok);
        await handler.HandleAsync(contextInsufficientPermission);
        await handler.HandleAsync(contextOtherScope);
        await handler.HandleAsync(contextOtherClaim);
        await handler.HandleAsync(contextAnonymous);

        Assert.True(contextRead1Ok.HasSucceeded);
        Assert.True(contextRead2Ok.HasSucceeded);
        Assert.False(contextInsufficientPermission.HasSucceeded);
        Assert.False(contextOtherScope.HasSucceeded);
        Assert.False(contextOtherClaim.HasSucceeded);
        Assert.False(contextAnonymous.HasSucceeded);
    }


    private AuthorizationHandlerContext CreateContext(IAuthorizationRequirement requirement, string claimString, bool authenticated)
    {
        var claims = new[] { new Claim(ClaimTypes.Role, claimString) };
        var identity = new ClaimsIdentity(claims, "HardCodedAuthenticated");
        var principalMock = new Mock<ClaimsPrincipal>();
        principalMock.Setup(m => m.Claims).Returns(() => claims);
        principalMock.Setup(m => m.Identity).Returns(() => authenticated ? identity : null);
        principalMock.Setup(m => m.Identities).Returns(() => authenticated ? new []{ identity } : Array.Empty<ClaimsIdentity>());

        return new AuthorizationHandlerContext(new [] { requirement }, principalMock.Object, null);
    }
}