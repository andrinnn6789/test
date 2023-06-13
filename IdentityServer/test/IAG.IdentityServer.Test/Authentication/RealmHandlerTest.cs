using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.IdentityServer.Authentication;
using IAG.IdentityServer.Configuration;
using IAG.IdentityServer.Configuration.Model.Realm;
using IAG.IdentityServer.Test.Configuration.Mock;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.IdentityServer.Plugin;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;

using Moq;

using Xunit;

namespace IAG.IdentityServer.Test.Authentication;

public class RealmHandlerTest
{
    private readonly IRealmConfig _realmConfig;
    private readonly RealmHandler _realmHandler;

    public RealmHandlerTest()
    {
        var pluginMeta = new Mock<IPluginMetadata>();
        pluginMeta.SetupGet(m => m.PluginId).Returns(TestPlugin.Id);
        pluginMeta.SetupGet(m => m.PluginConfigType).Returns(typeof(TestPluginConfig));
        var plugins = new List<IPluginMetadata> { pluginMeta.Object };
        var authenticationPlugin = new Mock<IAuthenticationPlugin>();
        var pluginCatalogue = new Mock<IPluginCatalogue>();
        pluginCatalogue.SetupGet(m => m.Plugins).Returns(plugins);
        pluginCatalogue.Setup(m => m.GetAuthenticationPlugin(It.IsAny<Guid>()))
            .Returns<Guid>(id => plugins.Any(p => p.PluginId == id) ? authenticationPlugin.Object : null);
        pluginCatalogue.Setup(m => m.GetPluginMeta(It.IsAny<Guid>()))
            .Returns<Guid>(
                id => { return plugins.FirstOrDefault(p => p.PluginId == id); });
            
        _realmConfig = new TestRealmConfig();
        var realms = new List<IRealmConfig> { _realmConfig };

        var realmCatalogue = new Mock<IRealmCatalogue>();
        realmCatalogue.SetupGet(m => m.Realms).Returns(realms);
        realmCatalogue.Setup(m => m.GetRealm(It.IsAny<string>()))
            .Returns<string>(id => { return realms.FirstOrDefault(p => p.Realm == id); });
            
        var refreshTokenManager = new Mock<IRefreshTokenManager>();
        refreshTokenManager.Setup(m => m.CheckRefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((IAuthenticationToken)null);
        refreshTokenManager.Setup(m => m.CheckRefreshTokenAsync("GoodToken"))
            .ReturnsAsync(new AuthenticationToken()
            {
                RefreshToken = "NewRefreshToken"
            });

        var stringLocalizer = new Mock<IStringLocalizer<RealmHandler>>();
            
            
        _realmHandler = new RealmHandler(realmCatalogue.Object, pluginCatalogue.Object, refreshTokenManager.Object, stringLocalizer.Object);
    }

    [Fact]
    public async Task RequestRefreshTokenTest()
    {
        var goodRefreshToken = RequestTokenParameter.ForRefreshGrant("GoodToken");
        var badRefreshToken = RequestTokenParameter.ForRefreshGrant("BadToken");

        var tokenHandler = new Mock<ITokenHandler>();
        tokenHandler.Setup(m => m.GetJwtToken(It.IsAny<IAuthenticationToken>(), It.IsAny<string>()))
            .Returns<IAuthenticationToken, string>((_, _) => "JwtToken");

        var goodResponse = await _realmHandler.RequestToken(goodRefreshToken, null, tokenHandler.Object);
        var badResponse = await _realmHandler.RequestToken(badRefreshToken, null, tokenHandler.Object);

        var goodAuthToken = Assert.IsType<ActionResult<RequestTokenResponse>>(goodResponse).Value;
        var badResult = Assert.IsType<ActionResult<RequestTokenResponse>>(badResponse).Result as IStatusCodeActionResult;
        Assert.NotNull(goodAuthToken);
        Assert.Equal("Bearer", goodAuthToken.TokenType);
        Assert.Equal("JwtToken", goodAuthToken.AccessToken);
        Assert.Equal("NewRefreshToken", goodAuthToken.RefreshToken);
        Assert.NotNull(badResult);
        Assert.Equal(403, badResult.StatusCode);
    }

    [Fact]
    public void GetPluginsTest()
    {
        var emptyPlugins = _realmHandler.GetAuthPlugins();
        _realmConfig.AuthenticationPluginConfig.Active = true;
        var singleActivePlugin = _realmHandler.GetAuthPlugins();

        Assert.Empty(emptyPlugins);
        Assert.Single(singleActivePlugin);
    }
}