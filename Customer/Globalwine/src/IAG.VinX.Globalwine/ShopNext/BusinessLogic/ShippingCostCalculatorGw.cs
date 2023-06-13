using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Exception.HttpException;
using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Basket.Dto;
using IAG.VinX.Basket.Enum;
using IAG.VinX.Basket.Interface;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

namespace IAG.VinX.Globalwine.ShopNext.BusinessLogic;

public class ShippingCostCalculatorGw : IShippingCostCalculator
{
    private readonly ISybaseConnection _connection;

    public ShippingCostCalculatorGw(ISybaseConnection connection)
    {
        _connection = connection;
    }

    public IList<ChargePosition> Calculate<TAddress, TBasket>(TBasket basket) where TAddress : OnlineAddress, new() where TBasket : Basket<TAddress>
    {
        var address = _connection.GetQueryable<Address>().FirstOrDefault(x => x.Id == basket.AddressForCondition);
        if (address == null) 
            throw new BadRequestException($"No address with Id {basket.AddressForCondition} was found");

        var deliveryCondition = _connection.GetQueryable<DeliveryConditionGw>().FirstOrDefault(d => d.Id == basket.DeliveryConditionId);
        if (deliveryCondition == null)
            throw new BadRequestException($"Delivery condition {basket.DeliveryConditionId} not found");

        var shippfreeArtsQ = _connection.GetQueryable<ArticleHelperGw>().Where(a => a.FreigthFree);
        shippfreeArtsQ = basket.Positions
            .Aggregate(shippfreeArtsQ, (current, position) => current.Where(a => a.Id == position.ArticleId));

        var charges = new List<ChargePosition>();
        if (shippfreeArtsQ.ToList().Count == basket.Positions.Count)
            return charges;

        var orderTotalBase = 
            basket.VatCalculation switch
            {
                VatCalculationType.Inclusive => basket.GrandTotalAmount,
                _ => basket.GrandTotalAmount - basket.TaxTotalAmount,
            };


        var shippingTotBase = orderTotalBase - basket
            .Charges
            .Where(c => c.PosType == OrderPositionType.Shipping)
            .Sum(c => c.GrandTotalAmount - basket.VatCalculation switch
                {
                    VatCalculationType.Inclusive => 0,
                    _ => c.TaxTotalAmount
                }
            );

        if (deliveryCondition.NoShippingCost || shippingTotBase == 0)
            return charges;

        var freightCost = _connection.GetQueryable<FreightCost>()
            .FirstOrDefault(x => shippingTotBase <= x.FreightExemptFrom && x.PriceGroupId == address.PriceGroupId);

        if (freightCost != null)
        {
            charges.Add(new ChargePosition()
            {
                BasketParams = basket,
                PosType = OrderPositionType.Shipping,
                BilledQuantity = 1,
                OrderedQuantity = 1,
                ArticleId = freightCost.ArticleId,
                UnitPrice = basket.VatCalculation switch
                {
                    VatCalculationType.Inclusive => freightCost.FreightPrice / (100 + freightCost.TaxRate) * 100,
                    _ => freightCost.FreightPrice
                },
                ApplicableTaxRate = freightCost.TaxRate
            });
        }

        return charges;
    }
}