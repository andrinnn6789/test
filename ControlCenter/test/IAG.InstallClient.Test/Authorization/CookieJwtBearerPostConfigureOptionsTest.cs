using System.Threading;

using IAG.InstallClient.Authorization;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

using Moq;

using Xunit;

namespace IAG.InstallClient.Test.Authorization;

public class CookieJwtBearerPostConfigureOptionsTest
{
    private readonly JwtBearerOptions _options;
    private readonly AuthenticationScheme _authSchema;

    public CookieJwtBearerPostConfigureOptionsTest()
    {
        var postConfigureOptions = new CookieJwtBearerPostConfigureOptions();
        _options = new JwtBearerOptions();
        _authSchema = new AuthenticationScheme("Bearer", null, typeof(JwtBearerHandler));

        postConfigureOptions.PostConfigure("Test", _options);

    }

    [Fact]
    public void OnMessageWithoutTokenReceivedTest()
    {
        var httpContext = new DefaultHttpContext();
        var msgContext = new MessageReceivedContext(httpContext, _authSchema, _options);
        _options.Events.OnMessageReceived(msgContext);

        Assert.Null(msgContext.Token);
    }
        
    [Fact]
    public void OnMessageWithTokenReceivedTest()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = new[] { "Bearer-Token=ValidBearerToken" };
        var msgContext = new MessageReceivedContext(httpContext, _authSchema, _options);
        var keySet = new JsonWebKeySet();
        keySet.Keys.Add(new JsonWebKey());
        keySet.SkipUnresolvedJsonWebKeys = false;
        msgContext.Options.Configuration = new OpenIdConnectConfiguration()
        {
            JsonWebKeySet = keySet
        };
        _options.Events.OnMessageReceived(msgContext);

        Assert.Equal("ValidBearerToken", msgContext.Token);
        Assert.NotEmpty(msgContext.Options.Configuration.SigningKeys);
    }

    [Fact]
    public void OnMessageWithTokenReceivedAndConfigurationManagerTest()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = new[] { "Bearer-Token=ValidBearerToken" };
        var msgContext = new MessageReceivedContext(httpContext, _authSchema, _options);
        var keySet = new JsonWebKeySet();
        keySet.Keys.Add(new JsonWebKey());
        keySet.SkipUnresolvedJsonWebKeys = false;
        var configurationManagerMock = new Mock<IConfigurationManager<OpenIdConnectConfiguration>>();
        configurationManagerMock.Setup(m => m.GetConfigurationAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OpenIdConnectConfiguration() { JsonWebKeySet = keySet });

        msgContext.Options.ConfigurationManager = configurationManagerMock.Object;
        _options.Events.OnMessageReceived(msgContext);

        Assert.Equal("ValidBearerToken", msgContext.Token);
        Assert.NotNull(msgContext.Options.Configuration);
        Assert.NotEmpty(msgContext.Options.Configuration.SigningKeys);
    }
}