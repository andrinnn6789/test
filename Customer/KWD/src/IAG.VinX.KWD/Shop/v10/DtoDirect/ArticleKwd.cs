using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

namespace IAG.VinX.KWD.Shop.v10.DtoDirect;

/// <summary>
/// Articles, table "Artikel" with additional customer extensions
/// </summary>
[DataContract]
[DisplayName("ArticleKwd")]
[TableCte(@"
    WITH ArticleKwd
        AS 
        (
        SELECT 
            Art_Bewertungspunkte                AS EvaluationPoints,
            Art_BeschriebCharakter              AS CharacterDescription,
            Art_AusbauVinifikation              AS VinificationMaturation,
            Art_Eignung1                        AS Suitability,

            Art_ID                              AS Id, 
            Art_Bezeichnung                     AS Designation,
            Art_Suchbegriff                     AS SearchTerm,
            TRIM(STR(Art_Artikelnummer, 20))    AS ArticleNumber,
            Art_AKatID                          AS ArticleCategoryId,
            Art_Artikeltyp                      AS ArticleType,
            Art_AbfID                           AS FillingUnitId,
            Art_GrossID                         AS TradingUnitId,
            MWST_Prozent                        AS VatPercentage,
            Art_LandID                          AS CountryId,
            Art_RegID                           AS RegionId,
            Art_ZyklusID                        AS ProductCycleId,
            Art_ProdID                          AS ProducerId,
            Art_Jahrgang                        AS Vintage,
            Art_WeininfoID                      AS WineId,
            Art_Volumen                         AS Volume,
            ABS(Art_Aktiv)                      AS IsActive,
            Art_EGruppeID                       AS ECommerceGroupId,
            Art_Preiseinheit                    AS PriceUnit,
            Art_Grundpreis                      AS BasePrice,
            Art_DatumErfassung                  AS CreatedDate,
            Art_DatumMutation                   AS ChangedOn,
            Art_ErsatzArtikelID                 AS SubstituteArticleId,
            Art_ErsatzArtikelText               AS SubstituteArticleText,
            Art_BildDatei                       AS PicturePathLocal,
            ABS(Art_MitLager)                   AS WithStock,
            Art_Gewichtsanteil                  AS Weight,
            CAST(Art_EAN1 AS BIGINT)            AS Ean1,
            CAST(Art_EAN2 AS BIGINT)            AS Ean2,
            CAST(Art_EAN3 AS BIGINT)            AS Ean3
        FROM Artikel
        LEFT JOIN MWST ON MWST_ID=Art_MWSTID 
        )
")]
public class ArticleKwd : ArticleV10
{
    /// <summary>
    /// Bewertungspunkte on the Artikel Table, customer specific
    /// </summary>
    [DataMember(Name = "evaluationPoints")]
    public string? EvaluationPoints { get; set; }

    /// <summary>
    /// BeschriebCharakter on the Artikel Table, customer specific
    /// </summary>
    [DataMember(Name = "characterDescription")]
    public string? CharacterDescription { get; set; }

    /// <summary>
    /// AusbauVinifikation on the Artikel Table, customer specific
    /// </summary>
    [DataMember(Name = "vinificationMaturation")]
    public string? VinificationMaturation { get; set; }

    /// <summary>
    /// Eignung1 on the Artikel Table, customer specific
    /// </summary>
    [DataMember(Name = "suitability")]
    public string? Suitability { get; set; }
}