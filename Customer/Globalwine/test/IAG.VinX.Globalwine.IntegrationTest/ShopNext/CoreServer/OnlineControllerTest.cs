using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.TestHelper.Startup;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Globalwine.IntegrationTest.CoreServer;
using IAG.VinX.Globalwine.ShopNext.Dto.Enum;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

using Newtonsoft.Json;

using Xunit;

namespace IAG.VinX.Globalwine.IntegrationTest.ShopNext.CoreServer;

[Collection("GlobalwineController")]
public class OnlineControllerTest : TestControllerBase
{
    public OnlineControllerTest(TestServerEnvironment testEnvironment) : base(testEnvironment, "Online")
    {
    }

    [Fact]
    public async Task PostOnlineAddressNewAndGet()
    {
        var onlineAddress = new OnlineAddressGw
        {
            LastName = "test",
            ChangeType = Basket.Enum.AddressChangeType.New
        };
        var response = await Client.PostAsync(
            $"{Url}OnlineAddress",
            new StringContent(
                JsonConvert.SerializeObject(onlineAddress), 
                Encoding.UTF8, 
                "application/json"));
        response.EnsureSuccessStatusCode();
        onlineAddress = JsonConvert.DeserializeObject<OnlineAddressGw>(await response.Content.ReadAsStringAsync());
        try
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(onlineAddress);
            Assert.True(onlineAddress.Id > 0);
            response = await Client.GetAsync(
                $"{Url}OnlineAddress({onlineAddress.Id})");
            response.EnsureSuccessStatusCode();
            var contactGet = JsonConvert.DeserializeObject<OnlineAddressGw>(await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(contactGet);
            Assert.Equal(contactGet.Id, onlineAddress.Id);
            Assert.Equal(contactGet.ChangeType, onlineAddress.ChangeType);
            response = await Client.PutAsync(
                $"{Url}OnlineAddress({-1})",
                new StringContent(
                    JsonConvert.SerializeObject(onlineAddress), 
                    Encoding.UTF8, 
                    "application/json"));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally
        {
            Connection.Delete(onlineAddress);
        }
    }

    [Fact]
    public async Task PostEnptyOnlineAddress()
    {
        var response = await Client.PostAsync(
            $"{Url}OnlineAddress", 
            new StringContent(
                string.Empty, 
                Encoding.UTF8, 
                "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostBasket()
    {
        var article = Connection.GetQueryable<ArticleGw>().First();
        var basket = new BasketRestGw
        {
            Action= BasketActionTypeGw.Calculate,
            ConditionAddressId = 1, 
            DeliveryConditionId = 2, 
            DeliveryTime = "now",
            OrderingContactId = 1,
            Positions = new List<BasketPositionGw>
            {
                new()
                {
                    ArticleId = article.Id,
                    OrderedQuantity = 1,
                    PriceKind = PriceCalculationKindGw.FromShop
                }
            }
        };
        var response = await Client.PostAsync(
            $"{Url}Basket",
            new StringContent(
                JsonConvert.SerializeObject(basket), 
                Encoding.UTF8, 
                "application/json"));
        response.EnsureSuccessStatusCode();
        var basketRet = JsonConvert.DeserializeObject<BasketRestGw>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(basketRet);
        Assert.Equal("now", basketRet.DeliveryTime);
        Assert.Equal(1, basketRet.OrderingContactId);
    }

    [Fact]
    public async Task PostProductDetailTest()
    {
        var address = Connection.GetQueryable<Address>().First();
        var article = Connection.GetQueryable<ArticleGw>().First(a => a.ActiveShop);
        var response = await Client.PostAsync(
            $"{Url}ProductDetail",
            new StringContent(
                JsonConvert.SerializeObject(
                    new PriceParameterGw
                    {
                        AddressId = address.Id,
                        PriceGroupId = null,
                        Division = null,
                        ArticleParameters = new List<ArticleParameterGw>
                        {
                            new()
                            {
                                ArticleId = article.Id,
                                Quantity = 1
                            }
                        }
                    }),
                Encoding.UTF8, 
                "application/json"));
        response.EnsureSuccessStatusCode();
        var products = JsonConvert.DeserializeObject<List<ProductDetailGw>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(products);
        Assert.NotEmpty(products);
    }
}