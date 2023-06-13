using System;
using System.Threading.Tasks;
using IAG.Infrastructure.TestHelper.MockHost;
using IAG.InstallClient.BusinessLogic;

using Microsoft.Extensions.Configuration;

using Moq;

using Xunit;

namespace IAG.InstallClient.IntegrationTest.BusinessLogic;

public class LinkListManagerTest
{
    private static readonly Guid TestCustomerId = Guid.Parse("68b7efca-ca3f-454e-977d-0ff767fad44b");

    private readonly ILinkListManager _linkListManager;

    public LinkListManagerTest()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(GetType().Namespace + ".LinkListManagerRequestMock.json", testPort);

        var config = new Mock<IConfiguration>();
        config.SetupGet(m => m["ControlCenter:BaseUrl"]).Returns($"http://localhost:{testPort}");
            
        _linkListManager = new LinkListManager(config.Object);
    }

    [Fact]
    public async Task GetLinksTest()
    {
        var links = await _linkListManager.GetLinksAsync(TestCustomerId);

        Assert.NotEmpty(links);
    }
}