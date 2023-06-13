using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.ControlCenter.Mobile.BusinessLayer.Model;
using IAG.ControlCenter.Mobile.DataLayer.Model;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.TestHelper.Startup;

using Newtonsoft.Json;

using Xunit;

namespace IAG.ControlCenter.IntegrationTest.ControlCenter.Mobile.CoreServer;

[Collection("TestEnvironmentCollection")]
public class MobileLicenceAppControllerTest
{
    private const string Uri = ControlCenterEndpoints.Mobile + "LicenceApp/";
    private readonly HttpClient _client;

    public MobileLicenceAppControllerTest(TestServerEnvironment testEnvironment)
    {
        _client = testEnvironment.NewClient();
    }

    [Fact]
    public async Task RequestToken()
    {
        var response = await _client.PostAsync(Uri + "RequestToken",
            new StringContent(JsonConvert.SerializeObject(
                    new SimpleRequestTokenParameter
                    {
                        Username = "me"
                    }),
                Encoding.UTF8, "application/json"));
        // when auth-plugin is disabled response is not found
        Assert.True(HttpStatusCode.Forbidden == response.StatusCode || HttpStatusCode.NotFound == response.StatusCode);
    }

    [Fact]
    public async Task TestCheck()
    {
        var licRequest = new LicenceRequest();
        var response = await _client.PostAsync(Uri + "Check",
            new StringContent(JsonConvert.SerializeObject(licRequest), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var licResponse = JsonConvert.DeserializeObject<LicenceResponse>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(licResponse);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(LicenceStatusAppEnum.Invalid, licResponse.LicenceStatus);
    }

    [Fact]
    public async Task TestFree()
    {
        var licRequest = new LicenceRequest();
        var response = await _client.PostAsync(Uri + "Free",
            new StringContent(JsonConvert.SerializeObject(licRequest), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var licResponse = JsonConvert.DeserializeObject<LicenceFreeResponse>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(licResponse);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(LicenceStatusAppEnum.Invalid, licResponse.LicenceStatus);
    }

    [Fact]
    public async Task TestGetFlatAllController()
    {
        var response = await _client.GetAsync(Uri + "GetFlat");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TestControllerRequestToken400()
    {
        var response = await _client.PostAsync(Uri + "RequestToken",
            new StringContent("bad", Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TestControllerCheck400()
    {
        var response = await _client.PostAsync(Uri + "Check",
            new StringContent("bad", Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TestControllerFree400()
    {
        var response = await _client.PostAsync(Uri + "Free",
            new StringContent("bad", Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}