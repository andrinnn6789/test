using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

namespace IAG.VinX.Denz.Shop.v10.DtoDirect;

/// <summary>
/// Articles with customer extensions
/// </summary>
[DataContract]
[DisplayName("ArticleDwa")]
[TableCte(@"
       WITH ArticleDwa
        AS 
        (
        SELECT 
            Art_ShopNeuheit                     AS IsShopNew,
            Art_ShopRarität                     AS IsShopRarity,
            Art_ShopPrimeur                     AS IsShopPrimeur,
            Art_ShopBioweine                    AS IsShopBio,
    
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
public class ArticleDwa: ArticleV10 
{
    /// <summary>
    /// is new in shop, Art_ShopNeuheit
    /// </summary>
    [DataMember(Name = "isShopNew")]
    public bool IsShopNew { get; set; }
    
    /// <summary>
    /// is rarity in shop, Art_ShopRarität
    /// </summary>
    [DataMember(Name = "isShopRarity")]
    public bool IsShopRarity { get; set; }
    
    /// <summary>
    /// is primeur in shop, Art_ShopPrimeur
    /// </summary>
    [DataMember(Name = "isShopPrimeur")]
    public bool IsShopPrimeur { get; set; }
    
    /// <summary>
    /// is bio in shop, Art_ShopBioweine
    /// </summary>
    [DataMember(Name = "isShopBio")]
    public bool IsShopBio { get; set; }
}