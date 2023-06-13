using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.ControlCenter.Mobile.BusinessLayer.Model;
using IAG.ControlCenter.Mobile.DataLayer.Model;
using IAG.Infrastructure.TestHelper.Startup;

using Newtonsoft.Json;

using Xunit;

namespace IAG.ControlCenter.IntegrationTest.ControlCenter.Mobile.CoreServer;

[Collection("TestEnvironmentCollection")]
public class MobileLicenceSyncControllerTest
{
    private const string Uri = ControlCenterEndpoints.Mobile + "LicenceSync/";
    private readonly HttpClient _client;

    public MobileLicenceSyncControllerTest(TestServerEnvironment testEnvironment)
    {
        _client = testEnvironment.NewClient();
    }

    [Fact]
    public async Task TestControllerSync()
    {
        var licRequest = new LicenceSync();
        var response = await _client.PostAsync(Uri + "Sync",
            new StringContent(JsonConvert.SerializeObject(licRequest), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var licResponse = JsonConvert.DeserializeObject<List<MobileLicence>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(licResponse);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(licResponse);
    }

    [Fact]
    public async Task TestControllerUpdate()
    {
        var licRequest = new LicenceSync();
        var response = await _client.PostAsync(Uri + "Update",
            new StringContent(JsonConvert.SerializeObject(licRequest), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TestControllerSync400()
    {
        var response = await _client.PostAsync(Uri + "Sync",
            new StringContent("bad", Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TestControllerUpdate400()
    {
        var response = await _client.PostAsync(Uri + "Update",
            new StringContent("bad", Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}