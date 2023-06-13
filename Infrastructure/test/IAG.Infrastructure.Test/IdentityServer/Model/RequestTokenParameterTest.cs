using System;

using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Model;

using Xunit;

namespace IAG.Infrastructure.Test.IdentityServer.Model;

public class RequestTokenParameterTest
{
    [Fact]
    public void ScopeSetterTest()
    {
        var requestTokenParam = new RequestTokenParameter();
        requestTokenParam.ScopeSetter = "Scope1 Scope2 Scope3";

        Assert.NotNull(requestTokenParam.Scopes);
        Assert.NotEmpty(requestTokenParam.Scopes);
        Assert.Equal(3, requestTokenParam.Scopes.Count);
        Assert.Contains("Scope1", requestTokenParam.Scopes);
        Assert.Contains("Scope2", requestTokenParam.Scopes);
        Assert.Contains("Scope3", requestTokenParam.Scopes);
    }

    [Fact]
    public void AcrValuesSetterTest()
    {
        var tenantId = Guid.NewGuid();
        var requestTokenParam = new RequestTokenParameter();
        requestTokenParam.AcrValuesSetter = $"IrrelevantNumber:42 tenant:{tenantId} IrrelevantString:FooBar";

        Assert.NotNull(requestTokenParam.TenantId);
        Assert.Equal(tenantId, requestTokenParam.TenantId);
    }

    [Fact]
    public void ForPasswordGrantTest()
    {
        var testTenant = Guid.NewGuid();
        var requestTokenParam = RequestTokenParameter.ForPasswordGrant("TestUser", "TestPassword", testTenant);

        Assert.NotNull(requestTokenParam);
        Assert.Equal(GrantTypes.Password, requestTokenParam.GrantType);
        Assert.Equal("TestUser", requestTokenParam.Username);
        Assert.Equal("TestPassword", requestTokenParam.Password);
        Assert.Equal(testTenant, requestTokenParam.TenantId);
    }

    [Fact]
    public void ForRefreshGrantTest()
    {
        var requestTokenParam = RequestTokenParameter.ForRefreshGrant("TestRefreshToken");

        Assert.NotNull(requestTokenParam);
        Assert.Equal(GrantTypes.RefreshToken, requestTokenParam.GrantType);
        Assert.Equal("TestRefreshToken", requestTokenParam.RefreshToken);
    }
}