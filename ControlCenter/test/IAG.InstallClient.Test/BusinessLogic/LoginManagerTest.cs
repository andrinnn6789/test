using System.Threading.Tasks;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.IdentityServer;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.InstallClient.BusinessLogic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Moq;

using Xunit;

namespace IAG.InstallClient.Test.BusinessLogic;

public class LoginManagerTest
{
    [Fact]
    public void ConstructorWithoutConfiguredIdentityServerTest()
    {
        var configMock = new Mock<IConfiguration>();

        Assert.Throws<LocalizableException>(() => new LoginManager(configMock.Object, null, null, null));
    }

    [Fact]
    public void ConstructorWithConfiguredIdentityServerTest()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(m => m["Authentication:IdentityServer"]).Returns("http://localhost:8079");

        var manager = new LoginManager(configMock.Object, null, null, null);

        Assert.NotNull(manager);
    }

    [Fact]
    public async Task DoLoginExceptionTest()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(m => m["Authentication:IdentityServer"]).Returns("http://notexisting:8079");

        var manager = new LoginManager(configMock.Object, null, null, null);

        await Assert.ThrowsAsync<LocalizableException>(() => manager.DoLoginAsync("TestUser", "DoesNotMatter"));
    }

    [Fact]
    public async Task DoLoginIntegratedSuccessTest()
    {
        var loginManager = GetIntegratedLoginManager();
        var token = await loginManager.DoLoginAsync("TestUser", "CorrectPassword");

        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task DoLoginIntegratedForbiddenTest()
    {
        var loginManager = GetIntegratedLoginManager();
        await Assert.ThrowsAsync<LocalizableException>(() => loginManager.DoLoginAsync("TestUser", "WrongPassword"));
    }

    private ILoginManager GetIntegratedLoginManager()
    {
        var configMock = new Mock<IConfiguration>();
        var realmHandlerMock = new Mock<IRealmHandler>();
        realmHandlerMock.Setup(m => m.RequestToken(It.IsAny<RequestTokenParameter>(), It.IsAny<IAttackDetection>(), It.IsAny<ITokenHandler>()))
            .Returns((RequestTokenParameter parameter, IAttackDetection _, ITokenHandler _) =>
            {
                RequestTokenResponse response = null;
                if (parameter.Password == "CorrectPassword")
                {
                    response = new RequestTokenResponse()
                    {
                        AccessToken = "ValidToken"
                    };
                }

                return Task.FromResult(new ActionResult<RequestTokenResponse>(response));
            });

        return new LoginManager(configMock.Object, realmHandlerMock.Object, null, null);
    }
}