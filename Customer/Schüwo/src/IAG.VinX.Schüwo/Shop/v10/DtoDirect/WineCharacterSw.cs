using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;

namespace IAG.VinX.Schüwo.Shop.v10.DtoDirect;

/// <summary>
/// Win characters, customer extensions
/// </summary>
[DataContract]
[DisplayName("CharacterSw")]
[TableCte(@"
        WITH WineCharacterSw
        AS 
        (
        SELECT 
            Char_ID              AS Id, 
            Char_Bezeichnung     AS Designation
        FROM Charaktereigenschaft
        )
    ")]
public class WineCharacterSw
{
    /// <summary>
    /// Primary key
    /// </summary>
    [DataMember(Name="id")]
    public int Id { get; set; }

    /// <summary>
    /// Unique designation, Char_Bezeichnung
    /// </summary>
    [DataMember(Name= "designation")]
    public string Designation { get; set; }
}