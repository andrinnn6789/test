using System.Linq;

using IAG.VinX.Basket.Dto;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Mapper;

public class BasketPositionMapperFromShop : PositionBaseMapperFromShop<BasketPositionGw, BasketPosition>
{
    protected override BasketPosition MapToDestination(BasketPositionGw source, BasketPosition destination)
    {
        base.MapToDestination(source, destination);
        destination.StockQuantity = source.StockQuantity;

        return destination;
    }
}

public class BasketPositionMapperToShop : PositionBaseMapperToShop<BasketPosition, BasketPositionGw>
{
    protected override BasketPositionGw MapToDestination(BasketPosition source, BasketPositionGw destination)
    {
        var chargePositionMapper = new PositionBaseMapperToShop<ChargePosition, ChargePositionGw>();
        var discountPositionMapper = new PositionBaseMapperToShop<DiscountPosition, DiscountPositionGw>();
        var packagePositionMapper = new PositionBaseMapperToShop<PackagePosition, PackagePositionGw>();

        base.MapToDestination(source, destination);
        destination.StockQuantity = source.StockQuantity;
            
        destination.Charges = source.Charges.Select(pos => chargePositionMapper.NewDestination(pos)).ToList();
        destination.Discounts = source.Discounts.Select(pos => discountPositionMapper.NewDestination(pos)).ToList();
        destination.Packages = source.Packages.Select(pos => packagePositionMapper.NewDestination(pos)).ToList();
        destination.ChargeTotalAmount = source.ChargeTotalAmount;
        destination.DiscountTotalAmount = source.DiscountTotalAmount;
        destination.PackagesTotalAmount = source.PackagesTotalAmount;
        destination.GrandTotalAmount = source.GrandTotalAmount;
        destination.TaxTotalAmount = source.TaxTotalAmount;

        return destination;
    }
}