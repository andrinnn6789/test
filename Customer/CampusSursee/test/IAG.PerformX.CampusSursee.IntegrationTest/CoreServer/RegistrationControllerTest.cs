using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.TestHelper.Startup;
using IAG.PerformX.CampusSursee.Dto.Address;
using IAG.PerformX.CampusSursee.Dto.Registration;
using Newtonsoft.Json;

using Xunit;

namespace IAG.PerformX.CampusSursee.IntegrationTest.CoreServer;

[Collection("CampusSurseeController")]
public class RegistrationControllerTest : BaseControllerTest
{
    public RegistrationControllerTest(TestServerEnvironment testEnvironment) : base(testEnvironment, "Registration")
    {
    }

    [Fact]
    public async Task PostAddressChangeAndGet()
    {
        var item = new RegistrationAddress
        {
            LastName = "test",
            AddressTypeId = 2,
            Foto = await File.ReadAllBytesAsync(Path.Combine("Dto", "Capture.jpg"))
        };
        var response = await Client.PostAsync($"{Url}AddressChange",
            new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var change = JsonConvert.DeserializeObject<RegistrationAddress>(await response.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(change);
        Assert.True(change.Id > 0);
        response = await Client.GetAsync($"{Url}Address({change.Id})");
        response.EnsureSuccessStatusCode();
        var itemGet = JsonConvert.DeserializeObject<RegistrationAddress>(await response.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(itemGet);
        Assert.Equal(itemGet.Id, change.Id);
        response = await Client.GetAsync($"{Url}Address?$filter=id eq ({change.Id})");
        response.EnsureSuccessStatusCode();
        itemGet = JsonConvert.DeserializeObject<List<RegistrationAddress>>(await response.Content.ReadAsStringAsync())?.FirstOrDefault();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(itemGet);
        Assert.Equal(itemGet.Id, change.Id);
        Connection.Delete(change);
    }

    [Fact]
    public async Task PostAddressChangeFail()
    {
        var item = new RegistrationAddress
        {
            LastName = "test",
            AddressTypeId = 2,
            Language = "xx"
        };
        var response = await Client.PostAsync($"{Url}AddressChange",
            new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostAddressNewAndGet()
    {
        var item = new RegistrationAddress
        {
            LastName = "test",
            AddressTypeId = 2,
            SbvIsMember = 10,
            UserName = Guid.NewGuid().ToString(),
            Foto = await File.ReadAllBytesAsync(Path.Combine("Dto", "Capture.jpg"))
        };
        var response = await Client.PostAsync($"{Url}AddressNew",
            new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var inserted = JsonConvert.DeserializeObject<RegistrationAddress>(await response.Content.ReadAsStringAsync());
        try
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(inserted);
            Assert.True(inserted.Id > 0);
            response = await Client.GetAsync($"{Url}Address({inserted.Id})");
            response.EnsureSuccessStatusCode();
            var itemGet = JsonConvert.DeserializeObject<RegistrationAddress>(await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(itemGet);
            Assert.Equal(itemGet.Id, inserted.Id);
        }
        finally
        {
            Connection.Delete(inserted);
        }
    }

    [Fact]
    public async Task PostAddressNewDataFail()
    {
        var item = new RegistrationAddress
        {
            LastName = "test",
            AddressTypeId = 2,
            Language = "xx"
        };
        var response = await Client.PostAsync($"{Url}AddressNew",
            new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostAddressNewUsernameFail()
    {
        var item = new RegistrationAddress
        {
            LastName = "test",
            AddressTypeId = 2,
            Language = "de",
            SbvIsMember = 10,
            UserName = "it's me from controller"
        };
        var response = await Client.PostAsync($"{Url}AddressNew",
            new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var inserted = JsonConvert.DeserializeObject<RegistrationAddress>(await response.Content.ReadAsStringAsync());
        try
        {
            response = await Client.PostAsync($"{Url}AddressNew",
                new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var inserted2 = JsonConvert.DeserializeObject<RegistrationAddress>(await response.Content.ReadAsStringAsync());
            Connection.Delete(inserted2);
        }
        finally
        {
            Connection.Delete(inserted);
        }
    }

    [Fact]
    public void GetAddressFail()
    {
        Assert.Equal(HttpStatusCode.NotFound, Client.GetAsync($"{Url}Address(-1)").Result.StatusCode);
    }

    [Fact]
    public async Task PostRegistration()
    {
        var item = new RegistrationPending
        {
            EventId = 123,
            OnWaitingList = true
        };
        var response = await Client.PostAsync($"{Url}Registration",
            new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var inserted = JsonConvert.DeserializeObject<RegistrationPending>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(inserted);
        Assert.Equal(HttpStatusCode.OK, Client.GetAsync($"{Url}Registration({inserted.Id})").Result.StatusCode);
        Assert.True(inserted.OnWaitingList);
        Connection.Delete(inserted);
    }

    [Fact]
    public void PostRegistrationFail()
    {
        Assert.Equal(HttpStatusCode.NotFound, Client.GetAsync($"{Url}Registration(-1)").Result.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, Client.PostAsync($"{Url}Registration",
            new StringContent(JsonConvert.SerializeObject(new RegistrationPending()), Encoding.UTF8, "application/json")).Result.StatusCode);
    }

    [Fact]
    public async Task PostRegistrationWithAddress()
    {
        var address = ChangedOnAndTake1Get<Address>();
        var registrationWithAddress = new RegistrationWithAddress
        {
            Registration = new RegistrationPending
            {
                EventId = 123
            },
            RegistrationAddress = new RegistrationAddress
            {
                LastName = "test",
                AddressTypeId = 2,
                UserName = Guid.NewGuid().ToString(),
                ChangeType = AddressChangeTypeEnum.New
            },
            RegistrationBillingAddress = new RegistrationAddress
            {
                LastName = "test b",
                AddressId = address.Id,
                ChangeType = AddressChangeTypeEnum.Change
            }
        };
        var response = await Client.PostAsync($"{Url}RegistrationWithAddress",
            new StringContent(JsonConvert.SerializeObject(registrationWithAddress), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var inserted = JsonConvert.DeserializeObject<RegistrationPending>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(inserted);
        Assert.Equal(HttpStatusCode.OK, Client.GetAsync($"{Url}Registration({inserted.Id})").Result.StatusCode);
        Connection.Delete(inserted);
        Connection.Delete(Connection.GetQueryable<RegistrationAddress>().First(a => a.Id == inserted.RegistrationAddressId));
        Connection.Delete(Connection.GetQueryable<RegistrationAddress>().First(a => a.Id == inserted.RegistrationBillingAddressId));
    }

    [Fact]
    public async Task PostRegistrationWithAddressNop()
    {
        var address = ChangedOnAndTake1Get<Address>();
        var registrationWithAddress = new RegistrationWithAddress
        {
            Registration = new RegistrationPending
            {
                EventId = 123
            },
            RegistrationAddress = new RegistrationAddress
            {
                AddressId = address.Id,
                ChangeType = AddressChangeTypeEnum.Nop
            }
        };
        var response = await Client.PostAsync($"{Url}RegistrationWithAddress",
            new StringContent(JsonConvert.SerializeObject(registrationWithAddress), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var inserted = JsonConvert.DeserializeObject<RegistrationPending>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(inserted);
        Assert.Equal(HttpStatusCode.OK, Client.GetAsync($"{Url}Registration({inserted.Id})").Result.StatusCode);
        Connection.Delete(inserted);
    }
}