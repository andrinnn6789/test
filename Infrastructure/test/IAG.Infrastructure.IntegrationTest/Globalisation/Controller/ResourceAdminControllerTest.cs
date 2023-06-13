using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.TestHelper.Startup;

using Newtonsoft.Json;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.Globalisation.Controller;

[Collection("InfrastructureController")]
public class ResourceAdminControllerTest
{
    private const string Uri = InfrastructureEndpoints.Resource + "ResourceAdmin/";
    private readonly HttpClient _client;

    public ResourceAdminControllerTest(TestServerEnvironment testServerEnvironment)
    {
        _client = testServerEnvironment.NewClient();
    }

    [Fact]
    public async Task TestSyncController()
    {
        var translationSync = new TranslationSync
        {
            // for coverage
            Resources = new List<Infrastructure.Globalisation.Model.Resource>(),
            Cultures = new List<Culture>(),
            Translations = new List<Translation>()
        };
        var response = await _client.PostAsync(Uri + "Update",
            new StringContent(JsonConvert.SerializeObject(translationSync), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TestGetFlatAllController()
    {
        var response = await _client.GetAsync(Uri + "GetFlat");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TestGetFlatFilterController()
    {
        var response = await _client.GetAsync(Uri + "GetFlat?culture=de");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public void TestController()
    {
        var response = _client.GetAsync(Uri + "DownloadResources").Result;
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var data = response.Content.ReadAsByteArrayAsync().Result;
        Assert.True(data.Length > 0, "resources found");
        response = _client.PostAsync(Uri + "UploadResources", new ByteArrayContent(data)).Result;
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ReloadTest()
    {
        var response = await _client.PostAsync(Uri + "Reload", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task CollectTest()
    {
        var response = await _client.PostAsync(Uri + "Collect", new StringContent(string.Empty, Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

}