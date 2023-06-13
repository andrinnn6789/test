using System.Runtime.Serialization;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Enum;

/// <summary>
/// action to perform: create new address or change existing
/// </summary>
public enum AddressChangeTypeGw
{
    /// <summary>
    /// Enum New for new
    /// </summary>
    [EnumMember(Value = "new")] New = 1,

    /// <summary>
    /// Enum Change for change
    /// </summary>
    [EnumMember(Value = "change")] Change = 6,

    /// <summary>
    /// Enum New for new
    /// </summary>
    [EnumMember(Value = "nop")] Nop = 99
}