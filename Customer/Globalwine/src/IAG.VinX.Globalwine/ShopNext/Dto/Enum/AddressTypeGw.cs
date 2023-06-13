using System.Runtime.Serialization;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Enum;

/// <summary>
/// type of address
/// </summary>
/// <value>type of address</value>
public enum AddressTypeGw
{
    /// <summary>
    /// Enum Main for main
    /// </summary>
    [EnumMember(Value = "main")] Main = 1,

    /// <summary>
    /// Enum Delivery for delivery
    /// </summary>
    [EnumMember(Value = "delivery")] Delivery = 2,

    /// <summary>
    /// Enum Billing for billing
    /// </summary>
    [EnumMember(Value = "billing")] Billing = 3,

    /// <summary>
    /// Enum Condition for condition
    /// </summary>
    [EnumMember(Value = "condition")] Condition = 4
}