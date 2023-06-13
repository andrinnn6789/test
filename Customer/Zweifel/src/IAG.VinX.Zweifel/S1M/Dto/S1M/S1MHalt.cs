using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.Attribute;

using Newtonsoft.Json;

namespace IAG.VinX.Zweifel.S1M.Dto.S1M;

[TableCte(@"
WITH S1MHalt AS (
    SELECT
        Station_ID              AS StationId,
        Station_AuslieferungID  AS DeliveryId,
        Station_AdresseId       AS AddressId,
        Station_BelegID         AS DocumentId,
        Station_Position        AS HaltPosition
    FROM Station
)
")]
public class S1MHalt
{
    public int StationId { get; set; }

    [JsonIgnore]
    public int DeliveryId { get; set; }

    [JsonIgnore]
    public int AddressId { get; set; }

    [JsonIgnore]
    public int DocumentId { get; set; }

    public int HaltPosition { get; set; }

    [NotMapped]
    public S1MAddress S1MAddress { get; set; }

    [NotMapped]
    public S1MDocument S1MDocument { get; set; }

    [NotMapped]
    public decimal Weight { get; set; }
}