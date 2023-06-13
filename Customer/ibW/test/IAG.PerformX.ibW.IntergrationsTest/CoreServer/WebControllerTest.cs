using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.TestHelper.Startup;
using IAG.PerformX.ibW.Dto.Web;

using Newtonsoft.Json;

using Xunit;

namespace IAG.PerformX.ibW.IntergrationsTest.CoreServer;

public class WebControllerTest : BaseControllerTest
{
    public WebControllerTest(TestServerEnvironment testEnvironment): 
        base(testEnvironment, "api/Core/" + SwaggerEndpointProvider.ApiEndpoint + "/Web/")
    {
    }

    [Fact]
    public async Task RequestTokenTest()
    {
        var item = RequestTokenParameter.ForPasswordGrant("test", "test").ForRealm("test");

        var response = await Client.PostAsync($"{EndPoint}RequestToken",
            new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AngebotGet()
    {
        await ChangedOnAndTake1Get<Angebot>();
    }

    [Fact]
    public async Task ECommerceGet()
    {
        await ChangedOnAndTake1Get<ECommerce>("Lookup/");
    }
}