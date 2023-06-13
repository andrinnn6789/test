using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.TestHelper.MockHost;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.IdentityServer.Authentication;

public class TokenRequestTest
{
    [Fact]
    public void GetBearerConfigTest()
    {
        var testPort = KestrelMock.NextFreePort;

        KestrelMock.Run(GetType().Namespace + ".TokenRequestMock.json", testPort);

        var tokenRequest = new TokenRequest();
        const string baseUrl = "http://cc.i-ag.ch:8088/api";
        var response = tokenRequest.GetBearerConfig($"http://localhost:{testPort}/api/auth", new RequestTokenParameter(), 
            baseUrl, null);
        Assert.Equal(baseUrl, response.BaseUrl);
        Assert.NotNull(response.Authentication.GetAuthorizationHeader());
        Assert.Equal("i-am-the-token", response.Authentication.GetAuthorizationHeader().Parameter);
        Assert.Equal("Bearer", response.Authentication.GetAuthorizationHeader().Scheme);
    }
}