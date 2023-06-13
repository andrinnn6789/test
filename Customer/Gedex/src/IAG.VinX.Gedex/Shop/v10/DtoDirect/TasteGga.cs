using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Gedex.Shop.v10.DtoDirect;

/// <summary>
/// Taste (Geschmack), customer extensions
/// </summary>
[DataContract]
[DisplayName("TasteGga")]
[TableCte(@"
        WITH TasteGga
        AS 
        (
        SELECT 
            ID              AS Id, 
            Bezeichnung     AS Designation
        FROM Geschmack
        )
    ")]
public class TasteGga
{
    /// <summary>
    /// Primary key
    /// </summary>
    [DataMember(Name="id")]
    public int Id { get; set; }

    /// <summary>
    /// Designation, Bezeichnung
    /// </summary>
    [DataMember(Name= "designation")]
    public string Designation { get; set; }
}