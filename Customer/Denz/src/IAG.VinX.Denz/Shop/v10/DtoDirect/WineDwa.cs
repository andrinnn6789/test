using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

namespace IAG.VinX.Denz.Shop.v10.DtoDirect;

/// <summary>
/// Wine Info with customer extensions
/// </summary>
[DataContract]
[DisplayName("WineDwa")]
[TableCte(@"
       WITH WineDwa
        AS 
        (
        SELECT 
            Wein_Sulfite                AS ContainsSulfit,

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
public class WineDwa: WineV10 
{
    /// <summary>
    /// contains sulfit, Wein_Sulfite
    /// </summary>
    [DataMember(Name = "containsSulfit")]
    public bool ContainsSulfit { get; set; }
}