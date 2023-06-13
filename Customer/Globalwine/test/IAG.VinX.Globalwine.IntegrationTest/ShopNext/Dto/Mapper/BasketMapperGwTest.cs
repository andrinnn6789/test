using System;
using System.Collections.Generic;

using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.Basket.Dto;
using IAG.VinX.Basket.Enum;
using IAG.VinX.Globalwine.ShopNext.Dto.Enum;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;
using IAG.VinX.Globalwine.ShopNext.Dto.Mapper;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

using Xunit;

namespace IAG.VinX.Globalwine.IntegrationTest.ShopNext.Dto.Mapper;

public class BasketMapperGwTest
{
    [Fact]
    public void FromTest()
    {
        var mapper = new BasketMapperFromShop();
        var source = new BasketRestGw
        {
            Id = 1,
            Action = BasketActionTypeGw.Order,
            AmountPayed = 123.45m,
            BillingAddressId = 2,
            BillingOnlineAddress = new OnlineAddressRestGw()
            {
                ChangeType = AddressChangeTypeGw.Nop
            },
            Charges = new List<ChargePositionGw>
            {
                new()
                {
                    PriceKind = PriceCalculationKindGw.Article
                },
                new()
                {
                    PriceKind = PriceCalculationKindGw.Article,
                    PosType = OrderPositionTypeGw.Charge
                }
            },
            ConditionAddressId = 4,
            DeliveryConditionId = 5,
            DeliveryDateRequested = DateTime.Today,
            DeliveryAddressId = 6,
            DeliveryOnlineAddress = null,
            Discounts = new List<DiscountPositionGw>
            {
                new()
                {
                    PriceKind = PriceCalculationKindGw.Article
                }
            },
            DivisionId = 7,
            OrderText = "source.OrderText",
            OrderingAddressId = null,
            OrderingOnlineAddress = null,
            Packages = new List<PackagePositionGw>
            {
                new()
                {
                    PriceKind = PriceCalculationKindGw.Article
                }
            },
            PaymentConditionId = 8,
            Positions = new List<BasketPositionGw>
            {
                new()
                {
                    PriceKind = PriceCalculationKindGw.Article
                }
            },
            SaferPayId = "source.SaferPayId",
            SaferPayToken = "source.SaferPayToken",
            ShopId = "source.ShopId",
            DeliveryTime = "now",
            DeliveryLocation = "del loc",
            DeliveryLocationRemark = "del loc remark",
            ValidDate = DateTime.Today.AddDays(1)
        };

        var destination = mapper.NewDestination(source);

        Assert.NotNull(destination);
        Assert.Equal(source.Id, destination.Id);
        Assert.Equal((int)source.Action, (int)destination.Action);
        Assert.Equal(source.AmountPayed, destination.AmountPayed);
        Assert.Equal(source.BillingAddressId, destination.BillingAddressId);
        Assert.Equal(2, destination.Charges.Count);
        Assert.Equal(source.ConditionAddressId, destination.ConditionAddressId);
        Assert.Equal(source.DeliveryConditionId, destination.DeliveryConditionId);
        Assert.Equal(source.DeliveryDateRequested, destination.DeliveryDateRequested);
        Assert.Equal(source.DeliveryAddressId, destination.DeliveryAddressId);
        Assert.Null(destination.DeliveryOnlineAddress);
        Assert.Single(destination.Discounts);
        Assert.Equal(source.DivisionId, destination.DivisionId);
        Assert.Equal(source.OrderText, destination.OrderText);
        Assert.Equal(source.OrderingAddressId, destination.OrderingAddressId);
        Assert.Null(destination.OrderingOnlineAddress);
        Assert.Single(destination.Packages);
        Assert.Equal(source.PaymentConditionId, destination.PaymentConditionId);
        Assert.Single(destination.Positions);
        Assert.Equal(source.SaferPayId, destination.SaferPayId);
        Assert.Equal(source.SaferPayToken, destination.SaferPayToken);
        Assert.Equal(source.ShopId, destination.ShopId);
        Assert.Equal(source.ValidDate, destination.ValidDate);
        Assert.Equal(source.DeliveryTime, destination.DeliveryTime);
        Assert.Equal(source.DeliveryLocation, destination.DeliveryLocation);
        Assert.Equal(source.DeliveryLocationRemark, destination.DeliveryLocationRemark);
    }

