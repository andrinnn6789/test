using System.Runtime.Serialization;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Enum;

/// <summary>
/// has promotion price / AktTyp_Typ
/// </summary>
/// <value>has promotion price / AktTyp_Typ</value>
public enum PromotionKindTypeGw
{
    /// <summary>
    /// Enum None for none
    /// </summary>
    [EnumMember(Value = "none")] None = 0,

    /// <summary>
    /// Enum ReducePrice for reducePrice
    /// </summary>
    [EnumMember(Value = "reducePrice")] ReducePrice = 10,

    /// <summary>
    /// Enum Percent for percent
    /// </summary>
    [EnumMember(Value = "percent")] Percent = 25,

    /// <summary>
    /// Enum CentOff for centOff
    /// </summary>
    [EnumMember(Value = "centOff")] CentOff = 20,

    /// <summary>
    /// Enum ReducePce for reducePce
    /// </summary>
    [EnumMember(Value = "reducePce")] ReducePce = 30
}