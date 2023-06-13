using System;
using System.Security.Claims;

using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.AspNetCore.Http;

using Moq;

using Xunit;

namespace IAG.Infrastructure.Test.IdentityServer.Authentication;

public class UserContextTest
{
    private static readonly string TestUser = "testUser";
    private static readonly Guid TestTenant = Guid.NewGuid();

    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;

    public UserContextTest()
    {

        var httpContext = new DefaultHttpContext();
        _httpContextAccessor = new Mock<IHttpContextAccessor>();

        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, TestUser), new Claim(CustomClaimTypes.Tenant, TestTenant.ToString()), }));
        _httpContextAccessor.Setup(m => m.HttpContext).Returns(httpContext);
    }

    [Fact]
    public void ConstructorTest()
    {
        var userContextHttp = new UserContext(_httpContextAccessor.Object);
        var userContextNull = new UserContext(null);

        Assert.Equal(TestUser, userContextHttp.UserName);
        Assert.Equal(TestTenant, userContextHttp.TenantId);
        Assert.Equal(UserContext.AnonymousUserName, userContextNull.UserName);
        Assert.Null(userContextNull.TenantId);
    }

    [Fact]
    public void SetExplicitUserTest()
    {
        var userContextHttp = new UserContext(_httpContextAccessor.Object);
        var userContextNull = new UserContext(null);
        var tenantIdHttp = Guid.NewGuid();
        var tenantIdNull = Guid.NewGuid();

        userContextHttp.SetExplicitUser("ExplicitUserName", tenantIdHttp);
        userContextNull.SetExplicitUser("ExplicitUserName", tenantIdNull);

        Assert.Equal("ExplicitUserName", userContextHttp.UserName);
        Assert.Equal(tenantIdHttp, userContextHttp.TenantId);
        Assert.Equal("ExplicitUserName", userContextNull.UserName);
        Assert.Equal(tenantIdNull, userContextNull.TenantId);
    }
}