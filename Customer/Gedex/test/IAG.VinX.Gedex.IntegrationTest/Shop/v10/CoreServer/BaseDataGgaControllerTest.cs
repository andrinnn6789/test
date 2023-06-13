using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.TestHelper.Startup;
using IAG.VinX.Gedex.Shop.v10.DtoDirect;

using Newtonsoft.Json;

using Xunit;

namespace IAG.VinX.Gedex.IntegrationTest.Shop.v10.CoreServer;

[Collection("ControllerGgaTest")]
public class BaseDataGgaControllerTest : IClassFixture<TestServerEnvironment>
{
    private readonly string _url = ControllerEndpoints.ShopV10 + "BaseDataGga/";

    private readonly HttpClient _client;

    public BaseDataGgaControllerTest(TestServerEnvironment testEnvironment)
    {
        _client = testEnvironment.NewClient();
    }

    [Fact]
    public async Task ArticleGet()
    {
        var response = await _client.GetAsync($"{_url}ArticleGga?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<ArticleGga>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GrapeAmountGet()
    {
        var response = await _client.GetAsync($"{_url}GrapeAmountGga?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<GrapeAmountGga>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task MatchGet()
    {
        var response = await _client.GetAsync($"{_url}MatchGga?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<MatchGga>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task StrengthGet()
    {
        var response = await _client.GetAsync($"{_url}StrengthGga?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<StrengthGga>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task TasteGet()
    {
        var response = await _client.GetAsync($"{_url}TasteGga?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<TasteGga>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task WineGet()
    {
        var response = await _client.GetAsync($"{_url}WineGga?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<WineGga>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task WineMatchRelationGet()
    {
        var response = await _client.GetAsync($"{_url}WineMatchRelationGga?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<WineMatchRelationGga>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task WineTasteRelationGet()
    {
        var response = await _client.GetAsync($"{_url}WineTasteRelationGga?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<WineTasteRelationGga>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }
}