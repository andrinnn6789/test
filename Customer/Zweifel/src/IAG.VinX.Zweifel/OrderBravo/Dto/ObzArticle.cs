using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Basket.Enum;
using IAG.VinX.OrderBravo.Dto.Interfaces;

namespace IAG.VinX.Zweifel.OrderBravo.Dto;

[TableCte(@"
        WITH ObzArticle AS
        (
            SELECT
                MainArticle.Art_Artikelnummer                                                   AS ArticleNumber,
                MainArticle.Art_Bezeichnung                                                     AS ArticleDescription,
                MainArticle.Art_Jahrgang                                                        AS Year,
                MainArticle.Art_Volumen                                                         AS Volume,
                Prod_Bezeichnung                                                                AS ProducerName,
                Abf_ID                                                                          AS FillingId,
                Abf_Kuerzel                                                                     AS FillingDescription,
                Abf_BezeichnungWeb                                                              AS FillingWebDescription,
                Gross_BezeichnungWeb                                                            AS PackingUnitDescription,
                Gross_ID                                                                        AS PackingUnitId,
                ABS(Gross_Anbrucherlaubt)                                                       AS PackingUnitBreakageAllowed,
                Gross_EinhProGG                                                                 AS PackingUnitQtyPerUnit,
                Sel_ID                                                                          AS SortimentId,
                Zyk_ID                                                                          AS CycleId,
                ReplacementArticle.Art_Artikelnummer                                            AS ReplacementArticleNumber,
                SUM(Lagbest_Bestand)                                                            AS StockAmount,
                ArtKat_Bezeichnung                                                              AS ArticleCategory,
                ArtKat_Artikeltyp                                                               AS ArticleType,
                CASE ArtKat_Artikeltyp WHEN 3 THEN 1 WHEN 4 THEN 1 WHEN 7 THEN 1 ELSE 0 END     AS PackingUnitOnly,
                MainArticle.Art_ArtikelFuerKunde                                                AS CustomerNumbers
            
                FROM Artikel MainArticle
                LEFT OUTER JOIN Artikel ReplacementArticle ON MainArticle.Art_ErsatzArtikelID = ReplacementArticle.Art_ID
                LEFT OUTER JOIN Produzent ON Produzent.Prod_ID = MainArticle.Art_ProdID
                JOIN Abfuellung ON Abfuellung.Abf_ID = MainArticle.Art_AbfID
                JOIN Grossgebinde ON Grossgebinde.Gross_ID = MainArticle.Art_GrossID
                JOIN ArtikelSelektionscode ON ArtikelSelektionscode.ASelCode_ArtID = MainArticle.Art_ID
                JOIN Selektionscode ON Selektionscode.Sel_ID = ArtikelSelektionscode.ASelCode_SelID
                JOIN Zyklus ON Zyklus.Zyk_ID = MainArticle.Art_ZyklusID
                LEFT OUTER JOIN Lagerbestand ON Lagerbestand.Lagbest_ArtikelID = MainArticle.Art_ID
                JOIN Artikelkategorie ON Artikelkategorie.ArtKat_ID = MainArticle.Art_AKatID

                WHERE SortimentId = 13521
                
                GROUP BY ArticleNumber, ArticleDescription, Year, Volume, ProducerName, FillingId, FillingDescription, 
                FillingWebDescription, PackingUnitId, PackingUnitDescription, PackingUnitBreakageAllowed, PackingUnitQtyPerUnit, 
                SortimentId, CycleId, ReplacementArticleNumber, ArticleCategory, ArticleType, PackingUnitOnly, CustomerNumbers
        )
    ")]
public class ObzArticle : IObArticle
{
    public decimal ArticleNumber { get; set; }
    public string ArticleDescription { get; set; }
    public int? Year { get; set; }
    public decimal Volume { get; set; }
    public string ProducerName { get; set; }
    public int FillingId { get; set; }
    public string FillingDescription { get; set; }
    public string FillingWebDescription { get; set; }
    public string PackingUnitDescription { get; set; }
    public int PackingUnitId { get; set; }
    public bool PackingUnitBreakageAllowed { get; set; }
    public int PackingUnitQtyPerUnit { get; set; }
    public int SortimentId { get; set; }
    public int CycleId { get; set; }
    public decimal? ReplacementArticleNumber { get; set; }
    public int StockAmount { get; set; }
    public string ArticleCategory { get; set; }
    public ArticleCategoryKind ArticleType { get; set; }
    public bool PackingUnitOnly { get; set; }
    public string CustomerNumbers { get; set; }
    public decimal CustomerNumber { get; set; }
}