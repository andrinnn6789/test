using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.TestHelper.Startup;
using IAG.PerformX.CampusSursee.Dto.Address;

using Newtonsoft.Json;

using Xunit;

namespace IAG.PerformX.CampusSursee.IntegrationTest.CoreServer;

[Collection("CampusSurseeController")]
public class AddressControllerTest : BaseControllerTest
{
    public AddressControllerTest(TestServerEnvironment testEnvironment) : base(testEnvironment, "Address")
    {
    }

    [Fact]
    public async Task AddressesGet()
    {
        await ChangedOnAndTake1Get<Address>();
    }

    [Fact]
    public async Task PostAddressChangeUserName()
    {
        var response = await Client.GetAsync($"{Url}{nameof(Address)}?%24top=1&$filter=username ne null");
        response.EnsureSuccessStatusCode();
        var firstAddress = JsonConvert.DeserializeObject<List<Address>>(await response.Content.ReadAsStringAsync())?.First();
        Assert.NotNull(firstAddress);
        var item = new AddressChangeParam
        {
            UserName = firstAddress.UserName
        };
        response = await Client.PostAsync($"{Url}Address({firstAddress.Id})/ChangeUserName",
            new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        response = await Client.PostAsync($"{Url}Address({firstAddress.Id})/ChangeUserName",
            new StringContent(JsonConvert.SerializeObject(new AddressChangeParam()), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        item = new AddressChangeParam
        {
            UserName = "xx"
        };
        response = await Client.PostAsync($"{Url}Address(-1)/ChangeUserName",
            new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DocumentsGet()
    {
        await ChangedOnAndTake1Get<Document>();
    }

    [Fact]
    public async Task RelationsGet()
    {
        await ChangedOnAndTake1Get<Relation>();
    }

    [Fact]
    public async Task RegistrationsGet()
    {
        await ChangedOnAndTake1Get<Registration>();
    }
}