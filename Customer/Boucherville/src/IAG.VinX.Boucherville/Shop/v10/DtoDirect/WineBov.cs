using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.AtlasType;
using IAG.Common.DataLayerSybase.Attribute;
using IAG.Infrastructure.Formatter;
using IAG.VinX.Shop.v10.DtoDirect;

using JetBrains.Annotations;

namespace IAG.VinX.Boucherville.Shop.v10.DtoDirect;

/// <summary>
/// Wines with customer extensions
/// </summary>
[DataContract]
[DisplayName("WineBov")]
[TableCte(@"
        WITH WineBov
        AS 
        (
        SELECT 
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
            Wein_Geschichte             AS HistoryRtf,

            Wein_BezeichnungEN          AS DesignationEn,
            Wein_VinifikationEN         AS VinificationRtfEn,
            Wein_CharakterEN            AS CharacteristicRtfEn,
            Wein_GeschichteEN           AS HistoryRtfEn,
            Wein_TerroirEN              AS TerroirRtfEn,
            Wein_KonsumhinweisEN        AS ConsumptionAdviceRtfEn,
            Wein_BewertungEN            AS RatingTextRtfEn
        FROM WeinInfo
        )
    ")]
public class WineBov : WineV10
{
    [UsedImplicitly]
    public string DesignationEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty DesignationMultiLang =>
        new()
        {
            { "DE", Designation },
            { "EN", DesignationEn }
        };

    [UsedImplicitly]
    public string VinificationRtfEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty VinificationMultiLang =>
        new()
        {
            { "DE", RtfCleaner.Clean(VinificationRtf) },
            { "EN", RtfCleaner.Clean(VinificationRtfEn) }
        };

    [UsedImplicitly]
    public string CharacteristicRtfEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty CharacteristicMultiLang =>
        new()
        {
            { "DE", RtfCleaner.Clean(CharacteristicRtf) },
            { "EN", RtfCleaner.Clean(CharacteristicRtfEn) }
        };

    [UsedImplicitly]
    public string HistoryRtfEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty HistoryMultiLang =>
        new()
        {
            { "DE", RtfCleaner.Clean(HistoryRtf) },
            { "EN", RtfCleaner.Clean(HistoryRtfEn) }
        };

    [UsedImplicitly]
    public string TerroirRtfEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty TerroirMultiLang =>
        new()
        {
            { "DE", RtfCleaner.Clean(TerroirRtf) },
            { "EN", RtfCleaner.Clean(TerroirRtfEn) }
        };

    [UsedImplicitly]
    public string ConsumptionAdviceRtfEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty ConsumptionAdviceMultiLang =>
        new()
        {
            { "DE", RtfCleaner.Clean(ConsumptionAdviceRtf) },
            { "EN", RtfCleaner.Clean(ConsumptionAdviceRtfEn) }
        };

    [UsedImplicitly]
    public string RatingTextRtfEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty RatingTextMultiLang =>
        new()
        {
            { "DE", RtfCleaner.Clean(RatingTextRtf) },
            { "EN", RtfCleaner.Clean(RatingTextRtfEn) }
        };

    /// <summary>
    /// translations
    /// </summary>
    [DataMember(Name = "translations")]
    public IEnumerable<Translation> Translations => TranslationAdder.GetTranslations(this, "MultiLang");
}