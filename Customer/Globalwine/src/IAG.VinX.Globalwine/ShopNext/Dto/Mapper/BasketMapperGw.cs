using System;
using System.Linq;

using IAG.Infrastructure.ObjectMapper;
using IAG.VinX.Basket.Dto;
using IAG.VinX.Globalwine.ShopNext.Dto.Enum;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Mapper;

public class BasketMapperFromShop: ObjectMapper<BasketRestGw, BasketGw<OnlineAddressGw>>
{
    protected override BasketGw<OnlineAddressGw> MapToDestination(BasketRestGw source, BasketGw<OnlineAddressGw> destination)
    {
        var addressMapper = new OnlineAddressMapperFromShop();
        var positionMapper = new BasketPositionMapperFromShop();
        var chargePositionMapper = new PositionBaseMapperFromShop<ChargePositionGw, ChargePosition>();
        var discountPositionMapper = new PositionBaseMapperFromShop<DiscountPositionGw, DiscountPosition>();
        var packagePositionMapper = new PositionBaseMapperFromShop<PackagePositionGw, PackagePosition>();

        destination.Id = source.Id;
        destination.AmountPayed = source.AmountPayed;
        destination.BillingAddressId = source.BillingAddressId;
        destination.BillingOnlineAddress = source.BillingOnlineAddress != null ? addressMapper.NewDestination(source.BillingOnlineAddress) : null;
        destination.Charges = source.Charges?.Select(pos => chargePositionMapper.NewDestination(pos)).ToList();
        destination.ConditionAddressId = source.ConditionAddressId;
        destination.DeliveryAddressId = source.DeliveryAddressId;
        destination.DeliveryConditionId = source.DeliveryConditionId;
        destination.DeliveryDateRequested = source.DeliveryDateRequested;
        destination.DeliveryAddressId = source.DeliveryAddressId;
        destination.DeliveryOnlineAddress = source.DeliveryOnlineAddress != null ? addressMapper.NewDestination(source.DeliveryOnlineAddress) : null;
        destination.Discounts = source.Discounts?.Select(pos => discountPositionMapper.NewDestination(pos)).ToList();
        destination.DivisionId = source.DivisionId;
        destination.OrderText = source.OrderText;
        destination.OrderingAddressId = source.OrderingAddressId;
        destination.OrderingOnlineAddress = source.OrderingOnlineAddress != null ? addressMapper.NewDestination(source.OrderingOnlineAddress) : null;
        destination.Packages = source.Packages?.Select(pos => packagePositionMapper.NewDestination(pos)).ToList();
        destination.PaymentConditionId = source.PaymentConditionId;
        destination.Positions = source.Positions?.Select(pos => positionMapper.NewDestination(pos)).ToList();
        destination.SaferPayId = source.SaferPayId;
        destination.SaferPayToken = source.SaferPayToken;
        destination.ShopId = source.ShopId;
        destination.ValidDate = source.ValidDate;
        destination.ProviderId = source.ProviderId;

        destination.OrderingContactId = source.OrderingContactId;
        destination.DeliveryTime = source.DeliveryTime;
        destination.DeliveryLocation = source.DeliveryLocation;
        destination.DeliveryLocationRemark = source.DeliveryLocationRemark;
        destination.CarrierId = source.CarrierId;
        destination.CrifDescription = source.CrifDescription;
        destination.CrifCheckDate = source.CrifCheckDate;

        destination.Action = source.Action switch
        {
            BasketActionTypeGw.Order => Basket.Enum.BasketActionType.Order,
            BasketActionTypeGw.Calculate => Basket.Enum.BasketActionType.Calculate,
            _ => throw new NotSupportedException($"Enum value '{source.Action}' is not supported")
        };

        return destination;
    }
}

