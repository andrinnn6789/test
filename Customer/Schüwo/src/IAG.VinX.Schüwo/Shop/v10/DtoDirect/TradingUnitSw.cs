using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

namespace IAG.VinX.Schüwo.Shop.v10.DtoDirect;

/// <summary>
/// Containers with customer extensions / Grossgebinde
/// </summary>
[DataContract]
[DisplayName("TradingUnitSw")]
[TableCte(@"
        WITH TradingUnitSw
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

            ABS(Gross_AnbruchErlaubtOnline)             AS IsSplitUsageAllowedWeb,
            Gross_VerpackungseinheitproGrossgebinde     AS InfoUnitsPerContainer

        FROM Grossgebinde
        )
    ")]
public class TradingUnitSw: TradingUnitV10
{
    /// <summary>
    /// Partial container allowed, Gross_AnbruchErlaubtOnline
    /// </summary>
    [DataMember(Name= "isSplitUsageAllowedWeb")]
    public bool IsSplitUsageAllowedWeb { get; set; }

    /// <summary>
    /// Info text units per container, Gross_VerpackungseinheitproGrossgebinde
    /// </summary>
    [DataMember(Name= "infoUnitsPerContainer")]
    public int InfoUnitsPerContainer { get; set; }
}