using IAG.Infrastructure.TestHelper.MockHost;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Config;
using IAG.VinX.IAG.IntegrationTest.ControlCenter.Mobile.ProcessEngine;

using Xunit;

namespace IAG.VinX.IAG.IntegrationTest.ControlCenter.Common;

public class ControlCenterTokenRequestTest 
{
    [Fact]
    public void GetConfigTest()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(typeof(LicenceSyncerJobTest).Namespace + ".RequestMock.json", testPort);

        var urlCc = "http://localhost:8088/";
        var urlBack = $"http://localhost:{testPort}/";
        var tokenRequest = new ControlCenterTokenRequest();
        var backendConfig = new BackendConfig
        {
            Username = "test",
            Password = "test",
            UrlAuth = urlBack,
            UrlControlCenter = urlCc
        };

        var response = tokenRequest.GetConfig(backendConfig, Endpoints.Control, null);
        Assert.Equal(urlCc + Endpoints.Control, response.BaseUrl);
        Assert.NotNull(response.Authentication.GetAuthorizationHeader());
        Assert.Equal("i-am-the-token", response.Authentication.GetAuthorizationHeader().Parameter);
        Assert.Equal("Bearer", response.Authentication.GetAuthorizationHeader().Scheme);
    }
}