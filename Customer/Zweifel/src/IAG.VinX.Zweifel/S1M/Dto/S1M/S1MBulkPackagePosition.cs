using System.ComponentModel.DataAnnotations;

using IAG.Common.DataLayerSybase.Attribute;

using Newtonsoft.Json;

namespace IAG.VinX.Zweifel.S1M.Dto.S1M;

[TableCte(@"
WITH S1MBulkPackagePosition AS (
    SELECT
        GebPos_ID                                           AS          ID,
        GebPos_BelegID                                      AS          DocumentId,
        Art_Artikelnummer                                   AS          ArticleNumber,
        Art_Bezeichnung                                     AS          ArticleDescription,
        GebPos_Geliefert                                    AS          QuantityDelivered,
        GebPos_Retour                                       AS          QuantityReturned
    FROM GebindePosition
    JOIN Artikel ON Art_ID = GebPos_ArtikelID
)
")]
public class S1MBulkPackagePosition
{
    [Key]
    public int Id { get; set; }

    [JsonIgnore]
    public int DocumentId { get; set; }

    public decimal ArticleNumber { get; set; }
    public string ArticleDescription { get; set; }
    public int QuantityDelivered { get; set; }
    public int QuantityReturned { get; set; }
}