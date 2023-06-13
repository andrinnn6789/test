using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.Common.TestHelper.Dto;
using IAG.Infrastructure.Exception.HttpException;
using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Basket.Dto;
using IAG.VinX.Basket.Enum;
using IAG.VinX.Basket.Interface;
using IAG.VinX.Globalwine.ShopNext.DataAccess;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

using Moq;

using Xunit;

namespace IAG.VinX.Globalwine.IntegrationTest.ShopNext.DataAccess;

public class OnlineClientGwTest
{
    private readonly Mock<ISybaseConnection> _sybaseMock;
    private readonly OnlineClientGw<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw> _client;
    private readonly CommonSystemDataHelper _commonSystemData = new ();
    private readonly Mock<IBasketDataReader> _mockBasketDataReader;

    public OnlineClientGwTest()
    {
        _sybaseMock = SybaseConnectionFactoryHelper.CreateConnectioMock();
        _sybaseMock.Setup(m => m.CommonSystemData).Returns(_commonSystemData);
        _sybaseMock.Setup(m => m.Insert(It.IsAny<OnlineAddressGw>())).Callback((OnlineAddressGw address) => address.Id = 1);
        _sybaseMock.Setup(c => c.Insert(It.IsAny<OnlineOrderGw>())).Callback<OnlineOrderGw>(o => o.Id = 1);
        _mockBasketDataReader = new Mock<IBasketDataReader>();
        _client = new OnlineClientGw<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw>(_sybaseMock.Object,
            _mockBasketDataReader.Object);
    }

    [Fact]
    public void NewAddressNoChangeTypeFail()
    {
        Assert.Throws<ArgumentException>(
            () => _client.NewOnlineAddress(new OnlineAddressGw()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("de")]
    [InlineData("FR")]
    [InlineData("en")]
    [InlineData("It")]
    public void NewAddressChangeTypeNewOk(string language)
    {
        _client.NewOnlineAddress(new OnlineAddressGw
        {
            ChangeType = AddressChangeType.New,
            Language = language
        });
    }

    [Fact]
    public void NewAddressCountryOk()
    {
        _sybaseMock.Setup(m => m.GetQueryable<Country>()).Returns(
            new List<Country>
            {
                new()
                {
                    Id = 1,
                    Code = "CH"
                }
            }.AsQueryable());
        _client.NewOnlineAddress(new OnlineAddressGw
        {
            ChangeType = AddressChangeType.New,
            Country = "CH"
        });
    }

    [Fact]
    public void NewAddressLanguageFail()
    {
        Assert.Throws<ArgumentException>(
            () => _client.NewOnlineAddress(new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                Language = "xx"
            }));
    }

    [Fact]
    public void NewAddressCountryFail()
    {
        Assert.Throws<ArgumentException>(
            () => _client.NewOnlineAddress(new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                Country = "xx"
            }));
    }

