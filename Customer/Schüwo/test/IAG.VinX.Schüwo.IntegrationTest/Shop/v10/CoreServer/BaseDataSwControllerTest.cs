using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.TestHelper.Startup;
using IAG.VinX.Schüwo.Shop.v10.DtoDirect;

using Newtonsoft.Json;

using Xunit;

namespace IAG.VinX.Schüwo.IntegrationTest.Shop.v10.CoreServer;

[Collection("ControllerSwTest")]
public class BaseDataSwControllerTest : IClassFixture<TestServerEnvironment>
{
    private readonly string _url = ControllerEndpoints.ShopV10 + "BaseDataSw/";

    private readonly HttpClient _client;

    public BaseDataSwControllerTest(TestServerEnvironment testEnvironment)
    {
        _client = testEnvironment.NewClient();
    }

    [Fact]
    public async Task ArticleGet()
    {
        var response = await _client.GetAsync($"{_url}ArticleSw?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<ArticleSw>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task ArticleRelationGet()
    {
        var response = await _client.GetAsync($"{_url}ArticleRelationSw?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<ArticleRelationSw>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task WineCharacterGet()
    {
        var response = await _client.GetAsync($"{_url}WineCharacterSw?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<WineCharacterSw>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task CustomerMarketingGet()
    {
        var response = await _client.GetAsync($"{_url}CustomerMarketingSw?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<CustomerMarketingSw>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task TradingUnitSwGet()
    {
        var response = await _client.GetAsync($"{_url}TradingUnitSw?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<TradingUnitSw>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task MarketingCodeGet()
    {
        var response = await _client.GetAsync($"{_url}MarketingCodeSw?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<MarketingCodeSw>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task RegionSwGet()
    {
        var response = await _client.GetAsync($"{_url}RegionSw?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<RegionSw>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task SelectionCodeSwGet()
    {
        var response = await _client.GetAsync($"{_url}SelectionCodeSw?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<SelectionCodeSw>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task CountrySwGet()
    {
        var response = await _client.GetAsync($"{_url}CountrySw?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<CountrySw>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task EcommerceGroupSwGet()
    {
        var response = await _client.GetAsync($"{_url}EcommerceGroupSw?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<EcommerceGroupSw>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task AddressSwGet()
    {
        var response = await _client.GetAsync($"{_url}AddressSw?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<AddressSw>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }
}