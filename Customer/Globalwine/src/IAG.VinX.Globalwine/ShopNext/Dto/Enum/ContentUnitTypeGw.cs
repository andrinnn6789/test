using System.Runtime.Serialization;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Enum;

/// <summary>
/// unit of a content
/// </summary>
/// <value>unit of a content</value>
public enum ContentUnitTypeGw
{
    /// <summary>
    /// Enum Ml for ml
    /// </summary>
    [EnumMember(Value = "ml")] Ml = 1,

    /// <summary>
    /// Enum Kg for kg
    /// </summary>
    [EnumMember(Value = "kg")] Kg = 2,

    /// <summary>
    /// Enum Pce for pce
    /// </summary>
    [EnumMember(Value = "pce")] Pce = 3
}