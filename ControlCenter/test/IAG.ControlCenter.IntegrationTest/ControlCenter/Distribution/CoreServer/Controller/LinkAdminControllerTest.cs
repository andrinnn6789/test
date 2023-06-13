using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.TestHelper.Startup;

using Newtonsoft.Json;

using Xunit;

namespace IAG.ControlCenter.IntegrationTest.ControlCenter.Distribution.CoreServer.Controller;

[Collection("TestEnvironmentCollection")]
public class LinkAdminControllerTest
{
    private const string Uri = ControlCenterEndpoints.Distribution + "LinkAdmin/";
    private readonly HttpClient _client;

    public LinkAdminControllerTest(TestServerEnvironment testEnvironment)
    {
        _client = testEnvironment.NewClient();
    }

    [Fact]
    public async Task SyncLinksTest()
    {
        var newLinks = new List<LinkRegistration>
        {
            new() {Name = "NewLink1", Url = "www.new-link.one", Description = "Description One"},
            new() {Name = "NewLink2", Url = "www.new-link.two", Description = "Description Two"}
        };
        var syncLinksResponse = await _client.PostAsync(Uri + "Link/Sync", new StringContent(JsonConvert.SerializeObject(newLinks), Encoding.UTF8, "application/json"));
        syncLinksResponse.EnsureSuccessStatusCode();
        var links = JsonConvert.DeserializeObject<List<LinkInfo>>(await syncLinksResponse.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, syncLinksResponse.StatusCode);
        Assert.NotNull(links);
        Assert.Equal(2, links.Count());
        Assert.All(links, link => Assert.NotEqual(Guid.Empty, link.Id));
        Assert.All(newLinks, link => Assert.Contains(links, resLink => resLink.Name == link.Name && resLink.Url == link.Url && link.Description == resLink.Description));
    }
}