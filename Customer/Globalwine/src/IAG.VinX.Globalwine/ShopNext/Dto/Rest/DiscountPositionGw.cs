using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.VinX.Globalwine.ShopNext.Dto.Enum;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// discount, all output and calculated by VinX
/// </summary>
[DisplayName("DiscountPosition")]
[DataContract]
public class DiscountPositionGw : PositionBaseGw
{
    public DiscountPositionGw()
    {
        PosType = OrderPositionTypeGw.Discount;
        PriceKind = PriceCalculationKindGw.Override;
    }
}