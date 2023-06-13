using System.Net;

using IAG.Infrastructure.TestHelper.Startup;
using IAG.VinX.Schuerch.Shop.v10.DtoDirect;

using Newtonsoft.Json;

using Xunit;

namespace IAG.VinX.Schuerch.IntegrationTest.Shop.CoreServer;

[Collection("ControllerSchuerchTest")]
public class BaseDataSchuerchControllerTest : IClassFixture<TestServerEnvironment>
{
    private const string Url = ControllerEndpoints.ShopV10 + "Schuerch/" + "BaseData/";
    private readonly HttpClient _httpClient;

    public BaseDataSchuerchControllerTest(TestServerEnvironment testServerEnvironment)
    {
        _httpClient = testServerEnvironment.NewClient();
    }

    [Fact]
    public async Task FillingGet()
    {
        var response = await _httpClient.GetAsync($"{Url}Filling?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<FillingSchuerch>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task ArticleGet()
    {
        var response = await _httpClient.GetAsync($"{Url}Article?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<ArticleSchuerch>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }
}