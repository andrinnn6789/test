using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Gw;

[TableCte(@"
        WITH ArticleHelperGw AS
        (
        SELECT
            Art_Id                      AS Id,
            Art_Ausverkauft             AS TemporairySoldOut,
            ABS(Art_Frachtkostenfrei)   AS FreigthFree
        FROM Artikel
        )
        ")]
public class ArticleHelperGw
{
    public int Id { get; set; }

    public bool TemporairySoldOut { get; set; }

    public bool FreigthFree { get; set; }
}