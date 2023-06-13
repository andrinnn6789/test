using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.TestHelper.Startup;
using IAG.VinX.Langendorf.Shop.v10.DtoDirect;

using Newtonsoft.Json;

using Xunit;

namespace IAG.VinX.Langendorf.IntegrationTest.Shop.v10.CoreServer;

[Collection("ControllerDwaTest")]
public class BaseDataLadControllerTest : IClassFixture<TestServerEnvironment>
{
    private readonly string _url = ControllerEndpoints.ShopV10 + "BaseDataLad/";

    private readonly HttpClient _client;

    public BaseDataLadControllerTest(TestServerEnvironment testEnvironment)
    {
        _client = testEnvironment.NewClient();
    }

    [Fact]
    public async Task ArticleGet()
    {
        var response = await _client.GetAsync($"{_url}ArticleLad?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<ArticleLad>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }
}