using IAG.Common.DataLayerSybase.Attribute;

using JetBrains.Annotations;

namespace IAG.VinX.Schüwo.SV.Dto;

[TableCte(@"
    WITH
    ArticleActive (
        ArtNr
    )
    AS
    (
        SELECT DISTINCT
            Art_Artikelnummer
        FROM Artikel
        WHERE Art_Aktiv = -1 OR (Art_Aktiv = 0 AND Art_DatumMutation > NOW() - 30) 
    )
    ")]
[UsedImplicitly]
public class ArticleActive
{
    public int ArtNr { get; [UsedImplicitly] set; }
}