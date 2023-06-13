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

public class NoAuthorizationHandlerTest
{
    [Fact]
    public async Task SimpleNoAuthorizationHandlerTest()
    {
        var claimRequirement = new ClaimAuthorizationRequirement(new List<RequiredClaim>()
        {
            new("DoesNot", "Matter", PermissionKind.None)
        });
            
        var claimString = ClaimHelper.ToString("StillNot", "Relevant", PermissionKind.Read);

        var handler = new NoAuthorizationHandler();
        var context = CreateContext(claimRequirement, claimString);

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    private AuthorizationHandlerContext CreateContext(IAuthorizationRequirement requirement, string claimString)
    {
        var principalMock = new Mock<ClaimsPrincipal>();
        principalMock.Setup(m => m.Claims).Returns(() => new []{ new Claim(ClaimTypes.Role, claimString) });

        return new AuthorizationHandlerContext(new [] { requirement }, principalMock.Object, null);
    }
}