using System.Runtime.Serialization;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Enum;

/// <summary>
/// type of address structures
/// </summary>
public enum AddressstructureGw
{
    /// <summary>
    /// Enum Main for main
    /// </summary>
    [EnumMember(Value = "Privat")] Private = 10,

    /// <summary>
    /// Enum Delivery for delivery
    /// </summary>
    [EnumMember(Value = "Firma")] Company = 20
}