public class BasketMapperToShop: ObjectMapper<BasketGw<OnlineAddressGw>, BasketRestGw>
{
    protected override BasketRestGw MapToDestination(BasketGw<OnlineAddressGw> source, BasketRestGw destination)
    {
        var addressMapper = new OnlineAddressMapperToShop();
        var positionMapper = new BasketPositionMapperToShop();
        var chargePositionMapper = new PositionBaseMapperToShop<ChargePosition, ChargePositionGw>();
        var discountPositionMapper = new PositionBaseMapperToShop<DiscountPosition, DiscountPositionGw>();
        var packagePositionMapper = new PositionBaseMapperToShop<PackagePosition, PackagePositionGw>();

        destination.Id = source.Id;
        destination.AmountPayed = source.AmountPayed;
        destination.BillingAddressId = source.BillingAddressId;
        destination.BillingOnlineAddress = source.BillingOnlineAddress != null ? addressMapper.NewDestination(source.BillingOnlineAddress) : null;
        destination.Charges = source.Charges?.Select(pos => chargePositionMapper.NewDestination(pos)).ToList();
        destination.ConditionAddressId = source.ConditionAddressId;
        destination.DeliveryConditionId = source.DeliveryConditionId;
        destination.DeliveryDateRequested = source.DeliveryDateRequested;
        destination.DeliveryAddressId = source.DeliveryAddressId;
        destination.DeliveryOnlineAddress = source.DeliveryOnlineAddress != null ? addressMapper.NewDestination(source.DeliveryOnlineAddress) : null;
        destination.Discounts = source.Discounts?.Select(pos => discountPositionMapper.NewDestination(pos)).ToList();
        destination.DivisionId = source.DivisionId;
        destination.OrderText = source.OrderText;
        destination.OrderingAddressId = source.OrderingAddressId;
        destination.OrderingOnlineAddress = source.OrderingOnlineAddress != null ? addressMapper.NewDestination(source.OrderingOnlineAddress) : null;
        destination.Packages = source.Packages?.Select(pos => packagePositionMapper.NewDestination(pos)).ToList();
        destination.PaymentConditionId = source.PaymentConditionId;
        destination.Positions = source.Positions?.Select(pos => positionMapper.NewDestination(pos)).ToList();
        destination.SaferPayId = source.SaferPayId;
        destination.SaferPayToken = source.SaferPayToken;
        destination.ShopId = source.ShopId;
        destination.ValidDate = source.ValidDate;
        destination.ProviderId = source.ProviderId;

        destination.ChargeTotalAmount = source.ChargeTotalAmount;
        destination.DiscountTotalAmount = source.DiscountTotalAmount;
        destination.GrandTotalAmount = source.GrandTotalAmount;
        destination.LineTotalAmount = source.LineTotalAmount;
        destination.LineCumulativeTotalAmount = source.LineCumulativeTotalAmount;
        destination.PackagesTotalAmount = source.PackagesTotalAmount;
        destination.PaymentTerms = source.PaymentTerms;
        destination.RoundingAmount = source.RoundingAmount;
        destination.TaxBaseTotalAmount = source.TaxBaseTotalAmount;
        destination.TaxTotalAmount = source.TaxTotalAmount;
        destination.VatCalculation = source.VatCalculation;

        destination.OrderingContactId = source.OrderingContactId;
        destination.DeliveryTime = source.DeliveryTime;
        destination.DeliveryLocation = source.DeliveryLocation;
        destination.DeliveryLocationRemark = source.DeliveryLocationRemark;
        destination.CarrierId = source.CarrierId;
        destination.CrifDescription = source.CrifDescription;
        destination.CrifCheckDate = source.CrifCheckDate;

        destination.Action = source.Action switch
        {
            Basket.Enum.BasketActionType.Order => BasketActionTypeGw.Order,
            Basket.Enum.BasketActionType.Calculate => BasketActionTypeGw.Calculate,
            _ => throw new NotSupportedException($"Enum value '{source.Action}' is not supported")
        };

        return destination;
    }
}