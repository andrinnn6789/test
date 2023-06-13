using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Gedex.Shop.v10.DtoDirect;

/// <summary>
/// Match (PasstZu), customer extensions
/// </summary>
[DataContract]
[DisplayName("MatchGga")]
[TableCte(@"
        WITH MatchGga
        AS 
        (
        SELECT 
            ID              AS Id, 
            Bezeichnung     AS Designation
        FROM PasstZu 
        )
    ")]
public class MatchGga
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