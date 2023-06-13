using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.Exception.HttpException;
using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Basket.Enum;
using IAG.VinX.Basket.Interface;
using IAG.VinX.Globalwine.ShopNext.DataAccess;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;

using Moq;

using Xunit;

namespace IAG.VinX.Globalwine.IntegrationTest.ShopNext.DataAccess;

public class OnlintClientGwTestShopIds
{
    private readonly Mock<ISybaseConnection> _sybaseMock;
    private readonly OnlineClientGw<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw> _client;
    private readonly Address _address = new()
    {
        Id = 1,
        LoginName = "c",
        VatCalculation = VatCalculationType.Inclusive
    };

    public OnlintClientGwTestShopIds()
    {
        _sybaseMock = SybaseConnectionFactoryHelper.CreateConnectioMock();
        _sybaseMock.Setup(m => m.Insert(It.IsAny<OnlineAddressGw>())).Callback((OnlineAddressGw address) 
            => address.Id = string.IsNullOrWhiteSpace(address.ShopId) ? 9 : Convert.ToInt32(address.ShopId));
        _sybaseMock.Setup(c => c.Insert(It.IsAny<OnlineOrderGw>())).Callback<OnlineOrderGw>(o => o.Id = 1);
        _sybaseMock.Setup(m => m.GetQueryable<AddresShopId>()).Returns(
            new List<AddresShopId>
            {
                new()
                {
                    Id = 2,
                    ShopId = "22"
                },
                new()
                {
                    Id = 3,
                    ShopId = "33"
                }
            }.AsQueryable());
        _sybaseMock.Setup(m => m.GetQueryable<Address>()).Returns(
            new List<Address>
            {
                new()
                {
                    Id = 1
                },
                new()
                {
                    Id = 9
                }
            }.AsQueryable());
        _sybaseMock.Setup(m => m.GetQueryable<OnlineAddressGw>()).Returns(
            new List<OnlineAddressGw>
            {
                new()
                {
                    Id = 4,
                    ShopId = "44"
                },
                new()
                {
                    Id = 5,
                    ShopId = "55"
                }
            }.AsQueryable());
        var mockBasketDataReader = new Mock<IBasketDataReader>();
        mockBasketDataReader.Setup(m => m.BasketAddress).Returns(_address);
        _client = new OnlineClientGw<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw>(_sybaseMock.Object,
            mockBasketDataReader.Object);
    }

