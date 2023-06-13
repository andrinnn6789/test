using System.Runtime.Serialization;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Enum;

/// <summary>
/// kind of orderPosition, 'orderPos' is the only one treated as in input from the shop, the rest are calculated positions
/// </summary>
/// <value>kind of orderPosition, 'orderPos' is the only one treated as in input from the shop, the rest are calculated positions</value>
public enum OrderPositionTypeGw
{
    /// <summary>
    /// orderPos position
    /// </summary>
    [EnumMember(Value = "orderPos")] OrderPos = 1,

    /// <summary>
    /// discount position
    /// </summary>
    [EnumMember(Value = "discount")] Discount = 2,

    /// <summary>
    /// charge position
    /// </summary>
    [EnumMember(Value = "charge")] Charge = 3,

    /// <summary>
    /// shipping position
    /// </summary>
    [EnumMember(Value = "shipping")] Shipping = 4,

    /// <summary>
    /// package position
    /// </summary>
    [EnumMember(Value = "package")] Package = 5
}