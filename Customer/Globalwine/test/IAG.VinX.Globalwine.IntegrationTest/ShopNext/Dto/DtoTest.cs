using System;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.DataLayerSybase;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Basket.Enum;
using IAG.VinX.Globalwine.ShopNext.Dto.Enum;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

using Xunit;

using DeliveryConditionGw = IAG.VinX.Globalwine.ShopNext.Dto.Rest.DeliveryConditionGw;
using ProducerGw = IAG.VinX.Globalwine.ShopNext.Dto.Rest.ProducerGw;

namespace IAG.VinX.Globalwine.IntegrationTest.ShopNext.Dto;

public class DtoTest
{
    private readonly ISybaseConnection _connection;

    public DtoTest()
    {
        _connection = new SybaseConnectionFactory(
            new ExplicitUserContext("test", null),
            new MockILogger<SybaseConnection>(),
            Infrastructure.Startup.Startup.BuildConfig(),
            null).CreateConnection();
    }

    [Fact]
    public void AddressGwVxTest()
    {
        var items = _connection.GetQueryable<AddresShopId>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void ContactsWithAddressFlatTest()
    {
        var items = _connection.GetQueryable<ContactWithAddressFlat>().Take(1).ToList();
        Assert.NotEmpty(items);
        Assert.NotNull(items[0].Id);
        Assert.True(items[0].KpAddressId > 0);
    }

    [Fact]
    public void ContactReadAndUpdateTest()
    {
        SybaseTransctionHelper.ExecuteInRollbackTransaction(_connection, () =>
        {
            var items = _connection.GetQueryable<ContactGw>().Take(1).ToList();
            Assert.NotEmpty(items);
            var contact = items[0];
            var guid = Guid.NewGuid().ToString();
            contact.LastName = guid;
            _connection.Update(contact);
            var contactUpd = _connection.GetQueryable<ContactGw>().First(c => c.Id == contact.Id);
            Assert.Equal(guid, contactUpd.LastName);
        });
    }

    [Fact]
    public void ContactInsertTest()
    {
        SybaseTransctionHelper.ExecuteInRollbackTransaction(_connection, () =>
        {
            var address = _connection.GetQueryable<ContactWithAddressFlat>().Take(1).First();
            var item = new ContactGw
            {
                FirstName = "first",
                AddressId = address.AdId
            };
            _connection.Insert(item);
            Assert.True(item.Id > 0);
        });
    }

    [Fact]
    public void ArticleTest()
    {
        var items = _connection.GetQueryable<ArticleGw>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void CountryTest()
    {
        var items = _connection.GetQueryable<Country>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void SalutationTest()
    {
        var items = _connection.GetQueryable<SalutationGw>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void RecommendationTest()
    {
        var items = _connection.GetQueryable<RecommendationGw>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void RatingTest()
    {
        var items = _connection.GetQueryable<RatingGw>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void CompositionTest()
    {
        var items = _connection.GetQueryable<CompositionGw>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void PriceTest()
    {
        var items = _connection.GetQueryable<PriceGw>().Take(1).ToList();
        Assert.NotEmpty(items);
        _ = items[0].ChangedOn;
    }

    [Fact]
    public void ProducerTest()
    {
        var items = _connection.GetQueryable<ProducerGw>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void OnlineAddressGwTest()
    {
        _ = _connection.GetQueryable<OnlineAddressGw>().Take(1).ToList();
    }

    [Fact]
    public void OnlineAddressGwInsertTest()
    {
        SybaseTransctionHelper.ExecuteInRollbackTransaction(_connection, () =>
        {
            var item = new OnlineAddressGw
            {
                FirstName = "first",
                ChangeType = AddressChangeType.New, 
                CustomerCategory = AddressstructureGw.Private
            };
            _connection.Insert(item);
            Assert.True(item.Id > 0);
            Assert.Equal(AddressstructureGw.Private, item.CustomerCategory);
        });
    }

    [Fact]
    public void DeliveryConditionTest()
    {
        var items = _connection.GetQueryable<DeliveryConditionGw>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void OnlineOrderGwTest()
    {
        _ = _connection.GetQueryable<OnlineOrderGw>().Take(1).ToList();
    }

    [Fact]
    public void OnlineOrderGwInsertTest()
    {
        var carrier = _connection.GetQueryable<Carrier>().First();
        var deliveryCondition = _connection.GetQueryable<DeliveryConditionGw>().First();
        SybaseTransctionHelper.ExecuteInRollbackTransaction(_connection, () =>
        {
            var item = new OnlineOrderGw
            {
                OrderText = "Test",
                CarrierId = carrier.Id,
                DeliveryConditionId = deliveryCondition.Id,
                HasAdhoc = true, 
                CrifDescription = "xx"
            };
            _connection.Insert(item);
            Assert.True(item.Id > 0);
            Assert.True(item.HasAdhoc);
        });
    }

    [Fact]
    public void FreightCostGwTest()
    {
        var items = _connection.GetQueryable<FreightCost>().ToList();
        Assert.NotEmpty(items);
        _ = items[0].Id;
        Assert.NotNull(items[0].PriceGroupName);
        _ = items[0].ArticleName;
    }
}