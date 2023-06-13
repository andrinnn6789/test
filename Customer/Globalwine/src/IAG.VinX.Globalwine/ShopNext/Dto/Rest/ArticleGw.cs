using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.Infrastructure.Formatter;
using IAG.VinX.Globalwine.ShopNext.Dto.Enum;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// details about the articles
/// </summary>
[DataContract]
[DisplayName("Article")]
[TableCte(@"
        WITH 
        Price (
            ArticleId, ChangedOn
        ) AS (
            SELECT 
               VK_ArtID, Max(VK_ChangedOn)
            FROM VKPreis
            JOIN Artikel ON Art_Id = VK_ArtID
            WHERE " + MasterFilter + @"
            GROUP BY VK_ArtId
        ),
        Rating (
            ArticleId, ChangedOn
        ) AS (
            SELECT 
                ArtBew_ArtikelID, MAX(GREATER(ArtBew_ChangedOn, Bewert_ChangedOn))
            FROM ArtikelBewertung
            JOIN Artikel ON Art_Id = ArtBew_ArtikelID
            JOIN Bewertungsart ON Bewert_ID = ArtBew_BewertungsartID
            WHERE " + MasterFilter + @"
            GROUP BY ArtBew_ArtikelID
        ),
        Composition (
            ArticleId, ChangedOn
        ) AS (
            SELECT 
               Art_ID, Max(GREATER(GREATER(GREATER(Wein_ChangedOn, ArtZus_ChangedOn), Sorte_ChangedOn), ArtZus_ChangedOn))
            FROM ArtikelZusammensetzung
            JOIN Traubensorte ON Sorte_ID = ArtZus_TraubensorteID
            JOIN WeinInfo ON Wein_Id = ArtZus_WeininfoID
            JOIN Artikel On Wein_Id = Art_WeininfoID
            WHERE " + MasterFilter + @"
            GROUP BY Art_Id
        ),
        Recommendation (
            ArticleId, ChangedOn
        ) AS (
            SELECT 
                Art_Id, MAX(GREATER(Empf_ChangedOn, ArtZus_ChangedOn))
            FROM ArtikelEmpfehlung
            JOIN Empfehlung ON Empf_ID = ArtZus_EmpfehlungID
            JOIN WeinInfo ON Wein_Id = ArtZus_WeininfoID
            JOIN Artikel On Wein_Id = Art_WeininfoID
            WHERE " + MasterFilter + @"
            GROUP BY Art_Id
        ),
        ArticleGw (
            Id, ActiveShop, Name, ProductNumber, Description, 
            EanUnit, EanBulkPackaging, Weight, OrderQuantityMin, 
            OrderQuantityMax, VatPercent, ContentDescription, ContentUnit, 
            ContentAmount, BulkPackagingDescription, BulkPackagingContent, Vintage, AlcoholContent, 
            Category, Country, Region, Domaine, Classification, 
            Cultivation, WineCharacter, History, Vinification, Consumation, 
            PromotionMessage, Terroir, Sugar, Acidity, TemporairySoldOut,
            LimitedAvailability, LabelBio, Barrique, Vegan, FineWine, 
            ContainsSulfit, StorageRecommended, StorageMin, StorageMax, TemperatureRecommended, 
            TemperatureMin, TemperatureMax, ProducerId, CarriageFree,

            Taste, 
            Style, 
            ChangedOn
        ) AS (
        SELECT 
            Art_Id, ABS(Art_OnlineAktiv), Art_Bezeichnung, TRIM(STR(Art_Artikelnummer, 20)), Art_Bezeichnung, 
            CAST(Art_EAN1 AS BIGINT), CAST(Art_EAN2 AS BIGINT), Art_Gewichtsanteil, Art_Mindestbestellmenge, 
            Art_MaximaleBestellmenge, MWST_Prozent, IsNull(Abf_BezeichnungShop, Abf_Kuerzel), CASE Abf_InhaltInCl WHEN NULL THEN 3 ELSE 1 END, 
            10 * Abf_InhaltInCl, Gross_Kuerzel, Gross_EinhProGG, Art_Jahrgang, Art_Volumen, 
            ArtKat_Bezeichnung, Land_Bezeichnung, Reg_Bezeichnung, Art_Appellation, Klass_Bezeichnung, 
            Wein_BemerkungAusbau, Wein_Charakter, Wein_Geschichte, Wein_Vinifikation,  Wein_Konsumhinweis,
            Wein_PromotionMessage, Wein_Terroir, Wein_Restzucker, Wein_Saeure, Art_Ausverkauft,
            Art_BeschraenkteLieferbarkeit, Wein_BioAnbau, Wein_BarriqueAusbau, Wein_Vegan, Wein_FineWine, 
            Wein_Sulfitte, Wein_Lagerdauer, Wein_LagerdauerMin, Wein_LagerdauerMax, Wein_Trinktemparatur, 
            Wein_TrinktemparaturMin, Wein_TrinktemparaturMax, Art_ProdId, Abs(Art_Frachtkostenfrei),
            CASE Wein_Geschmack  
                WHEN 10 THEN 'trocken'
                WHEN 20 THEN 'halbtrocken'
                WHEN 30 THEN 'süss'
                WHEN 40 THEN 'Brut'
                WHEN 50 THEN 'edelsüss'
                WHEN 60 THEN 'Extra Brut'
                WHEN 70 THEN 'Extra Trocken'
                WHEN 80 THEN 'fruchtsüss'
			END,
            CASE Wein_Weinstil  
               WHEN 1  THEN 'fruchtig & aromatisch'
               WHEN 2  THEN 'jung & frisch'
               WHEN 3  THEN 'üppig & voluminös'
               WHEN 4  THEN 'fein & aromatisch'
               WHEN 5  THEN 'fruchtig & weich'
               WHEN 6  THEN 'kräftig & würzig'
               WHEN 7  THEN 'samtig & üppig'
               WHEN 8  THEN 'aufgespritet rot'
               WHEN 9  THEN 'aufgespritet weiss'
               WHEN 10 THEN 'ausdruckstark & fein'
               WHEN 11 THEN 'edelsüss'
               WHEN 12 THEN 'frisch & mineralisch'
               WHEN 13 THEN 'komplex & reif'
               WHEN 14 THEN 'leicht & fruchtig'
               WHEN 15 THEN 'restsüss'
               WHEN 16 THEN 'roter Schaumwein'
               WHEN 17 THEN 'üppig & voluminös'
               WHEN 18 THEN 'aromatisch & frisch'  
			END,
			GREATER(
			    GREATER(
			        GREATER(
                        GREATER(
                            GREATER(
                                GREATER(Art_ChangedOn, IsNull(Wein_ChangedOn, Art_ChangedOn)), 
                            Abf_ChangedOn), 
                         IsNull(Price.ChangedOn, Art_ChangedOn)), 
                    IsNull(Rating.ChangedOn, Art_ChangedOn)),
                IsNull(Recommendation.ChangedOn, Art_ChangedOn)),
            IsNull(Composition.ChangedOn, Art_ChangedOn))
        FROM Artikel
        JOIN Abfuellung ON Abf_ID = Art_AbfID
        LEFT OUTER JOIN MWST ON MWST_ID = Art_MwstID                
        LEFT OUTER JOIN Grossgebinde ON Gross_ID = Art_GrossID
        LEFT OUTER JOIN ArtikelKategorie ON ArtKat_ID = Art_AKatID
        LEFT OUTER JOIN Land ON Land_ID = Art_LandID
        LEFT OUTER JOIN Region ON Reg_ID = Art_RegID
        LEFT OUTER JOIN WeinInfo ON Wein_ID = Art_WeininfoID
        LEFT OUTER JOIN Klassifikation ON Klass_ID = Wein_KlassifikationID
        LEFT OUTER JOIN Price ON Price.ArticleId = Art_ID
        LEFT OUTER JOIN Rating ON Rating.ArticleId = Art_ID
        LEFT OUTER JOIN Recommendation ON Recommendation.ArticleId = Art_ID
        LEFT OUTER JOIN Composition ON Composition.ArticleId = Art_ID        
        WHERE " + MasterFilter + @"
        )
        ")]
public class ArticleGw
{
    public const string MasterFilter = "Art_Aktiv = -1 AND Art_OnlineAktiv = -1";

    /// <summary>
    /// id in VinX / Art_Id
    /// </summary>
    [Key]
    [DataMember(Name = "id")]
    public int Id { get; set; }

    /// <summary>
    /// active in shop / Art_OnlineAktiv
    /// </summary>
    [DataMember(Name = "activeShop")]
    public bool ActiveShop { get; set; }

    /// <summary>
    /// timestamp utc last change
    /// </summary>
    [DataMember(Name = "changedOn")]
    public DateTime ChangedOn { get; set; }

    /// <summary>
    /// name / Art_Bezeichnung
    /// </summary>
    [DataMember(Name = "name")]
    public string Name { get; set; }

    /// <summary>
    /// product number / Art_Artikelnummer
    /// </summary>
    [DataMember(Name = "productNumber")]
    public string ProductNumber { get; set; }

    /// <summary>
    /// description / Art_Bezeichnung
    /// </summary>
    [DataMember(Name = "description")]
    public string Description { get; set; }

    /// <summary>
    /// EAN of the unit / Art_EAN1
    /// </summary>
    [DataMember(Name = "eanUnit")]
    public long? EanUnit { get; set; }

    /// <summary>
    /// EAN of the bulk packaging / Art_EAN2
    /// </summary>
    [DataMember(Name = "eanBulkPackaging")]
    public long? EanBulkPackaging { get; set; }

    /// <summary>
    /// weight / Art_Gewichtsanteil
    /// </summary>
    [DataMember(Name = "weight")]
    public decimal? Weight { get; set; }

    /// <summary>
    /// stock available / stock - reservations and other logic
    /// </summary>
    [DataMember(Name = "qtyStock")]
    [NotMapped]
    public decimal QtyStock { get; set; }

    /// <summary>
    /// minimal ordering quantity / Art_BestellmengeMin
    /// </summary>
    [DataMember(Name = "orderQuantityMin")]
    public int? OrderQuantityMin { get; set; }

    /// <summary>
    /// maximal ordering quantity / Art_BestellmengeMax
    /// </summary>
    [DataMember(Name = "orderQuantityMax")]
    public int? OrderQuantityMax { get; set; }

    /// <summary>
    /// vat percent / MWST_Prozent
    /// </summary>
    [DataMember(Name = "vatPercent")]
    public decimal? VatPercent { get; set; }

    /// <summary>
    /// description selling unit / Abf_KuerzelWeb, fallback Abf_Kuerzel if empty
    /// </summary>
    [DataMember(Name = "contentDescription")]
    public string ContentDescription { get; set; }

    /// <summary>
    /// Gets or Sets ContentUnit
    /// </summary>
    [DataMember(Name = "contentUnit")]
    public ContentUnitTypeGw ContentUnit { get; set; }

    /// <summary>
    /// content in contentUnit / ml: 10 * Abf_InhaltCl
    /// </summary>
    [DataMember(Name = "contentAmount")]
    public decimal? ContentAmount { get; set; }

    /// <summary>
    /// name of the bulk packaging
    /// </summary>
    [DataMember(Name = "bulkPackagingDescription")]
    public string BulkPackagingDescription { get; set; }

    /// <summary>
    /// number of unit in the bulk packaging / Gross_EinhProGG
    /// </summary>
    [DataMember(Name = "bulkPackagingContent")]
    public decimal? BulkPackagingContent { get; set; }

    /// <summary>
    /// vintage / Art_Jahrgang
    /// </summary>
    [DataMember(Name = "vintage")]
    public int? Vintage { get; set; }

    /// <summary>
    /// alcohol content % / Art_Volumen
    /// </summary>
    [DataMember(Name = "alcoholContent")]
    public decimal? AlcoholContent { get; set; }

    /// <summary>
    /// category / ArtKat_Bezeichnung
    /// </summary>
    [DataMember(Name = "category")]
    public string Category { get; set; }

    /// <summary>
    /// country / Land_Bezeichnung
    /// </summary>
    [DataMember(Name = "country")]
    public string Country { get; set; }

    /// <summary>
    /// region / Reg_Bezeichnung
    /// </summary>
    [DataMember(Name = "region")]
    public string Region { get; set; }

    /// <summary>
    /// domain / Art_Appellation
    /// </summary>
    [DataMember(Name = "domaine")]
    public string Domaine { get; set; }

    /// <summary>
    /// classification / Klass_Bezeichnung
    /// </summary>
    [DataMember(Name = "classification")]
    public string Classification { get; set; }

    /// <summary>
    /// cultivation / Wein_BemerkungAusbau
    /// </summary>
    [DataMember(Name = "cultivation")]
    public string Cultivation { get; set; }

    /// <summary>
    /// character / Wein_Charakter
    /// </summary>
    public string WineCharacter { get; set; }

    [NotMapped]
    [DataMember(Name = "character")]
    public string WineCharacterClean => RtfCleaner.Clean(WineCharacter);

    /// <summary>
    /// history / Wein_Geschichte
    /// </summary>
    public string History { get; set; }

    [NotMapped]
    [DataMember(Name = "history")]
    public string HistoryClean => RtfCleaner.Clean(History);

    /// <summary>
    /// vinification / Wein_Vinifikation
    /// </summary>
    public string Vinification { get; set; }

    [NotMapped]
    [DataMember(Name = "vinification")]
    public string VinificationClean => RtfCleaner.Clean(Vinification);

    /// <summary>
    /// consumation / Wein_Konsumhinweis
    /// </summary>
    public string Consumation { get; set; }

    [NotMapped]
    [DataMember(Name = "consumation")]
    public string ConsumationClean => RtfCleaner.Clean(Consumation);

    /// <summary>
    /// consumation / Wein_PromotionMessage
    /// </summary>
    public string PromotionMessage { get; set; }

    [NotMapped]
    [DataMember(Name = "promotionMessage")]
    public string PromotionMessageClean => RtfCleaner.Clean(PromotionMessage);

    /// <summary>
    /// terroir / Wein_Terroir
    /// </summary>
    public string Terroir { get; set; }

    [NotMapped]
    [DataMember(Name = "terroir")]
    public string TerroirClean => RtfCleaner.Clean(Terroir);

    /// <summary>
    /// sugar g/l / Wein_Restzucker
    /// </summary>
    [DataMember(Name = "sugar")]
    public decimal? Sugar { get; set; }

    /// <summary>
    /// acidity in per thousand / Wein_Saeure
    /// </summary>
    [DataMember(Name = "acidity")]
    public decimal? Acidity { get; set; }

    /// <summary>
    /// sold out / Art_Ausverkauft
    /// </summary>
    [DataMember(Name = "temporairySoldOut")]
    public bool TemporairySoldOut { get; set; }

    /// <summary>
    /// sold out / Art_BeschraenkteLieferbarkeit
    /// </summary>
    [DataMember(Name = "limitedAvailability")]
    public bool LimitedAvailability { get; set; }

    /// <summary>
    /// label bio / Wein_BioAnbau
    /// </summary>
    [DataMember(Name = "labelBio")]
    public bool LabelBio { get; set; }

    /// <summary>
    /// barrique / Wein_BarriqueAusbau
    /// </summary>
    [DataMember(Name = "barrique")]
    public bool Barrique { get; set; }

    /// <summary>
    /// vegan / Wein_Vegan
    /// </summary>
    [DataMember(Name = "vegan")]
    public bool Vegan { get; set; }

    /// <summary>
    /// label fine wine / Wein_FineWine
    /// </summary>
    [DataMember(Name = "fineWine")]
    public bool FineWine { get; set; }

    /// <summary>
    /// contains sulfit / Wein_Sulfitte
    /// </summary>
    [DataMember(Name = "containsSulfit")]
    public bool ContainsSulfit { get; set; }

    /// <summary>
    /// storage recommended until year / Wein_Lagerdauer
    /// </summary>
    [DataMember(Name = "storageRecommended")]
    public int? StorageRecommended { get; set; }

    /// <summary>
    /// storage recommended minimum year / Wein_LagerdauerMin
    /// </summary>
    [DataMember(Name = "storageMin")]
    public int? StorageMin { get; set; }

    /// <summary>
    /// storage recommended maximum year / Wein_LagerdauerMax
    /// </summary>
    [DataMember(Name = "storageMax")]
    public int? StorageMax { get; set; }

    /// <summary>
    /// temperature recommended / Wein_Trinktemparatur
    /// </summary>
    [DataMember(Name = "temperatureRecommended")]
    public int? TemperatureRecommended { get; set; }

    /// <summary>
    /// temperature recommended minimum / Wein_TrinktemparaturMin
    /// </summary>
    [DataMember(Name = "temperatureMin")]
    public int? TemperatureMin { get; set; }

    /// <summary>
    /// temperature recommended maximum / Wein_TrinktemparaturMax
    /// </summary>
    [DataMember(Name = "temperatureMax")]
    public int? TemperatureMax { get; set; }

    /// <summary>
    /// taste / Wein_Geschmack
    /// </summary>
    [DataMember(Name = "taste")]
    public string Taste { get; set; }

    /// <summary>
    /// taste / Wein_Weinstil
    /// </summary>
    [DataMember(Name = "style")]
    public string Style { get; set; }

    /// <summary>
    /// id of the producer / Art_ProdId
    /// </summary>
    [DataMember(Name = "producerId")]
    public int? ProducerId { get; set; }

    /// <summary>
    /// freight free / Art_Frachtfrei
    /// </summary>
    [DataMember(Name = "carriageFree")]
    public bool CarriageFree { get; set; }

    /// <summary>
    /// list of prices per price group
    /// </summary>
    [NotMapped]
    [DataMember(Name = "prices")]
    public List<PriceGw> Prices { get; set; }

    /// <summary>
    /// list of the grape compositions
    /// </summary>
    [NotMapped]
    [DataMember(Name = "compositions")]
    public List<CompositionGw> Compositions { get; set; }

    /// <summary>
    /// list of the ratings
    /// </summary>
    [NotMapped]
    [DataMember(Name = "ratings")]
    public List<RatingGw> Ratings { get; set; }

    /// <summary>
    /// list of the recommendations
    /// </summary>
    [NotMapped]
    [DataMember(Name = "recommendations")]
    public List<RecommendationGw> Recommendations { get; set; }
}