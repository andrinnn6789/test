using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Gedex.Shop.v10.DtoDirect;

/// <summary>
/// Wine match relation (ArtikelPasstZu), customer extensions
/// </summary>
[DataContract]
[DisplayName("WineMatchRelationGga")]
[TableCte(@"
        WITH WineMatchRelationGga
        AS 
        (
        SELECT 
            ID              AS Id, 
            WeininfoID      AS WineId,
            PasstZuID       AS MatchId
        FROM ArtikelPasstZu 
        )
    ")]
public class WineMatchRelationGga
{
    /// <summary>
    /// Primary key
    /// </summary>
    [DataMember(Name="id")]
    public int Id { get; set; }

    /// <summary>
    /// ID of wine, WeininfoID
    /// </summary>
    [DataMember(Name = "wineId")]
    public int? WineId { get; set; }

    /// <summary>
    /// ID of match, PasstZuID
    /// </summary>
    [DataMember(Name = "matchId")]
    public int? MatchId { get; set; }
}