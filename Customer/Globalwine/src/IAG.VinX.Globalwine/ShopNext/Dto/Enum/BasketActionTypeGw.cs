using System.Runtime.Serialization;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Enum;

/// <summary>
/// action to perform, calculate only or treat as order, input, required
/// </summary>
/// <value>action to perform, calculate only or treat as order, input, required</value>
public enum BasketActionTypeGw
{
    /// <summary>
    /// Enum Calculate for calculate
    /// </summary>
    [EnumMember(Value = "calculate")] Calculate = 1,

    /// <summary>
    /// Enum Order for order
    /// </summary>
    [EnumMember(Value = "order")] Order = 2
}