    [Fact]
    public void ToTest()
    {
        var mapper = new BasketMapperToShop();
        var source = new BasketGw<OnlineAddressGw>
        {
            Id = 1,
            Action = BasketActionType.Order,
            AmountPayed = 123.45m,
            BillingAddressId = 2,
            BillingOnlineAddress = new OnlineAddressGw()
            {
                ChangeType = AddressChangeType.Nop
            },
            Charges = new List<ChargePosition>
            {
                new()
                {
                    UnitPrice = 23.45m,
                    BilledQuantity = 2,
                    ApplicableTaxRate = 8.1m,
                    PriceKind = PriceCalculationKind.Article
                },
                new()
                {
                    UnitPrice = 23.45m,
                    BilledQuantity = 2,
                    ApplicableTaxRate = 8.1m,
                    PriceKind = PriceCalculationKind.Article,
                    PosType = OrderPositionType.Fee
                }
            },
            ConditionAddressId = 4,
            DeliveryConditionId = 5,
            DeliveryDateRequested = DateTime.Today,
            DeliveryAddressId = 6,
            DeliveryOnlineAddress = null,
            Discounts = new List<DiscountPosition>
            {
                new()
                {
                    BilledQuantity = 1m,
                    UnitPrice = 4.56m,
                    PriceKind = PriceCalculationKind.Article

                }
            },
            DivisionId = 7,
            OrderText = "source.OrderText",
            OrderingAddressId = null,
            OrderingOnlineAddress = null,
            Packages = new List<PackagePosition>
            {
                new()
                {
                    BilledQuantity = 1m,
                    UnitPrice = 4.5m,
                    PriceKind = PriceCalculationKind.Article

                }
            },
            PaymentConditionId = 8,
            Positions = new List<BasketPosition>
            {
                new()
                {
                    BilledQuantity = 1m,
                    UnitPrice = 5.6m,
                    PriceKind = PriceCalculationKind.Article
                }
            },
            SaferPayId = "source.SaferPayId",
            SaferPayToken = "source.SaferPayToken",
            ShopId = "source.ShopId",
            ValidDate = DateTime.Today.AddDays(1)
        };

        var destination = mapper.NewDestination(source);

        Assert.NotNull(destination);
        Assert.Equal(source.Id, destination.Id);
        Assert.Equal((int)source.Action, (int)destination.Action);
        Assert.Equal(source.AmountPayed, destination.AmountPayed);
        Assert.Equal(source.BillingAddressId, destination.BillingAddressId);
        Assert.Equal(2, destination.Charges.Count);
        Assert.Equal(source.ConditionAddressId, destination.ConditionAddressId);
        Assert.Equal(source.DeliveryConditionId, destination.DeliveryConditionId);
        Assert.Equal(source.DeliveryDateRequested, destination.DeliveryDateRequested);
        Assert.Equal(source.DeliveryAddressId, destination.DeliveryAddressId);
        Assert.Null(destination.DeliveryOnlineAddress);
        Assert.Single(destination.Discounts);
        Assert.Equal(source.DivisionId, destination.DivisionId);
        Assert.Equal(source.OrderText, destination.OrderText);
        Assert.Equal(source.OrderingAddressId, destination.OrderingAddressId);
        Assert.Null(destination.OrderingOnlineAddress);
        Assert.Single(destination.Packages);
        Assert.Equal(source.PaymentConditionId, destination.PaymentConditionId);
        Assert.Single(destination.Positions);
        Assert.Equal(source.SaferPayId, destination.SaferPayId);
        Assert.Equal(source.SaferPayToken, destination.SaferPayToken);
        Assert.Equal(source.ShopId, destination.ShopId);
        Assert.Equal(source.ValidDate, destination.ValidDate);

        Assert.True(source.ChargeTotalAmount > 0);
        Assert.True(source.DiscountTotalAmount > 0);
        Assert.True(source.GrandTotalAmount > 0);
        Assert.True(source.LineTotalAmount > 0);
        Assert.True(source.PackagesTotalAmount > 0);
        Assert.Null(source.PaymentTerms);
        Assert.True(source.RoundingAmount == 0);
        Assert.True(source.TaxBaseTotalAmount > 0);
        Assert.True(source.TaxTotalAmount > 0);
        Assert.Equal(VatCalculationType.NotDefined, source.VatCalculation);
    }

    [Fact]
    public void EnumExceptionTest()
    {
        var source = new BasketRestGw()
        {
            Action = 0
        };
        var sourceGw = new BasketGw<OnlineAddressGw>()
        {
            Action = 0
        };

        Assert.Throws<NotSupportedException>(() => new BasketMapperFromShop().NewDestination(source));
        Assert.Throws<NotSupportedException>(() => new BasketMapperToShop().NewDestination(sourceGw));
    }
}