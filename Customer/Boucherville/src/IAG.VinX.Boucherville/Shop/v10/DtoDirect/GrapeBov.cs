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
/// Grapes with customer extensions, Traubensorte
/// </summary>
[DataContract]
[DisplayName("GrapeBov")]
[TableCte(@"
        WITH GrapeBov
        AS 
        (
        SELECT 
            Sorte_ID            AS Id, 
            Sorte_Bezeichnung   AS Designation,
            Sorte_Geschichte    AS HistoryRtf,
            Sorte_Charakter     AS CharacteristicRtf,

            Sorte_BezeichnungEN AS DesignationEn,
            Sorte_GeschichteEN  AS HistoryRtfEn,
            Sorte_CharakterEN   AS CharacteristicRtfEn
        FROM Traubensorte
        )
    ")]
public class GrapeBov : GrapeV10
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
    public string HistoryRtfEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty HistoryMultiLang =>
        new()
        {
            { "DE", RtfCleaner.Clean(HistoryRtf) },
            { "EN", RtfCleaner.Clean(HistoryRtfEn) }
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

    /// <summary>
    /// translations
    /// </summary>
    [DataMember(Name = "translations")]
    public IEnumerable<Translation> Translations => TranslationAdder.GetTranslations(this, "MultiLang");
}