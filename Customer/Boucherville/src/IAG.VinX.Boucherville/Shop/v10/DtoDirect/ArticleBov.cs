using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.AtlasType;
using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

using JetBrains.Annotations;

namespace IAG.VinX.Boucherville.Shop.v10.DtoDirect;

/// <summary>
/// Articles with customer extensions
/// </summary>
[DataContract]
[DisplayName("ArticleBov")]
[TableCte(@"
       WITH ArticleBov
        AS 
        (
        SELECT 
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
            CAST(Art_EAN3 AS BIGINT)            AS Ean3,

            Art_Memofeld                        AS Description,
            Art_PraedikatID                     AS PredicateId,
            Art_BezeichnungEN                   AS DesignationEn,
            Art_MaximaleBestellmenge            AS MaximumOrderingQuantity,
            Art_VerfuegbarAb                    AS AvailableFrom
        FROM Artikel
        LEFT JOIN MWST ON MWST_ID=Art_MWSTID 
        )
    ")]
public class ArticleBov : ArticleV10
{
    /// <summary>
    /// description, Art_Memofeld
    /// </summary>
    [DataMember(Name = "description")]
    public string Description { get; set; }

    /// <summary>
    /// ID of predicate, Art_PraedikatID
    /// </summary>
    [DataMember(Name = "predicateId")]
    public int? PredicateId { get; set; }

    [UsedImplicitly]
    public string DesignationEn { get; set; }

    [UsedImplicitly]
    private MultiLanguageProperty DesignationMultiLang =>
        new()
        {
            { "DE", Designation },
            { "EN", DesignationEn }
        };

    /// <summary>
    /// translations
    /// </summary>
    [DataMember(Name = "translations")]
    public IEnumerable<Translation> Translations => TranslationAdder.GetTranslations(this, "MultiLang");
        
    /// <summary>
    /// maximum ordering quantity, Art_MaximaleBestellmenge
    /// </summary>
    [DataMember(Name = "maximumOrderingQuantity")]
    public int MaximumOrderingQuantity { get; set; }

    /// <summary>
    /// the date from which the article is available from, Art_VerfuegbarAb
    /// </summary>
    [DataMember(Name = "availableFrom")]
    public DateTime AvailableFrom { get; set; }
}