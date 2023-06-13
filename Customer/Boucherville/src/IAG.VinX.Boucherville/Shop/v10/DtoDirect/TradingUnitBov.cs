using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.AtlasType;
using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

using JetBrains.Annotations;

namespace IAG.VinX.Boucherville.Shop.v10.DtoDirect;

/// <summary>
/// Trading units with customer extensions, Grossgebinde
/// </summary>
[DataContract]
[DisplayName("TradingUnitBov")]
[TableCte(@"
        WITH TradingUnitBov
        AS 
        (
        SELECT 
            Gross_ID                    AS Id, 
            Gross_BezeichnungWeb        AS DesignationWeb,
            Gross_KuerzelWeb            AS ShortNameWeb, 
            Gross_Kuerzel               AS ShortName,
            Gross_Suchbegriff           AS SearchTerm,
            Gross_AnbruchArtikelID      AS SplitUsageArticleId,
            ABS(Gross_AnbruchErlaubt)   AS IsSplitUsageAllowed,
            Gross_AnbruchMenge          AS SplitUsageQuantity,
            ABS(Gross_AnbruchZuschlag)  AS HasSplitUsageFee,
            Gross_ArtID                 AS ArticleId,
            Gross_EinhMinimal           AS MinimalUnit,
            Gross_EinhProGG             AS UnitsPerTradingUnit,
            ABS(Gross_InklAbfDepot)     AS IsFillingDepositIncluded,
            ABS(Gross_Verrechenbar)     AS IsChargeable,

            Gross_BezeichnungWebEN      AS DesignationWebEn,
            Gross_KuerzelWebEN          AS ShortNameWebEn,
            Gross_KuerzelEN             AS ShortNameEn
        FROM Grossgebinde
        )
    ")]
public class TradingUnitBov : TradingUnitV10
{
    [UsedImplicitly]
    public string DesignationWebEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty DesignationWebMultiLang =>
        new()
        {
            { "DE", DesignationWeb },
            { "EN", DesignationWebEn }
        };

    [UsedImplicitly]
    public string ShortNameWebEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty ShortNameWebMultiLang =>
        new()
        {
            { "DE", ShortNameWeb },
            { "EN", ShortNameWebEn }
        };
    [UsedImplicitly]
    public string ShortNameEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty ShortNameMultiLang =>
        new()
        {
            { "DE", ShortName },
            { "EN", ShortNameEn }
        };

    /// <summary>
    /// translations
    /// </summary>
    [DataMember(Name = "translations")]
    public IEnumerable<Translation> Translations => TranslationAdder.GetTranslations(this, "MultiLang");
}