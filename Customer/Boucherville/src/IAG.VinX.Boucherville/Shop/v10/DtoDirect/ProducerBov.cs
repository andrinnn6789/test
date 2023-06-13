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
/// Producers with customer extensions
/// </summary>
[DataContract]
[DisplayName("ProducerBov")]
[TableCte(@"
        WITH ProducerBov
        AS 
        (
        SELECT 
            Prod_ID                     AS Id, 
            Prod_Bezeichnung            AS Designation,
            Prod_Text                   AS Description,
            Prod_Geschichte             AS HistoryRtf,
            Prod_Homepage               AS Website,
            Prod_AdresseID              AS AddressId,
            ABS(Prod_EigenProduktion)   AS InhouseProduction,

            Prod_Sort                   AS Sort,
            Prod_BezeichnungEN          AS DesignationEn,
            Prod_TextEN                 AS DescriptionEn,
            Prod_GeschichteEN           AS HistoryRtfEn
        FROM Produzent
        )
    ")]
public class ProducerBov : ProducerV10
{
    /// <summary>
    /// sort field for lists, Prod_Sort
    /// </summary>
    [DataMember(Name = "sort")]
    public string Sort { get; set; }

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
    public string DescriptionEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty DescriptionMultiLang =>
        new()
        {
            { "DE", Description },
            { "EN", DescriptionEn }
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

    /// <summary>
    /// translations
    /// </summary>
    [DataMember(Name = "translations")]
    public IEnumerable<Translation> Translations => TranslationAdder.GetTranslations(this, "MultiLang");
}