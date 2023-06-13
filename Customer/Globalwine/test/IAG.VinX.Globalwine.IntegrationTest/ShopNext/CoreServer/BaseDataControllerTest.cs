using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.TestHelper.Startup;
using IAG.VinX.Globalwine.IntegrationTest.CoreServer;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

using Newtonsoft.Json;

using Xunit;

using DeliveryConditionGw = IAG.VinX.Globalwine.ShopNext.Dto.Rest.DeliveryConditionGw;
using ProducerGw = IAG.VinX.Globalwine.ShopNext.Dto.Rest.ProducerGw;

namespace IAG.VinX.Globalwine.IntegrationTest.ShopNext.CoreServer;

[Collection("GlobalwineController")]
public class BaseDataControllerTest : TestControllerBase
{
    public BaseDataControllerTest(TestServerEnvironment testEnvironment) : base(testEnvironment, "BaseData")
    {
    }

    [Fact]
    public async Task ArticleGet()
    {
        var response = await Client.GetAsync($"{Url}Article?%24top=100");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ContactWithAddresstGet()
    {
        await ChangedOnAndTake1Get<ContactWithAddress>();
    }

    [Fact]
    public async Task ContactGet()
    {
        await ChangedOnAndTake1Get<ContactGw>("Contact");
    }

    [Fact]
    public async Task CruAddress()
    {
        var address = Connection.GetQueryable<ContactWithAddressFlat>().First();
        var contact = new ContactGw
        {
            LastName = "test",
            AddressId= address.AdId
        };
        var response = await Client.PostAsync($"{Url}Contact",
            new StringContent(JsonConvert.SerializeObject(contact), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        contact = JsonConvert.DeserializeObject<ContactGw>(await response.Content.ReadAsStringAsync());
        try
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(contact);
            Assert.True(contact.Id > 0);
            response = await Client.GetAsync($"{Url}Contact({contact.Id})");
            response.EnsureSuccessStatusCode();
            var contactGet = JsonConvert.DeserializeObject<ContactGw>(await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(contactGet);
            Assert.Equal(contactGet.Id, contact.Id);
            contact.FirstName = "updated";
            response = await Client.PutAsync($"{Url}Contact({contact.Id})",
                new StringContent(JsonConvert.SerializeObject(contact), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            contactGet = JsonConvert.DeserializeObject<ContactGw>(await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(contactGet);
            Assert.Equal(contactGet.FirstName, contact.FirstName);
            response = await Client.PutAsync($"{Url}Contact({-1})",
                new StringContent(JsonConvert.SerializeObject(contact), Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally
        {
            Connection.Delete(contact);
        }
    }


    [Fact]
    public async Task ProducerGet()
    {
        await ChangedOnAndTake1Get<ProducerGw>("Producer");
    }

    [Fact]
    public async Task PaymentConditionGet()
    {
        await Take1<PaymentConditionGw>("PaymentCondition");
    }

    [Fact]
    public async Task DeliveryConditionGet()
    {
        await Take1<DeliveryConditionGw>("DeliveryCondition");
    }

    [Fact]
    public async Task SalutationGet()
    {
        await Take1<SalutationGw>("Salutation");
    }

    [Fact]
    public async Task CarrierGet()
    {
        await Take1<CarrierGw>("Carrier");
    }
}