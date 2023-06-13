using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.VinX.Globalwine.ShopNext.Dto.Enum;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// charge, all output and always calculated by VinX
/// </summary>
[DisplayName("ChargePosition")]
[DataContract]
public class ChargePositionGw : PositionBaseGw
{ 
    public ChargePositionGw()
    {
        PosType = OrderPositionTypeGw.Charge;
        PriceKind = PriceCalculationKindGw.Override;
    }
}