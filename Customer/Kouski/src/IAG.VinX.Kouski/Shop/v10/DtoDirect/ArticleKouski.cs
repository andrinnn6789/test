using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

namespace IAG.VinX.Kouski.Shop.v10.DtoDirect;

/// <summary>
/// Articles with customer extensions
/// </summary>
[DataContract]
[DisplayName("ArticleKouski")]
[TableCte(@"
       WITH ArticleKouski
        AS
        (
        SELECT 
            Art_Gewichtsanteil                  AS WeightInclTare,
            Art_Tara                            AS Tare,
            Art_Hoehe                           AS Height,
            Art_HoeheVerpackung                 AS HeightPackaging,
            Art_Breite                          AS Width,
            Art_BreiteVerpackung                AS WidthPackaging,
            Art_Tiefe                           AS Length,
            Art_TiefeVerpackung                 AS LengthPackaging,
            Art_Mindesthaltbarkeit              AS MinimumStorageLife,
            CAST(Art_EAN4 AS BIGINT)            AS Ean4,
            Art_Memofeld                        AS Description,
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
public class ArticleKouski : ArticleV10
{
     /// <summary>
    /// Weight incl. tare, Art_Gewichtsanteil
    /// </summary>
    [DataMember(Name = "weightInclTare")]
    public decimal? WeightInclTare { get; set; }
    
    /// <summary>
    /// Tare, Art_Tara
    /// </summary>
    [DataMember(Name = "tare")]
    public decimal? Tare { get; set; }
    
    /// <summary>
    /// Height, Art_Hoehe
    /// </summary>
    [DataMember(Name = "height")]
    public decimal? Height { get; set; }
    
    /// <summary>
    /// Height Packaging, Art_HoeheVerpackung 
    /// </summary>
    [DataMember(Name = "heightPackaging")]
    public decimal? HeightPackaging { get; set; }
    
    /// <summary>
    /// Width, Art_Breite
    /// </summary>
    [DataMember(Name = "width")]
    public decimal? Width { get; set; }
    
    /// <summary>
    /// Width Packaging, Art_BreiteVerpackung 
    /// </summary>
    [DataMember(Name = "widthPackaging")]
    public decimal? WidthPackaging { get; set; }
    
    /// <summary>
    /// Length, Art_Tiefe
    /// </summary>
    [DataMember(Name = "length")]
    public decimal? Length { get; set; }
    
    /// <summary>
    /// Length Packaging, Art_TiefeVerpackung 
    /// </summary>
    [DataMember(Name = "lengthPackaging")]
    public decimal? LengthPackaging { get; set; }
    
    /// <summary>
    /// Minimum storage life in months, Art_Mindesthaltbarkeit 
    /// </summary>
    [DataMember(Name = "minimumStorageLife")]
    public int MinimumStorageLife { get; set; }

    /// <summary>
    /// EAN4
    /// </summary>
    [DataMember(Name = "ean4")]
    public long? Ean4 { get; set; }
    
    /// <summary>
    /// description, Art_Memofeld
    /// </summary>
    [DataMember(Name = "description")]
    public string? Description { get; set; }
}