    [Fact]
    public void NewAddressChangeTypeNewWithAdrId()
    {
        Assert.Throws<ArgumentException>(
            () => _client.NewOnlineAddress(new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                AddressId = 1
            }));
    }

    [Fact]
    public void NewAddressChangeTypeChangeOk()
    {
        _client.NewOnlineAddress(new OnlineAddressGw
        {
            ChangeType = AddressChangeType.Change,
            AddressId = 1
        });
    }

    [Fact]
    public void NewAddressChangeTypeChangeWithoutAdrId()
    {
        Assert.Throws<ArgumentException>(
            () => _client.NewOnlineAddress(new OnlineAddressGw
            {
                ChangeType = AddressChangeType.Change
            }));
    }

    [Fact]
    public void NewContactFailNoRecord()
    {
        Assert.Throws<InvalidOperationException>(
            () => _client.UpdateContact(new ContactGw
            {
                Id = 1
            }));
    }

    [Fact]
    public void NewContactFailNoId()
    {
        Assert.Throws<ArgumentException>(
            () => _client.UpdateContact(new ContactGw()));
    }

    [Fact]
    public void NewContactFailWithId()
    {
        Assert.Throws<ArgumentException>(
            () => _client.NewContact(new ContactGw
            {
                Id = 1
            }));
    }

    [Fact]
    public void CheckAddressUniqueUsername()
    {
        _sybaseMock.Setup(m => m.GetQueryable<Address>()).Returns(
            new List<Address>
            {
                new()
                {
                    Id = 1,
                    LoginName = "a"
                },
                new()
                {
                    Id = 2,
                    LoginName = "b"
                }
            }.AsQueryable());
        _sybaseMock.Setup(m => m.GetQueryable<ContactGw>()).Returns(
            new List<ContactGw>
            {
                new()
                {
                    Id = 1,
                    LoginName = "c"
                }
            }.AsQueryable());
        _client.NewOnlineAddress(new OnlineAddressGw
        {
            ChangeType = AddressChangeType.Change,
            AddressId = 1,
            LoginName = "x"
        });
        _client.NewOnlineAddress(new OnlineAddressGw
        {
            ChangeType = AddressChangeType.Change,
            AddressId = 1,
            LoginName = "a"
        });
    }

    [Fact]
    public void CheckContactUniqueUsername()
    {
        _sybaseMock.Setup(m => m.GetQueryable<ContactGw>()).Returns(
            new List<ContactGw>
            {
                new()
                {
                    Id = 1,
                    LoginName = "a"
                },
                new()
                {
                    Id = 2,
                    LoginName = "b"
                }
            }.AsQueryable());
        _sybaseMock.Setup(m => m.GetQueryable<Address>()).Returns(
            new List<Address>
            {
                new()
                {
                    Id = 1,
                    LoginName = "c"
                }
            }.AsQueryable());
        _client.NewContact(new ContactGw
        {
            LoginName = "x"
        });
        _client.UpdateContact(new ContactGw
        {
            Id = 1
        });
        _client.UpdateContact(new ContactGw
        {
            Id = 1,
            LoginName = "a"
        });
        Assert.Throws<DuplicateKeyException>(
            () => _client.UpdateContact(new ContactGw
            {
                Id = 1,
                LoginName = "b"
            }));
    }

    [Fact]
    public void ProductDetail()
    {
        var mockBasketReader = new Mock<IBasketDataReader>();
        mockBasketReader.Setup(m => m.GetPriceBaseDataRaw()).Returns(new List<PriceBaseDataRaw>
        {
            new()
            {
                ArticleId = -1,
                CalculationMode = CustomerPriceCalculationMode.Percent,
                PromotionKind = PromotionKind.ReducePercent,
                IsPromotion = true,
                UnitPrice = 1,
                PromotionPrice = 0.9m
            }
        });
        mockBasketReader.Setup(m => m.Company).Returns(new Company());
        mockBasketReader.Setup(m => m.BasketAddress).Returns(new Address
        {
            PriceGroupId = 1,
            VatCalculation = VatCalculationType.Inclusive
        });
        mockBasketReader.Setup(m => m.Articles).Returns(new List<Article>
        {
            new()
            {
                Id = -1
            }
        });
        mockBasketReader.Setup(m => m.Stock).Returns(new List<StockData>
        {
            new()
            {
                ArticleId = -1,
                QuantityAvailable = 1,
                WarehouseId = 1
            },
            new()
            {
                ArticleId = -1,
                QuantityAvailable = 1,
                WarehouseId = 2
            },
            new()
            {
                ArticleId = 0,
                QuantityAvailable = 1,
                WarehouseId = 3
            }
        });
        _sybaseMock.Setup(m => m.GetQueryable<ArticleHelperGw>()).Returns(
            new List<ArticleHelperGw>
            {
                new()
                {
                    Id = -1
                }
            }.AsQueryable());
        var client = new OnlineClientGw<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw>(
            _sybaseMock.Object, mockBasketReader.Object);
        var details = client.GetProductDetail(new PriceParameter
        {
            ArticleParameters = new List<ArticleParameter>
            {
                new()
                {
                    ArticleId = -1,
                    Quantity = 1
                }
            },
            PriceGroupId = 3
        });
        Assert.Equal(2, details.First().Stock);

        client.GetProductDetail(new PriceParameter
        {
            ArticleParameters = new List<ArticleParameter>
            {
                new()
                {
                    ArticleId = -1,
                    Quantity = 1
                }
            },
            AddressId = 8
        });
        client.GetProductDetail(new PriceParameter
        {
            ArticleParameters = new List<ArticleParameter>
            {
                new()
                {
                    ArticleId = -1,
                    Quantity = 1
                }
            },
            AddressId = 8,
            PriceGroupId = 1
        });
        client.GetProductDetail(new PriceParameter
        {
            AddressId = 8,
            PriceGroupId = -11
        });

        mockBasketReader.Setup(m => m.GetPriceBaseDataRaw()).Returns(new List<PriceBaseDataRaw>
        {
            new()
        });
        client.GetProductDetail(new PriceParameter
        {
            AddressId = 8,
            PriceGroupId = -11
        });
    }

    [Fact] 
    public void InsertOnlineOrder()
    {
        _mockBasketDataReader.Setup(m => m.BasketAddress).Returns(
            new Address
            {
                Id = 1,
                LoginName = "c",
                VatCalculation = VatCalculationType.Inclusive
            });
        var crifCheckDate = DateTime.Today.AddMonths(-1);
        var basket = new BasketGw<OnlineAddressGw>
        {
            VatCalculation = VatCalculationType.Inclusive,
            ValidDate = DateTime.Today,
            OrderingOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New
            },
            DeliveryTime = "now",
            OrderingContactId = 11,
            CarrierId = 1,
            CrifDescription = "YELLOW",
            CrifCheckDate = crifCheckDate
        };
        var onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(onlineOrder.CarrierId, basket.CarrierId);
        Assert.Equal(onlineOrder.OrderingContactId, basket.OrderingContactId);
        Assert.Equal(onlineOrder.CrifDescription, basket.CrifDescription);
        Assert.Equal(crifCheckDate, onlineOrder.CrifCheckDate);
    }

    [Fact] 
    public void OrderText()
    {
        var adr = new Address
        {
            Id = 1,
            LoginName = "c",
            VatCalculation = VatCalculationType.Inclusive
        };
        _mockBasketDataReader.Setup(m => m.BasketAddress).Returns(adr);
        var basket = new BasketGw<OnlineAddressGw>
        {
            VatCalculation = VatCalculationType.Inclusive,
            ValidDate = DateTime.Today,
            OrderingOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New
            }
        };
        var onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Null(onlineOrder.OrderText);

        adr.InvoiceText = "receipt";
        onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Null(onlineOrder.OrderText);
        adr.InvoiceText = string.Empty;

        basket.DeliveryLocation = "info";
        onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(basket.DeliveryLocation, onlineOrder.OrderText);
        basket.DeliveryLocation = string.Empty;

        basket.OrderText = "order";
        onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(basket.OrderText, onlineOrder.OrderText);
        basket.OrderText= string.Empty;

        basket.DeliveryLocationRemark = "remark";
        onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(basket.DeliveryLocationRemark, onlineOrder.OrderText);
        basket.DeliveryLocationRemark = string.Empty;

        basket.DeliveryTime = "time";
        onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.EndsWith(basket.DeliveryTime, onlineOrder.OrderText);
        basket.DeliveryTime = string.Empty;

        adr.InvoiceText = "receipt";
        basket.OrderText = "order";
        basket.DeliveryLocation = "info";
        basket.DeliveryLocationRemark = "remark";
        basket.DeliveryTime = "time";
        onlineOrder = _client.InsertOnlineOrder(basket);
        Assert.Equal(7, onlineOrder.OrderText.Split(Environment.NewLine).Length);
    }


    [Fact]
    public void AdHocAddresses()
    {
        _mockBasketDataReader.Setup(m => m.BasketAddress).Returns(
            new Address
            {
                Id = 1,
                LoginName = "c",
                VatCalculation = VatCalculationType.Inclusive
            });
        var onlineDeliveryAddress = new OnlineAddressGw
        {
            FirstName = "sdsdsd",
            LastName = "sdsdsdsdjlsdlsd",
            AdditionalAddressLine1 = "aaaa",
            Country = "sldkfjsdlkjdkflj",
            City = "49085458j",
            Street = "wwewe",
            PhoneMobile = "343434",
            Zipcode = "123",
            ContactFirstName = "cf"
        };
        var basket = new BasketGw<OnlineAddressGw>
        {
            VatCalculation = VatCalculationType.Inclusive,
            OrderingOnlineAddress = new OnlineAddressGw
            {
                ChangeType = AddressChangeType.New,
                ShopId = "12"
            },
            DeliveryOnlineAddress = onlineDeliveryAddress
        };
        var onlineOrder = _client.InsertOnlineOrder(basket);

        Assert.NotEmpty(onlineOrder.AdHocZip);
        Assert.NotEmpty(onlineOrder.AdHocAddress);
        Assert.NotEmpty(onlineOrder.AdHocStreet);
        Assert.NotEmpty(onlineOrder.AdHocCity);
        Assert.NotEmpty(onlineOrder.AdHocCountry);
        Assert.NotEmpty(onlineOrder.AdHocPhoneNumber);
        Assert.Null(onlineOrder.DeliveryOnlineAddressId);
        Assert.NotNull(onlineOrder.OrderingOnlineAddressId);
    }

    [Fact]
    public void ValidateAddresseNoOrdering()
    {
        _commonSystemData.MainVersion = 2000;
        _mockBasketDataReader.Setup(m => m.BasketAddress).Returns(
            new Address
            {
                Id = 1,
                LoginName = "c",
                VatCalculation = VatCalculationType.Inclusive
            });
        var basket = new BasketGw<OnlineAddressGw>
        {
            VatCalculation = VatCalculationType.Inclusive,
            DeliveryAddressId = 2, 
            BillingAddressId = 3
        };

        Assert.Throws<BadRequestException>(() => _client.InsertOnlineOrder(basket));
    }
}