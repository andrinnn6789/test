using System;
using System.Collections.Generic;
using System.Linq;

using IAG.VinX.Basket.Dto;
using IAG.VinX.Basket.Enum;
using IAG.VinX.Globalwine.ShopNext.Dto.Enum;
using IAG.VinX.Globalwine.ShopNext.Dto.Mapper;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

using Xunit;

namespace IAG.VinX.Globalwine.IntegrationTest.ShopNext.Dto.Mapper;

public class BasketPositionMapperGwTest
{
    [Fact]
    public void FromTest()
    {
        var mapper = new BasketPositionMapperFromShop();
        var source = new BasketPositionGw
        {
            PosType = OrderPositionTypeGw.OrderPos,
            PosNumber = "source.PosNumber",
            ArticleId = 1,
            OrderedQuantity = 2.3m,
            BilledQuantity = 2.2m,
            ArticleNumber = "source.ArticleNumber",
            Description = "source.Description",
            PriceKind = PriceCalculationKindGw.FromShop,
            UnitPrice = 3.45m,
            UnitPriceWithTax = 4.5m,
            ApplicableTaxRate = 8.1m,
            StockQuantity = 123,
        };

        var destination = mapper.NewDestination(source);

        Assert.NotNull(destination);
        Assert.Equal((int)source.PosType, (int)destination.PosType);
        Assert.Equal(source.PosNumber, destination.PosNumber);
        Assert.Equal(source.ArticleId, destination.ArticleId);
        Assert.Equal(source.OrderedQuantity, destination.OrderedQuantity);
        Assert.Equal(source.BilledQuantity, destination.BilledQuantity);
        Assert.Equal(source.ArticleNumber, destination.ArticleNumber);
        Assert.Equal(source.Description, destination.Description);
        Assert.Equal((int)source.PriceKind, (int)destination.PriceKind);
        Assert.True(destination.UnitPrice > 0);
        Assert.True(destination.UnitPriceWithTaxCalculated > 0);
        Assert.Equal(source.ApplicableTaxRate, destination.ApplicableTaxRate);
        Assert.Equal(source.StockQuantity, destination.StockQuantity);
    }

    [Fact]
    public void ToTest()
    {
        var mapper = new BasketPositionMapperToShop();
        var source = new BasketPosition()
        {
            PosType = OrderPositionType.OrderPos,
            PosNumber = "source.PosNumber",
            ArticleId = 1,
            OrderedQuantity = 2.3m,
            BilledQuantity = 2.2m,
            ArticleNumber = "source.ArticleNumber",
            Description = "source.Description",
            PriceKind = PriceCalculationKind.FromShop,
            UnitPrice = 3.45m,
            UnitPriceWithTaxFromExternal = 4.5m,
            ApplicableTaxRate = 8.1m,
            StockQuantity = 123,
            Packages = new List<PackagePosition> {new()},
            Discounts = new List<DiscountPosition> {new()},
            Charges = new List<ChargePosition> {new()}
        };

        var destination = mapper.NewDestination(source);

        Assert.NotNull(destination);
        Assert.Equal((int)source.PosType, (int)destination.PosType);
        Assert.Equal(source.PosNumber, destination.PosNumber);
        Assert.Equal(source.ArticleId, destination.ArticleId);
        Assert.Equal(source.OrderedQuantity, destination.OrderedQuantity);
        Assert.Equal(source.BilledQuantity, destination.BilledQuantity);
        Assert.Equal(source.ArticleNumber, destination.ArticleNumber);
        Assert.Equal(source.Description, destination.Description);
        Assert.Equal((int)source.PriceKind, (int)destination.PriceKind);
        Assert.True(destination.UnitPrice > 0);
        Assert.True(destination.UnitPriceWithTax > 0);
        Assert.Equal(source.ApplicableTaxRate, destination.ApplicableTaxRate);
        Assert.Equal(source.StockQuantity, destination.StockQuantity);
        Assert.Single(destination.Charges);
    }

    [Fact]
    public void PriceKindMappingFromTest()
    {
        var sources = Enum.GetValues<PriceCalculationKindGw>().Select(priceKind => new BasketPositionGw()
        {
            PosType = OrderPositionTypeGw.OrderPos,
            PriceKind = priceKind
        }).ToList();
        var destinations = sources.Select(source => new BasketPositionMapperFromShop().NewDestination(source)).ToList();

        for (var i = 0; i < sources.Count; i++)
        {
            Assert.Equal((int)sources[i].PriceKind, (int)destinations[i].PriceKind);
        }
    }


    [Fact]
    public void PriceKindMappingToTest()
    {
        var sources = Enum.GetValues<PriceCalculationKind>().Select(priceKind => new BasketPosition
        {
            PosType = OrderPositionType.OrderPos,
            PriceKind = priceKind
        }).ToList();
        _ = sources.Select(source => new BasketPositionMapperToShop().NewDestination(source)).ToList();
    }

    [Fact]
    public void EnumExceptionTest()
    {
        var sourceInvalidPosType = new BasketPosition
        {
            PosType = 0
        };
        var sourceGwInvalidPosType = new BasketPositionGw
        {
            PosType = 0
        };
        var sourceInvalidPriceKind = new BasketPosition
        {
            PosType = OrderPositionType.Shipping,
            PriceKind = 0
        };
        var sourceGwInvalidPriceKind = new BasketPositionGw
        {
            PosType = OrderPositionTypeGw.Shipping,
            PriceKind = 0
        };

        Assert.Throws<NotSupportedException>(() => new BasketPositionMapperToShop().NewDestination(sourceInvalidPosType));
        _ = new BasketPositionMapperFromShop().NewDestination(sourceGwInvalidPosType);
        Assert.Throws<NotSupportedException>(() => new BasketPositionMapperToShop().NewDestination(sourceInvalidPriceKind));
        _ = new BasketPositionMapperFromShop().NewDestination(sourceGwInvalidPriceKind);
    }
}