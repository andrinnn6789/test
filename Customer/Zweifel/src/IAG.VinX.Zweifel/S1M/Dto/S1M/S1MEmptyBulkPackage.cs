using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Zweifel.S1M.Dto.S1M;

[TableCte(@"
WITH S1MEmptyBulkPackage AS (
    SELECT 
        Art_ID            AS Id,
        Art_Artikelnummer AS ArticleNumber,
        Art_Bezeichnung   AS ArticleDescription,
        Art_SortRetouren  AS ArticleSortingReturns

    FROM Artikel WHERE Art_Artikeltyp = 1
)
")]
public class S1MEmptyBulkPackage
{
    public int Id { get; set; }
    public decimal ArticleNumber { get; set; }
    public string ArticleDescription { get; set; }
    public int ArticleSortingReturns { get; set; }
}