using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.VinX.Globalwine.ShopNext.Dto.Enum;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// package position of the basket
/// </summary>
[DisplayName("PackagePosition")]
[DataContract]
public class PackagePositionGw : PositionBaseGw
{
    public PackagePositionGw()
    {
        PosType = OrderPositionTypeGw.Package;
        PriceKind = PriceCalculationKindGw.Override;
    }
}