using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

using JetBrains.Annotations;

namespace IAG.VinX.Langendorf.Shop.v10.DtoDirect;

/// <summary>
/// Articles with customer extensions
/// </summary>
[DataContract]
[DisplayName("ArticleLad")]
[TableCte(@"
       WITH ArticleLad
        AS 
        (
        SELECT 
            DDProduct_ContentInfo               AS ContentInfo,
            DDProduct_NahrungsInfo              AS NutritionInfo,
            DDProduct_AllergenInfo              AS AllergenInfo,
            CASE DDProduct_NoNutritionInfo WHEN 0 THEN 0 ELSE 1 END AS NoNutritionInfo,
            Art_Artikelinfo                     AS ArticleInfo,

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
        LEFT JOIN DDBundle ON DDBundle_ID = Art_DDBundleID
        LEFT JOIN DDProduct ON DDProduct_ID = DDBundle_DDProductID 
        )
    ")]
public class ArticleLad: ArticleV10 
{
    /// <summary>
    /// Content Info from Product, DDProduct_ContentInfo
    /// </summary>
    [DataMember(Name = "contentInfo")]
    [CanBeNull]
    public string ContentInfo { get; set; }

    /// <summary>
    /// Nutrition Info from Product, DDProduct_NahrungsInfo
    /// </summary>
    [DataMember(Name = "nutritionInfo")]
    [CanBeNull]
    public string NutritionInfo { get; set; }

    /// <summary>
    /// Allergen Info from Product, DDProduct_AllergenInfo
    /// </summary>
    [DataMember(Name = "allergenInfo")]
    [CanBeNull]
    public string AllergenInfo { get; set; }

    /// <summary>
    /// has no Nutrition Info, DDProduct_NoNutritionInfo
    /// </summary>
    [DataMember(Name = "noNutritionInfo")]
    public bool? NoNutritionInfo { get; set; }

    [DataMember(Name = "articleInfo")]
    [CanBeNull]
    public string ArticleInfo { get; set; }
}