using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.TestHelper.MockHost;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;
using IAG.VinX.IAG.ControlCenter.Distribution.Rest;

using Xunit;

namespace IAG.VinX.IAG.IntegrationTest.ControlCenter.Distribution.Rest;

public class LinkAdminClientTest
{
    [Fact]
    public async Task HappyPathTest()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(GetType().Namespace + ".LinkAdminRequestMock.json", testPort);

        var httpConfig = new HttpConfig() { BaseUrl = $"http://localhost:{testPort}/{Endpoints.Distribution}" };

        var client = new LinkAdminClient(httpConfig);
        var links = await client.SyncLinksAsync(new List<LinkData>() { new() });

        Assert.NotNull(links);
        Assert.Single(links);
    }

    [Fact]
    public async Task ErrorTest()
    {
        var testPort = KestrelMock.NextFreePort;
        KestrelMock.Run(GetType().Namespace + ".LinkAdminRequestErrorMock.json", testPort);

        var httpConfig = new HttpConfig() { BaseUrl = $"http://localhost:{testPort}/{Endpoints.Distribution}" };

        var client = new LinkAdminClient(httpConfig);

        await Assert.ThrowsAsync<LocalizableException>(() => client.SyncLinksAsync(new List<LinkData>()));
    }
}