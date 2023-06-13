using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

namespace IAG.VinX.Gedex.Shop.v10.DtoDirect;

/// <summary>
/// Wine with customer extensions
/// </summary>
[DataContract]
[DisplayName("WineGga")]
[TableCte(@"
        WITH WineGga
        AS 
        (
        SELECT 
            Wein_Empfehlung             AS IsRecommendation,
            Wein_Topseller              AS IsTopseller,
            Wein_Bio                    AS IsBio,
            Wein_Vegan                  AS IsVegan,
            Wein_Praemiert              AS IsAwarded,
            Wein_SchwereID              AS StrengthId,
            Wein_AnzahlRebsorteID       AS GrapeAmountId,
            
            Wein_ID                     AS Id, 
            Wein_Bezeichnung            AS Designation,
            Wein_SuchfelderInternet     AS SearchTerm,
            Wein_KlassifikationID       AS ClassificationId,
            Wein_Lagerdauer             AS StoragePeriod,
            Wein_LagerdauerMax          AS MaximalStoragePeriod,
            Wein_Bewertung              AS RatingTextRtf,
            Wein_Trinktemparatur        AS DrinkingTemperature,
            Wein_TrinktemparaturMin     AS MinimalDrinkingTemperature,
            Wein_TrinktemparaturMax     AS MaximalDrinkingTemperature,
            Wein_Bewertungspunkte       AS RatingPoints,
            Wein_Konsumhinweis          AS ConsumptionAdviceRtf,
            Wein_Vinifikation           AS VinificationRtf,
            Wein_Charakter              AS CharacteristicRtf,
            Wein_Terroir                AS TerroirRtf,
            Wein_Geschichte             AS HistoryRtf
        FROM WeinInfo
        )
    ")]
public class WineGga : WineV10 
{
    /// <summary>
    /// Is recommendation, Wein_Empfehlung
    /// </summary>
    [DataMember(Name = "isRecommendation")]
    public bool IsRecommendation { get; set; }

    /// <summary>
    /// Is topseller, Wein_Topseller
    /// </summary>
    [DataMember(Name = "isTopseller")]
    public bool IsTopseller { get; set; }

    /// <summary>
    /// Is bio, Wein_Bio
    /// </summary>
    [DataMember(Name = "isBio")]
    public bool IsBio { get; set; }

    /// <summary>
    /// Is vegan, Wein_Vegan
    /// </summary>
    [DataMember(Name = "isVegan")]
    public bool IsVegan { get; set; }

    /// <summary>
    /// Is awarded, Wein_Praemiert
    /// </summary>
    [DataMember(Name = "isAwarded")]
    public bool IsAwarded { get; set; }

    /// <summary>
    /// ID of strength, Wein_SchwereID
    /// </summary>
    [DataMember(Name = "strengthId")]
    public int? StrengthId { get; set; }

    /// <summary>
    /// ID of grape amount, Wein_AnzahlRebsorteID
    /// </summary>
    [DataMember(Name = "grapeAmountId")]
    public int? GrapeAmountId { get; set; }
}