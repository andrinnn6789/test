using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.Startup;
using IAG.Infrastructure.TestHelper.Startup;
using IAG.Infrastructure.TestHelper.xUnit;

using Newtonsoft.Json;

using Xunit;

namespace IAG.PerformX.CampusSursee.IntegrationTest.CoreServer;

public class BaseControllerTest : IClassFixture<TestServerEnvironment>
{
    private readonly string _endPoint;
    protected const string Uri = "api/Core/" + SwaggerEndpointProvider.ApiEndpoint + "/";

    protected BaseControllerTest(TestServerEnvironment testEnvironment, string endPoint)
    {
        _endPoint = endPoint;
        Client = testEnvironment.NewClient();
        Connection = new SybaseConnectionFactory(
            new ExplicitUserContext("test", null),
            new MockILogger<SybaseConnection>(), 
            Startup.BuildConfig(),
            null).CreateConnection();
    }

    protected string Url => Uri + $"{_endPoint}/";

    protected HttpClient Client { get; }

    protected ISybaseConnection Connection { get; }

    protected async Task<T> ChangedOnAndTake1Get<T>()
    {
        var response = await Client.GetAsync(
            $"{Url}{typeof(T).Name}?%24top=1&$filter=lastchange gt 2014-06-23T03:30:00.000Z and lastchange lt 2014-06-26T03:30:00.000Z&$orderby=id desc");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<T>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.Empty(items);

        response = await Client.GetAsync($"{Url}{typeof(T).Name}?%24top=1&$filter=lastchange gt 2014-06-23T03:30:00.000Z");
        response.EnsureSuccessStatusCode();
        items = JsonConvert.DeserializeObject<List<T>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
        return items.First();
    }
}