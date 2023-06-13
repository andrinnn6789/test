using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.TestHelper.MockHost;

using Microsoft.Extensions.Configuration;

using Moq;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.IdentityServer.Authorization.ClaimProvider;

public class RemoteClaimPublisherTest
{
    [Fact]
    public async Task PublishClaimsAsyncOkTest()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(GetType().Namespace + ".PublishRequestMock.json", testPort);

        var configMock = new Mock<IConfigurationRoot>();
        configMock.Setup(m => m["Authentication:IdentityServer"]).Returns($"http://localhost:{testPort}");
        configMock.Setup(m => m["Authentication:SystemUser"]).Returns("User");
        configMock.Setup(m => m["Authentication:SystemPassword"]).Returns("WontBeChecked");

        var remotePublisher = new RemoteClaimPublisher(configMock.Object);

        await remotePublisher.PublishClaimsAsync(Enumerable.Empty<ClaimDefinition>());
    }

    [Fact]
    public async Task PublishClaimsAsyncFailsTest()
    {
        var configMock = new Mock<IConfigurationRoot>();

        var remotePublisher = new RemoteClaimPublisher(configMock.Object);

        await Assert.ThrowsAsync<System.Exception>(() => remotePublisher.PublishClaimsAsync(Enumerable.Empty<ClaimDefinition>()));
    }

}