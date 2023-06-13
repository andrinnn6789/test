using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Boucherville.Shop.v10.DtoDirect;

/// <summary>
/// Predicate (Praedikat), customer extensions
/// </summary>
[DataContract]
[DisplayName("PredicateBov")]
[TableCte(@"
        WITH PredicateBov
        AS 
        (
        SELECT 
            Prae_ID                 AS Id, 
            Prae_Bezeichnung        AS Designation,
            Prae_Sort               AS Sort
        FROM Praedikat 
        )
    ")]
public class PredicateBov
{
    /// <summary>
    /// Primary key
    /// </summary>
    [DataMember(Name="id")]
    public int Id { get; set; }

    /// <summary>
    /// Designation, Prae_Bezeichnung
    /// </summary>
    [DataMember(Name= "designation")]
    public string Designation { get; set; }

    /// <summary>
    /// sort field for lists, Prae_Sort
    /// </summary>
    [DataMember(Name = "sort")]
    public string Sort { get; set; }
}