    [Fact]
    public void GuestOrder()
    {
        var basket = new BasketGw<OnlineAddressGw>
        {
            Action = BasketActionType.Calculate,
            VatCalculation = VatCalculationType.Inclusive,
            OrderingOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "11"
            }
        };
        var onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(11, onlineOrder.OrderingOnlineAddressId);
        Assert.Null(onlineOrder.DeliveryOnlineAddressId);
        Assert.Null(onlineOrder.BillingOnlineAddressId);
        Assert.Null(onlineOrder.OrderingAddressId);
        Assert.Null(onlineOrder.DeliveryAddressId);
        Assert.Null(onlineOrder.BillingAddressId);
    }

    [Fact]
    public void NewCustomerOrder()
    {
        var basket = new BasketGw<OnlineAddressGw>
        {
            Action = BasketActionType.Calculate,
            VatCalculation = VatCalculationType.Inclusive,
            OrderingOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "11",
                LoginName = "11"
            }
        };
        var onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(11, onlineOrder.OrderingOnlineAddressId);
        Assert.Null(onlineOrder.DeliveryOnlineAddressId);
        Assert.Null(onlineOrder.BillingOnlineAddressId);
        Assert.Null(onlineOrder.OrderingAddressId);
        Assert.Null(onlineOrder.DeliveryAddressId);
        Assert.Null(onlineOrder.BillingAddressId);
    }

    [Fact]
    public void GuestOrderDel()
    {
        var basket = new BasketGw<OnlineAddressGw>
        {
            Action = BasketActionType.Calculate,
            VatCalculation = VatCalculationType.Inclusive,
            OrderingOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "11"
            },
            DeliveryOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "12"
            }
        };
        var onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(11, onlineOrder.OrderingOnlineAddressId);
        Assert.True(onlineOrder.HasAdhoc);
        Assert.Null(onlineOrder.DeliveryOnlineAddressId);
        Assert.Null(onlineOrder.BillingOnlineAddressId);
        Assert.Null(onlineOrder.OrderingAddressId);
        Assert.Null(onlineOrder.DeliveryAddressId);
        Assert.Null(onlineOrder.BillingAddressId);
    }

    [Fact]
    public void GuestOrderBil()
    {
        var basket = new BasketGw<OnlineAddressGw>
        {
            Action = BasketActionType.Calculate,
            VatCalculation = VatCalculationType.Inclusive,
            OrderingOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "11"
            },
            BillingOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "13"
            }
        };
        var onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(11, onlineOrder.OrderingOnlineAddressId);
        Assert.False(onlineOrder.HasAdhoc);
        Assert.Null(onlineOrder.DeliveryOnlineAddressId);
        Assert.NotNull(onlineOrder.BillingOnlineAddressId);
        Assert.Null(onlineOrder.OrderingAddressId);
        Assert.Null(onlineOrder.DeliveryAddressId);
        Assert.Null(onlineOrder.BillingAddressId);
    }

    [Fact]
    public void GuestOrderBilExisting()
    {
        var basket = new BasketGw<OnlineAddressGw>
        {
            Action = BasketActionType.Calculate,
            VatCalculation = VatCalculationType.Inclusive,
            OrderingOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "11"
            },
            BillingOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "33"
            }
        };
        Assert.ThrowsAny<Exception>(() =>  _client.InsertOnlineOrder(basket));
    }

    [Fact]
    public void GuestOrderDelBil()
    {
        var basket = new BasketGw<OnlineAddressGw>
        {
            Action = BasketActionType.Calculate,
            VatCalculation = VatCalculationType.Inclusive,
            OrderingOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "11"
            },
            DeliveryOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "12"
            },
            BillingOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "13"
            }
        };
        var onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(11, onlineOrder.OrderingOnlineAddressId);
        Assert.True(onlineOrder.HasAdhoc);
        Assert.Null(onlineOrder.DeliveryOnlineAddressId);
        Assert.Equal(13, onlineOrder.BillingOnlineAddressId);
        Assert.Null(onlineOrder.OrderingAddressId);
        Assert.Null(onlineOrder.DeliveryAddressId);
        Assert.Null(onlineOrder.BillingAddressId);
    }

    [Fact]
    public void ExistingAddress()
    {
        var basket = new BasketGw<OnlineAddressGw>
        {
            Action = BasketActionType.Calculate,
            VatCalculation = VatCalculationType.Inclusive,
            OrderingAddressId = 1
        };
        var onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(1, onlineOrder.OrderingAddressId);
        Assert.Null(onlineOrder.OrderingOnlineAddressId);
        Assert.Null(onlineOrder.DeliveryAddressId);
    }

    [Fact]
    public void ExistingAddressBil()
    {
        var basket = new BasketGw<OnlineAddressGw>
        {
            Action = BasketActionType.Calculate,
            VatCalculation = VatCalculationType.Inclusive,
            OrderingAddressId = 1,
            BillingAddressId = 2
        };
        var onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(1, onlineOrder.OrderingAddressId);
        Assert.Equal(2, onlineOrder.BillingAddressId);
        Assert.Null(onlineOrder.DeliveryAddressId);
    }

    [Fact]
    public void ExistingAddressDel()
    {
        var basket = new BasketGw<OnlineAddressGw>
        {
            Action = BasketActionType.Calculate,
            VatCalculation = VatCalculationType.Inclusive,
            OrderingAddressId = 1,
            DeliveryAddressId = 2
        };
        var onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(1, onlineOrder.OrderingAddressId);
        Assert.Equal(2, onlineOrder.DeliveryAddressId);
        Assert.Null(onlineOrder.BillingAddressId);
    }

    [Fact]
    public void ExistingAddressBilDel()
    {
        var basket = new BasketGw<OnlineAddressGw>
        {
            Action = BasketActionType.Calculate,
            VatCalculation = VatCalculationType.Inclusive,
            OrderingAddressId = 1,
            DeliveryAddressId = 2,
            BillingAddressId = 3
        };
        var onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(1, onlineOrder.OrderingAddressId);
        Assert.Equal(3, onlineOrder.BillingAddressId);
        Assert.Equal(2, onlineOrder.DeliveryAddressId);
    }

    [Fact]
    public void ExistingOnlineAddress()
    {
        var basket = new BasketGw<OnlineAddressGw>
        {
            Action = BasketActionType.Calculate,
            VatCalculation = VatCalculationType.Inclusive,
            OrderingOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "22"
            }
        };
        var onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(2, onlineOrder.OrderingAddressId);
        Assert.Null(onlineOrder.OrderingOnlineAddressId);
    }

    [Fact]
    public void ExistingOnlineAddressDelB2B()
    {
        _address.VatCalculation = VatCalculationType.Exclusive;
        var basket = new BasketGw<OnlineAddressGw>
        {
            Action = BasketActionType.Calculate,
            VatCalculation = VatCalculationType.Inclusive,
            OrderingAddressId = 1,
            DeliveryOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "55"
            }
        };
        var onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(1, onlineOrder.OrderingAddressId);
        Assert.False(onlineOrder.HasAdhoc);
        Assert.Equal(5, onlineOrder.DeliveryOnlineAddressId);
    }

    [Fact]
    public void ExistingOnlineAddressDelB2C()
    {
        var basket = new BasketGw<OnlineAddressGw>
        {
            Action = BasketActionType.Calculate,
            VatCalculation = VatCalculationType.Inclusive,
            OrderingAddressId = 1,
            DeliveryOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New
            }
        };
        var onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(1, onlineOrder.OrderingAddressId);
        Assert.True(onlineOrder.HasAdhoc);
        Assert.Null(onlineOrder.DeliveryOnlineAddressId);
    }

    [Fact]
    public void ExistingOnlineAddressDelBil()
    {
        _address.VatCalculation = VatCalculationType.Exclusive;
        var basket = new BasketGw<OnlineAddressGw>
        {
            Action = BasketActionType.Calculate,
            VatCalculation = VatCalculationType.Inclusive,
            OrderingAddressId = 1,
            DeliveryOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "55"
            },
            BillingOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "44"
            }
        };
        var onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(1, onlineOrder.OrderingAddressId);
        Assert.Null(onlineOrder.OrderingOnlineAddressId);
        Assert.Equal(5, onlineOrder.DeliveryOnlineAddressId);
    }

    [Fact]
    public void DuplicateShopId()
    {
        // duplicate shopId
        _sybaseMock.Setup(m => m.GetQueryable<AddresShopId>()).Returns(
            new List<AddresShopId>
            {
                new()
                {
                    Id = 2,
                    ShopId = "22"
                },
                new()
                {
                    Id = 3,
                    ShopId = "22"
                }
            }.AsQueryable());
        var basket = new BasketGw<OnlineAddressGw>
        {
            Action = BasketActionType.Calculate,
            VatCalculation = VatCalculationType.Inclusive,
            OrderingOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "22"
            }
        };
        basket.OrderingOnlineAddress.ShopId = "22";
        Assert.Throws<DuplicateKeyException>(() => _client.InsertOnlineOrder(basket));
    }
}