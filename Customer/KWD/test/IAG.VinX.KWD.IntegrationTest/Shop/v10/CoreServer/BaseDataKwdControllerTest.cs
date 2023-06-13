using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.TestHelper.Startup;
using IAG.VinX.KWD.Shop.v10.DtoDirect;

using Newtonsoft.Json;

using Xunit;

namespace IAG.VinX.Kwd.IntegrationTest.Shop.v10.CoreServer;

public class BaseDataKwdControllerTest : IClassFixture<TestServerEnvironment>
{
    private readonly string _url = ControllerEndpoints.ShopV10 + "BaseDataKwd/";

    private readonly HttpClient _httpClient;

    public BaseDataKwdControllerTest(TestServerEnvironment testServerEnvironment)
    {
        _httpClient = testServerEnvironment.NewClient();
    }

    [Fact]
    public async Task GetArticles()
    {
        var response = await _httpClient.GetAsync($"{_url}ArticleKwd?$top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<ArticleKwd>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }
}