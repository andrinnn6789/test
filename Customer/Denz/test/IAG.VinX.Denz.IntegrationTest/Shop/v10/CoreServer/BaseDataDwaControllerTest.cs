using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.TestHelper.Startup;
using IAG.VinX.Denz.Shop.v10.DtoDirect;

using Newtonsoft.Json;

using Xunit;

namespace IAG.VinX.Denz.IntegrationTest.Shop.v10.CoreServer;

[Collection("ControllerDwaTest")]
public class BaseDataDwaControllerTest : IClassFixture<TestServerEnvironment>
{
    private readonly string _url = ControllerEndpoints.ShopV10 + "BaseDataDwa/";

    private readonly HttpClient _client;

    public BaseDataDwaControllerTest(TestServerEnvironment testEnvironment)
    {
        _client = testEnvironment.NewClient();
    }

    [Fact]
    public async Task ArticleGet()
    {
        var response = await _client.GetAsync($"{_url}ArticleDwa?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<ArticleDwa>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task WineInfosGet()
    {
        var response = await _client.GetAsync($"{_url}WineDwa?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<WineDwa>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }
}