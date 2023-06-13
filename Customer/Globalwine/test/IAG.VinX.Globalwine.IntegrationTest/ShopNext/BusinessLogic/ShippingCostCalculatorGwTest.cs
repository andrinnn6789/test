using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Exception.HttpException;
using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Basket.Dto;
using IAG.VinX.Basket.Enum;
using IAG.VinX.Globalwine.ShopNext.BusinessLogic;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;

using Moq;

using Xunit;

using DeliveryConditionGw = IAG.VinX.Globalwine.ShopNext.Dto.Rest.DeliveryConditionGw;

namespace IAG.VinX.Globalwine.IntegrationTest.ShopNext.BusinessLogic;

public class ShippingCostCalculatorGwTest
{
    private readonly ShippingCostCalculatorGw _shippingCostCalculator;


    public ShippingCostCalculatorGwTest()
    {
        #region Dummy Data

        var freightCosts = new List<FreightCost>
        {
            new()
            {
                Id = 1,
                ArticleId = 1,
                ArticleName = "Transportkosten Privat",
                FreightExemptFrom = 100,
                PriceGroupId = 1,
                PriceGroupName = "Privat"
            },
            new()
            {
                Id = 2,
                ArticleId = 2,
                ArticleName = "Transportkosten Gastronomie",
                FreightExemptFrom = 200,
                PriceGroupId = 2,
                PriceGroupName = "Gastronomie"
            },
            new()
            {
                Id = 3,
                ArticleId = 3,
                ArticleName = "Transportkosten Fachhandel",
                FreightExemptFrom = 300,
                PriceGroupId = 3,
                PriceGroupName = "Fachhandel"
            },
            new()
            {
                Id = 4,
                ArticleId = 4,
                ArticleName = "Transportkosten Grosshandel",
                FreightExemptFrom = 400,
                PriceGroupId = 4,
                PriceGroupName = "Grosshandel"
            }
        };

        var addresses = new List<Address>
        {
            new()
            {
                LastName = "Privat",
                Id = 1,
                PriceGroupId = 1,
                VatCalculation = VatCalculationType.Inclusive
            },
            new()
            {
                LastName = "Gastronomie",
                Id = 2,
                PriceGroupId = 2,
                VatCalculation = VatCalculationType.Exclusive
            },
            new()
            {
                LastName = "Fachhandel",
                Id = 3,
                PriceGroupId = 3,
                VatCalculation = VatCalculationType.Exclusive
            },
            new()
            {
                LastName = "Grosshandel",
                Id = 4,
                PriceGroupId = 4,
                VatCalculation = VatCalculationType.Exclusive
            },
            new()
            {
                LastName = "vat not def",
                Id = 5,
                PriceGroupId = 4,
                VatCalculation = VatCalculationType.NotDefined
            }
        };

        var deliveryConditions = new List<DeliveryConditionGw>
        {
            new()
            {
                Name = "Abholer",
                Id = 1,
                NoShippingCost = true
            },
            new()
            {
                Name = "Spedi",
                Id = 2,
                NoShippingCost = false
            }
        };

        #endregion

        var sybaseConnection = new Mock<ISybaseConnection>();
        sybaseConnection.Setup(m => m.GetQueryable<FreightCost>()).Returns(() => freightCosts.AsQueryable());
        sybaseConnection.Setup(m => m.GetQueryable<Address>()).Returns(() => addresses.AsQueryable());
        sybaseConnection.Setup(m => m.GetQueryable<DeliveryConditionGw>()).Returns(() => deliveryConditions.AsQueryable());
        sybaseConnection.Setup(m => m.GetQueryable<ArticleHelperGw>()).Returns(() => new List<ArticleHelperGw>
        {
            new ()
            {
                Id = 1,
                FreigthFree = true
            }
        }.AsQueryable());
        _shippingCostCalculator =
            new ShippingCostCalculatorGw(sybaseConnection.Object);
    }

    [InlineData(VatCalculationType.Exclusive)]
    [InlineData(VatCalculationType.Inclusive)]
    [Theory]
    public void ShippingCostCalculator(VatCalculationType vatCalculation)
    {
        var basket = new BasketGw<OnlineAddressGw>
        {
            VatCalculation = vatCalculation,
            ConditionAddressId = 1,
            DeliveryConditionId = 2,
            Positions = new List<BasketPosition>
            {
                new()
                {
                    UnitPrice = 10,
                    ApplicableTaxRate = 7.7m,
                    OrderedQuantity = 5,
                    BilledQuantity = 5,
                    BasketParams = new BasketParams
                    {
                        VatCalculation = VatCalculationType.Inclusive
                    }
                }
            },
            Charges = new List<ChargePosition>
            {
                new()
                {
                    UnitPrice = 10,
                    ApplicableTaxRate = 5,
                    PosType = OrderPositionType.Shipping
                }
            }

        };
        var charge = _shippingCostCalculator.Calculate<OnlineAddressGw, BasketGw<OnlineAddressGw>>(basket);
        Assert.NotEmpty(charge);
    }

    [Fact]
    public void ShippingCostCalculatorPickUp()
    {
        var charge = _shippingCostCalculator
            .Calculate<OnlineAddressGw, BasketGw<OnlineAddressGw>>(new BasketGw<OnlineAddressGw>
            {
                ConditionAddressId = 1,
                DeliveryConditionId = 1,
                Positions = new List<BasketPosition>
                {
                    new()
                    {
                        UnitPrice = 10,
                        ApplicableTaxRate = 7.7m,
                        OrderedQuantity = 5,
                        BilledQuantity = 5
                    }
                }
            });
        Assert.Empty(charge);
    }

    [Fact]
    public void ShippingFree()
    {
        var basket = new BasketGw<OnlineAddressGw>
        {
            ConditionAddressId = 1,
            DeliveryConditionId = 2,
            Positions = new List<BasketPosition>
            {
                new()
                {
                    UnitPrice = 10,
                    ApplicableTaxRate = 7.7m,
                    OrderedQuantity = 5,
                    BilledQuantity = 5,
                    ArticleId = 1
                }
            }
        };
        var charge = _shippingCostCalculator.Calculate<OnlineAddressGw, BasketGw<OnlineAddressGw>>(basket);
        Assert.Empty(charge);
    }

    [Fact]
    public void NoAddressFound()
    {
        Assert.Throws<BadRequestException>(()=> _shippingCostCalculator
            .Calculate<OnlineAddressGw, BasketGw<OnlineAddressGw>>(new BasketGw<OnlineAddressGw>()));
    }

    [Fact]
    public void NoDeliveryConditionFound()
    {
        Assert.Throws<BadRequestException>(()=> _shippingCostCalculator
            .Calculate<OnlineAddressGw, BasketGw<OnlineAddressGw>>(new BasketGw<OnlineAddressGw>
            {
                ConditionAddressId = 1,
                DeliveryConditionId = 0
            }));
    }
}