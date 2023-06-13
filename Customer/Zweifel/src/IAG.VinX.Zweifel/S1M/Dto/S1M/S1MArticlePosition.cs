using System;

using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace IAG.VinX.Zweifel.S1M.Dto.S1M;

[TableCte(@"
WITH S1MArticlePosition AS (
      SELECT
        BelPos_ID                                           AS          Id,
        BelPos_BelegId                                      AS          DocumentId,
        BelPos_Position                                     AS          PositionOnDocument,
        ArtPosArticle.Art_Id                                AS          ArticleId,    
        ArtPosArticle.Art_Artikelnummer                     AS          ArticleNumber,
        ArtPosArticle.Art_Jahrgang                          AS          ArticleVintage,
        ArtPosArticle.Art_Artikeltyp                        AS          ArticleType,
        ArtPosArticle.Art_Bezeichnung                       AS          ArticleName,
        Abf_ID                                              AS          ArticleFillingId,
        Abf_BezeichnungWeb                                  AS          ArticleFilling,
        Abf_ArtID                                           AS          ArticleFillingArticleId,
        Gross_ArtId                                         AS          ArticleGGId,
        Abf_Kuerzel                                         AS          ArticleFillingShortform,
        Gross_BezeichnungWeb                                AS          ArticleGGName,
        ABS(Gross_AnbruchErlaubt)                           AS          ArticleGGBreakable,
        Gross_EinhProGG                                     AS          ArticleGGUnitsPerGG,
        BelPos_MengeAbf                                     AS          ArticleQuantity,
        BelPos_MengeGG                                      AS          ArticleQuantityGG,
        CASE ABS(Adr_LieferscheinMitPreis) 
            WHEN 1 THEN BelPos_Preis
            ELSE 0
        END                                                 AS          ArticlePrice,
        BelPos_KundenRabatt                                 AS          ArticleDiscount,
        BelPos_BerechnungsartKR                             AS          ArticleDiscountCalculationKind,
        ArtPosArticle.Art_Gewichtsanteil * BelPos_MengeAbf  AS          Weight,
        Klass_Bezeichnung                                   AS          WineClassification,
        Prod_Bezeichnung                                    AS          ProducerName,
        Prod_IstMarke                                       AS          ProducerIsBrand,
        Zyk_Bezeichnung                                     AS          CycleDesignation
        
    FROM Artikelposition
    JOIN Artikel ArtPosArticle                              ON          ArtPosArticle.Art_ID = BelPos_ArtikelID
    JOIN Abfuellung                                         ON          Abf_ID = Art_AbfID
    LEFT JOIN WeinInfo                                      ON          Wein_ID = Art_WeininfoId
    LEFT JOIN Klassifikation                                ON          Wein_KlassifikationId = Klass_ID
    LEFT JOIN Produzent                                     ON          Prod_ID = Art_ProdID
    LEFT JOIN Grossgebinde                                  ON          Gross_ID = Art_GrossID
    JOIN Beleg                                              ON          Bel_ID = BelPos_BelegID
    JOIN Adresse                                            ON          Adr_ID = CASE
                                                                           WHEN Bel_LieferAdresseID IS NOT NULL THEN Bel_LieferAdresseID
                                                                           ELSE Bel_AdrID
                                                                        END 
    JOIN Zyklus                                             ON          Zyk_ID = Art_ZyklusID                                                       
    GROUP BY Id, DocumentId, PositionOnDocument, ArticleId, ArticleNumber, ArticleVintage, ArticleType, ArticleName, ArticleFillingId,  ArticleFilling, ArticleFillingArticleId, ArticleGGId, ArticleFillingShortform, ArticleGGName, ArticleGGBreakable, ArticleGGUnitsPerGG, ArticleQuantity, ArticleQuantityGG,
                ArticlePrice, ArticleDiscount, ArticleDiscountCalculationKind, Weight, WineClassification, ProducerName, ProducerIsBrand, Zyk_Bezeichnung
)
")]
public class S1MArticlePosition
{
    public int Id { get; set; }

    [JsonIgnore] public int DocumentId { get; set; }

    public int PositionOnDocument { get; set; }
    public int ArticleId { get; set; }
    public decimal ArticleNumber { get; set; }
    public int ArticleVintage { get; set; }
    public int ArticleType { get; set; }
    public string ArticleName { get; set; }
    public string ArticleName1 => SplitArticleName(ArticleName, 1);
    public string ArticleName2 => SplitArticleName(ArticleName, 2);
    public string ArticleName3 => SplitArticleName(ArticleName, 3);
    public int ArticleFillingId { get; set; }
    public string ArticleFilling { get; set; }
    public int ArticleFillingArticleId { get; set; }
    public int ArticleGgId { get; set; }
    public string ArticleFillingShortform { get; set; }
    public string ArticleGgName { get; set; }
    public bool ArticleGgBreakable { get; set; }
    public decimal ArticleGgUnitsPerGg { get; set; }
    public int ArticleQuantity { get; set; }
    public int ArticleQuantityGg { get; set; }
    public decimal ArticlePrice { get; set; }
    public decimal ArticleDiscount { get; set; }
    public int ArticleDiscountCalculationKind { get; set; }
    public decimal Weight { get; set; }

    [CanBeNull] public string WineClassification { get; set; }

    public string ProducerName { get; set; }
    public bool ProducerIsBrand { get; set; }
    public string CycleDesignation { get; set; }

    private string SplitArticleName(string articleName, int lineNumber)
    {
        var articleNames = articleName.Split(Environment.NewLine);

        if (articleNames.Length == 0 || articleNames.Length < lineNumber)
            return string.Empty;

        switch (lineNumber)
        {
            case 1:
                return articleNames[0];
            case 2:
                return articleNames[1];
            case 3:
                return articleNames[2];
            default:
                return string.Empty;
        }
    }
}