using System;

using IAG.Infrastructure.ObjectMapper;
using IAG.VinX.Basket.Dto;
using IAG.VinX.Globalwine.ShopNext.Dto.Enum;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Mapper;

public class PositionBaseMapperFromShop<TPosShop, TPos> : ObjectMapper<TPosShop, TPos>
    where TPosShop: PositionBaseGw
    where TPos: PositionBase, new()
{
    protected override TPos MapToDestination(TPosShop source, TPos destination)
    {
        destination.PosNumber = source.PosNumber;
        destination.ArticleId = source.ArticleId;
        destination.OrderedQuantity = source.OrderedQuantity;
        destination.BilledQuantity = source.BilledQuantity;
        destination.ArticleNumber = source.ArticleNumber;
        destination.Description = source.Description;
        destination.UnitPriceWithTaxFromExternal = source.UnitPriceWithTax;
        destination.UnitPrice = source.UnitPrice;
        destination.ApplicableTaxRate = source.ApplicableTaxRate;

        destination.PosType = source.PosType switch
        {
            OrderPositionTypeGw.OrderPos => Basket.Enum.OrderPositionType.OrderPos,
            OrderPositionTypeGw.Discount => Basket.Enum.OrderPositionType.Discount,
            OrderPositionTypeGw.Charge => Basket.Enum.OrderPositionType.Charge,
            OrderPositionTypeGw.Shipping => Basket.Enum.OrderPositionType.Shipping,
            OrderPositionTypeGw.Package => Basket.Enum.OrderPositionType.Package,
            _ => 0
        };

        destination.PriceKind = source.PriceKind switch
        {
            PriceCalculationKindGw.Customer => Basket.Enum.PriceCalculationKind.Customer,
            PriceCalculationKindGw.Promotion => Basket.Enum.PriceCalculationKind.Promotion,
            PriceCalculationKindGw.PriceGroup => Basket.Enum.PriceCalculationKind.PriceGroup,
            PriceCalculationKindGw.Article => Basket.Enum.PriceCalculationKind.Article,
            PriceCalculationKindGw.Override => Basket.Enum.PriceCalculationKind.Override,
            PriceCalculationKindGw.FromShop => Basket.Enum.PriceCalculationKind.FromShop,
            PriceCalculationKindGw.NotFound => Basket.Enum.PriceCalculationKind.NotFound,
            _ => 0
        };

        return destination;
    }
}

public class PositionBaseMapperToShop<TPos, TPosShop> : ObjectMapper<TPos, TPosShop>
    where TPos : PositionBase
    where TPosShop : PositionBaseGw, new()
{
    protected override TPosShop MapToDestination(TPos source, TPosShop destination)
    {
        destination.PosNumber = source.PosNumber;
        destination.ArticleId = source.ArticleId;
        destination.OrderedQuantity = source.OrderedQuantity;
        destination.BilledQuantity = source.BilledQuantity;
        destination.ArticleNumber = source.ArticleNumber;
        destination.Description = source.Description;
        destination.UnitPrice = source.UnitPrice;
        destination.UnitPriceWithTax = source.UnitPriceWithTaxCalculated;
        destination.ApplicableTaxRate = source.ApplicableTaxRate;

        destination.LineTotalAmount = source.LineTotalAmount;
        destination.LineCumulativeTotalAmount = source.LineCumulativeTotalAmount;
        destination.TaxBaseTotalAmount = source.TaxBaseTotalAmount;
        destination.TaxTotalAmount = source.TaxTotalAmount;
        destination.GrandTotalAmount = source.GrandTotalAmount;

        destination.PosType = source.PosType switch
        {
            Basket.Enum.OrderPositionType.OrderPos => OrderPositionTypeGw.OrderPos,
            Basket.Enum.OrderPositionType.Discount => OrderPositionTypeGw.Discount,
            Basket.Enum.OrderPositionType.Charge => OrderPositionTypeGw.Charge,
            Basket.Enum.OrderPositionType.Shipping => OrderPositionTypeGw.Shipping,
            Basket.Enum.OrderPositionType.Package => OrderPositionTypeGw.Package,
            Basket.Enum.OrderPositionType.Fee => OrderPositionTypeGw.Charge,
            _ => throw new NotSupportedException($"Enum value '{source.PosType}' is not supported")
        };

        destination.PriceKind = source.PriceKind switch
        {
            Basket.Enum.PriceCalculationKind.Customer => PriceCalculationKindGw.Customer,
            Basket.Enum.PriceCalculationKind.Promotion => PriceCalculationKindGw.Promotion,
            Basket.Enum.PriceCalculationKind.PriceGroup => PriceCalculationKindGw.PriceGroup,
            Basket.Enum.PriceCalculationKind.Article => PriceCalculationKindGw.Article,
            Basket.Enum.PriceCalculationKind.Override => PriceCalculationKindGw.Override,
            Basket.Enum.PriceCalculationKind.Calculated => PriceCalculationKindGw.Override,
            Basket.Enum.PriceCalculationKind.FromShop => PriceCalculationKindGw.FromShop,
            Basket.Enum.PriceCalculationKind.NotFound => PriceCalculationKindGw.NotFound,
            _ => throw new NotSupportedException($"Enum value '{source.PriceKind}' is not supported")
        };

        return destination;
    }
}