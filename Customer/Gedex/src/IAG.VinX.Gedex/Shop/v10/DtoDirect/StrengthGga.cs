using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Gedex.Shop.v10.DtoDirect;

/// <summary>
/// Wine strength (Schwere), customer extensions
/// </summary>
[DataContract]
[DisplayName("StrengthGga")]
[TableCte(@"
        WITH StrengthGga
        AS 
        (
        SELECT 
            ID              AS Id, 
            Bezeichnung     AS Designation
        FROM Schwere
        )
    ")]
public class StrengthGga
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