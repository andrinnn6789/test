using IAG.Infrastructure.TestHelper.Startup;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IAG.PerformX.ibW.IntergrationsTest.CoreServer;

[Collection("ibWController")]
public class BaseControllerTest : IClassFixture<TestServerEnvironment>
{
    protected BaseControllerTest(TestServerEnvironment testEnvironment, string endPoint)
    {
        EndPoint = endPoint;
        Client = testEnvironment.NewClient();
    }

    protected HttpClient Client { get; }

    protected string EndPoint { get; }

    protected async Task<T> Take1Get<T>()
    {
        var response = await Client.GetAsync($"{EndPoint}{typeof(T).Name}?%24top=1");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<T>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
        return items.First();
    }

    protected async Task<T> ChangedOnAndTake1Get<T>(string prefix = "")
    {
        var endpoint = $"{EndPoint}{prefix}{typeof(T).Name}";
        var response = await Client.GetAsync(
            $"{endpoint}?%24top1&$filter=lastchange gt 2014-06-23T03:30:00.000Z and lastchange lt 2014-06-26T03:30:00.000Z&$orderby=id desc");
        response.EnsureSuccessStatusCode();
        var items = JsonConvert.DeserializeObject<List<T>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.Empty(items);

        response = await Client.GetAsync($"{endpoint}?%24top=1&$filter=lastchange gt 2014-06-23T03:30:00.000Z");
        response.EnsureSuccessStatusCode();
        items = JsonConvert.DeserializeObject<List<T>>(await response.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.NotEmpty(items);
        return items.First();
    }
}