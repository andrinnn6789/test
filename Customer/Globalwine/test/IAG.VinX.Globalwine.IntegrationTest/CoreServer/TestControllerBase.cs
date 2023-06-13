using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.TestHelper.Startup;
using IAG.Infrastructure.TestHelper.xUnit;

using Newtonsoft.Json;

using Xunit;

namespace IAG.VinX.Globalwine.IntegrationTest.CoreServer;

public class TestControllerBase : IClassFixture<TestServerEnvironment>
{
    private readonly string _endPoint;
    private const string Uri = "api/Core/" + SwaggerEndpointProvider.ApiEndpoint + "/";

    protected TestControllerBase(TestServerEnvironment testEnvironment, string endPoint)
    {
        _endPoint = endPoint;
        Client = testEnvironment.NewClient();
        Connection = new SybaseConnectionFactory(
            new ExplicitUserContext("test", null),
            new MockILogger<SybaseConnection>(), 
            Infrastructure.Startup.Startup.BuildConfig(),
            null).CreateConnection();
    }

    protected string Url => Uri + $"{_endPoint}/";

    protected HttpClient Client { get; }

    protected ISybaseConnection Connection { get; }

    protected async Task ChangedOnAndTake1Get<T>(string name = null)
    {
        name ??= typeof(T).Name;
        var response = await Client.GetAsync(
            $"{Url}{name}?%24top1&$filter=changedOn gt 2014-06-23T03:30:00.000Z and changedOn lt 2014-06-26T03:30:00.000Z&$orderby=id desc");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<T>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.Empty(items);

        response = await Client.GetAsync($"{Url}{name}?%24top=1&$filter=changedOn gt 2014-06-23T03:30:00.000Z");
        response.EnsureSuccessStatusCode();
        items = JsonConvert.DeserializeObject<List<T>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    protected async Task Take1<T>(string name = null)
    {
        name ??= typeof(T).Name;
        var response = await Client.GetAsync($"{Url}{name}?%24top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<T>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }
}