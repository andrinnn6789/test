using System.Net;

using IAG.Infrastructure.TestHelper.Startup;
using IAG.VinX.Kouski.Shop.v10.DtoDirect;

using Newtonsoft.Json;

using Xunit;

namespace IAG.VinX.Kouski.Test.Shop.CoreServer;

[Collection("ControllerKouskiTest")]
public class BaseDataKouskiControllerTest : IClassFixture<TestServerEnvironment>
{
    private readonly string _url = ControllerEndpoints.ShopV10 + "BaseDataKouski/";

    private readonly HttpClient _client;

    public BaseDataKouskiControllerTest(TestServerEnvironment testEnvironment)
    {
        _client = testEnvironment.NewClient();
    }

    [Fact]
    public async Task ArticleGet()
    {
        var response = await _client.GetAsync($"{_url}ArticleCefs?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<ArticleKouski>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }
}