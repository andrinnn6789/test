using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.TestHelper.Startup;
using IAG.VinX.Boucherville.Shop.v10.DtoDirect;

using Newtonsoft.Json;

using Xunit;

namespace IAG.VinX.Boucherville.IntegrationTest.Shop.v10.CoreServer;

[Collection("ControllerBovTest")]
public class BaseDataBovControllerTest : IClassFixture<TestServerEnvironment>
{
    private const string Url = ControllerEndpoints.ShopV10 + "BaseDataBov/";

    private readonly HttpClient _client;

    public BaseDataBovControllerTest(TestServerEnvironment testEnvironment)
    {
        _client = testEnvironment.NewClient();
    }

    [Fact]
    public async Task AddressGet()
    {
        await GetTest<AddressBov>("AddressBov?$top=1");
    }

    [Fact]
    public async Task ArticleGet()
    {
        await GetTest<ArticleBov>("ArticleBov?$top=1");
    }

    [Fact]
    public async Task ECommerceGroupGet()
    {
        await GetTest<ECommerceGroupBov>("ECommerceGroupBov?$top=1");
    }

    [Fact]
    public async Task FillingGet()
    {
        await GetTest<FillingBov>("FillingBov?$top=1");
    }

    [Fact]
    public async Task TradingUnitGet()
    {
        await GetTest<TradingUnitBov>("TradingUnitBov?$top=1");
    }

    [Fact]
    public async Task CountryGet()
    {
        await GetTest<CountryBov>("CountryBov?$top=1");
    }

    [Fact]
    public async Task RegionGet()
    {
        await GetTest<RegionBov>("RegionBov?$top=1");
    }

    [Fact]
    public async Task PredicateGet()
    {
        await GetTest<PredicateBov>("PredicateBov?$top=1");
    }

    [Fact]
    public async Task ProducerGet()
    {
        await GetTest<ProducerBov>("ProducerBov?$top=1");
    }

    [Fact]
    public async Task GrapeGet()
    {
        await GetTest<GrapeBov>("GrapeBov?$top=1");
    }

    [Fact]
    public async Task WineGet()
    {
        await GetTest<WineBov>("WineBov?$top=1");
    }

    [Fact]
    public async Task PaymentConditionGet()
    {
        await GetTest<PaymentConditionBov>("PaymentConditionBov?$top=1");
    }

    [Fact]
    public async Task DeliveryConditionGet()
    {
        await GetTest<DeliveryConditionBov>("DeliveryConditionBov?$top=1");
    }

    [Fact]
    public async Task ReceiptTypeGet()
    {
        await GetTest<ReceiptTypeBov>("ReceiptTypeBov?$top=1");
    }

    [Fact]
    public async Task SelectionCodeV10Get()
    {
        await GetTest<SelectionCodeBov>("SelectionCodeBov?$top=1");
    }

    private async Task GetTest<T>(string query)
    {
        var response = await _client.GetAsync($"{Url}{query}");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<T>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }
}