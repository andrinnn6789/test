using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Gedex.Shop.v10.DtoDirect;

/// <summary>
/// amount grape-types (AnzahlRebsorten), customer extensions
/// </summary>
[DataContract]
[DisplayName("GrapeAmountGga")]
[TableCte(@"
        WITH GrapeAmountGga
        AS 
        (
        SELECT 
            ID              AS Id, 
            Bezeichnung     AS Designation
        FROM AnzahlRebsorten
        )
    ")]
public class GrapeAmountGga
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