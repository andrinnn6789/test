using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

namespace IAG.VinX.Schüwo.Shop.v10.DtoDirect;

/// <summary>
/// Articles with customer extensions
/// </summary>
[DataContract]
[DisplayName("ArticleSw")]
[TableCte(@"
        WITH ArticleSw
        AS 
        (
        SELECT 
            Art_MindestbestellmengeAufAnfrage   AS OrderQuantityMinOnDemand,
            Art_Mindestbestellmenge             AS OrderQuantityMin,
            Art_MaximaleBestellmenge            AS orderQuantityMax,
            Art_CharaktereigenschaftID          AS CharakterId,
            ABS(Art_Barriqueausbau)             AS Barrique,
            ABS(Art_Holzfassausbau)             AS WoodenBarrel,
            ABS(Art_Stahltankausbau)            AS SteelTank,
            ABS(Art_ZertifizierterBiowein)      AS CertyfiedBio,
            ABS(Art_Vegan)                      AS Vegan,
            Art_InfotextAufAnfrage              AS InfotextOnDemand,
            Art_LieferfristAufAnfrage           AS InfotextDeliverytimeOnDemand,
            Art_AGrpRabID                       AS DiscountGroupId,
            Art_MarkeID                         AS BrandId,
            Art_BezeichnungKatalog              AS DesignationOnline,

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
public class ArticleSw: ArticleV10 
{
    /// <summary>
    /// Hint für minimum ordering quantity / Art_MindestbestellmengeAufAnfrage
    /// </summary>
    [DataMember(Name = "orderQuantityMinOnDemand")]
    public string OrderQuantityMinOnDemand { get; set; }

    /// <summary>
    /// Minimale Bestellmenge / Art_Mindestbestellmenge
    /// </summary>
    [DataMember(Name = "orderQuantityMin")]
    public int? OrderQuantityMin { get; set; }

    /// <summary>
    /// Maximale Bestellmenge / Art_MaximaleBestellmenge
    /// </summary>
    [DataMember(Name = "orderQuantityMax")]
    public int? OrderQuantityMax { get; set; }

    /// <summary>
    /// FK Character / Art_CharaktereigenschaftID
    /// </summary>
    [DataMember(Name = "charakterId")]
    public int? CharakterId { get; set; }

    /// <summary>
    /// Art_Barriqueausbau
    /// </summary>
    [DataMember(Name = "barrique")]
    public bool Barrique { get; set; }

    /// <summary>
    /// Art_Holzfassausbau
    /// </summary>
    [DataMember(Name = "woodenBarrel")]
    public bool WoodenBarrel { get; set; }

    /// <summary>
    /// Art_Stahltankausbau
    /// </summary>
    [DataMember(Name = "steelTank")]
    public bool SteelTank { get; set; }

    /// <summary>
    /// Art_ZertifizierterBiowein
    /// </summary>
    [DataMember(Name = "certyfiedBio")]
    public bool CertyfiedBio { get; set; }

    /// <summary>
    /// Art_Vegan
    /// </summary>
    [DataMember(Name = "vegan")]
    public bool Vegan { get; set; }

    /// <summary>
    /// Info text on product demand / Art_InfotextAufAnfrage
    /// </summary>
    [DataMember(Name = "infotextOnDemand")]
    public string InfotextOnDemand { get; set; }

    /// <summary>
    /// Info text delivery time / Art_LieferfristAufAnfrage
    /// </summary>
    [DataMember(Name = "infotextDeliverytimeOnDemand")]
    public string InfotextDeliverytimeOnDemand { get; set; }

    /// <summary>
    /// FK discount group / Art_AGrpRabID
    /// </summary>
    [DataMember(Name = "discountGroupId")]
    public int? DiscountGroupId { get; set; }

    /// <summary>
    /// FK producer, brand / Art_MarkeID
    /// </summary>
    [DataMember(Name = "brandId")]
    public int? BrandId { get; set; }

    /// <summary>
    /// Designation online, Art_BezeichnungKatalog
    /// </summary>
    [DataMember(Name= "designationOnline")]
    public string DesignationOnline { get; set; }
}