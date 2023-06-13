using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.TestHelper.Startup;

using Newtonsoft.Json;

using Xunit;

namespace IAG.PerformX.CampusSursee.IntegrationTest.CoreServer;

[Collection("CampusSurseeController")]
public class AuthenticationControllerTest : BaseControllerTest
{
    public AuthenticationControllerTest(TestServerEnvironment testEnvironment) : base(testEnvironment, "Authentication")
    {
    }

    [Fact]
    public async Task AddressesGet()
    {
        var item = RequestTokenParameter.ForPasswordGrant("test", "test").ForRealm("test");

        var response = await Client.PostAsync($"{Url}RequestToken",
            new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}