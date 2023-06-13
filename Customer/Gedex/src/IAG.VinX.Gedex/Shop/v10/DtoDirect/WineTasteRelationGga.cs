using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Gedex.Shop.v10.DtoDirect;

/// <summary>
/// Wine taste relation (ArtikelGeschmack), customer extensions
/// </summary>
[DataContract]
[DisplayName("WineTasteRelationGga")]
[TableCte(@"
        WITH WineTasteRelationGga
        AS 
        (
        SELECT 
            ID              AS Id, 
            WeininfoID      AS WineId,
            GeschmackID     AS TasteId
        FROM ArtikelGeschmack 
        )
    ")]
public class WineTasteRelationGga
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
    /// ID of taste, GeschmackID
    /// </summary>
    [DataMember(Name = "tasteId")]
    public int? TasteId { get; set; }
}