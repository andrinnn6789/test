using System.Threading.Tasks;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.TestHelper.MockHost;
using IAG.InstallClient.BusinessLogic;

using Microsoft.Extensions.Configuration;

using Moq;

using Xunit;

namespace IAG.InstallClient.IntegrationTest.BusinessLogic;

public class LoginManagerTest
{
    private readonly ILoginManager _loginManager;

    public LoginManagerTest()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(GetType().Namespace + ".LoginManagerRequestMock.json", testPort);

        var config = new Mock<IConfiguration>();
        config.SetupGet(m => m["Authentication:IdentityServer"]).Returns($"http://localhost:{testPort}");
            
        _loginManager = new LoginManager(config.Object, null, null, null);
    }

    [Fact]
    public async Task DoLoginSuccessTest()
    {
        var token = await _loginManager.DoLoginAsync("TestUser", "CorrectPassword");

        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task DoLoginForbiddenTest()
    {
        await Assert.ThrowsAsync<LocalizableException>(() => _loginManager.DoLoginAsync("TestUser", "WrongPassword"));
    }

    [Fact]
    public async Task DoLoginExceptionTest()
    {
        await Assert.ThrowsAsync<LocalizableException>(() => _loginManager.DoLoginAsync("TestUser", "Exception"));
    }
}