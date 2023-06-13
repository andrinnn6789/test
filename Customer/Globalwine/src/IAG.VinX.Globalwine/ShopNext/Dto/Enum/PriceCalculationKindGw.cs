
using System.Runtime.Serialization;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Enum;

public enum PriceCalculationKindGw
{
    /// <summary>
    /// special customer price
    /// </summary>
    [EnumMember(Value = "customer")]
    Customer = 10,
    /// <summary>
    /// promotion price
    /// </summary>
    [EnumMember(Value = "promotion")]
    Promotion = 20,
    /// <summary>
    /// price from price group
    /// </summary>
    [EnumMember(Value = "priceGroup")]
    PriceGroup = 30,
    /// <summary>
    /// base price from article
    /// </summary>
    [EnumMember(Value = "article")]
    Article = 40,
    /// <summary>
    /// price is calculated 
    /// </summary>
    [EnumMember(Value = "override")]
    Override = 50,
    /// <summary>
    /// price is set by the fromShop
    /// </summary>
    [EnumMember(Value = "fromShop")]
    FromShop = 55,
    /// <summary>
    /// no price found
    /// </summary>
    [EnumMember(Value = "notFound")]
    NotFound = 90
}