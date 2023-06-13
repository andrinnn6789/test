using System;

using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Model;

using Xunit;

namespace IAG.Infrastructure.Test.IdentityServer.Model;

public class SimpleRequestTokenParameterTest
{
    [Fact]
    public void ToRequestTokenParameterTest()
    {
        var simpleParameter = new SimpleRequestTokenParameter()
        {
            Username = "TestUser",
            Password = "TestPassword",
            TenantId = Guid.NewGuid()
        };
        var requestTokenParam = simpleParameter.ToRequestTokenParameter();

        Assert.NotNull(requestTokenParam);
        Assert.Equal(GrantTypes.Password, requestTokenParam.GrantType);
        Assert.Equal("TestUser", requestTokenParam.Username);
        Assert.Equal("TestPassword", requestTokenParam.Password);
        Assert.Equal(simpleParameter.TenantId, requestTokenParam.TenantId);
    }

    [Fact]
    public void ForRefreshGrantTest()
    {
        var simpleParameter = new SimpleRequestTokenParameter()
        {
            RefreshToken = "TestRefreshToken"
        };
        var requestTokenParam = simpleParameter.ToRequestTokenParameter();

        Assert.NotNull(requestTokenParam);
        Assert.Equal(GrantTypes.RefreshToken, requestTokenParam.GrantType);
        Assert.Equal("TestRefreshToken", requestTokenParam.RefreshToken);
    }
}