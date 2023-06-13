using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

namespace IAG.VinX.Schüwo.Shop.v10.DtoDirect;

/// <summary>
/// Countries
/// </summary>
[DataContract]
[DisplayName("CountrySw")]
[TableCte(@"
        WITH CountrySw
        AS 
        (
        SELECT 
            Land_ID             AS Id, 
            Land_Bezeichnung    AS Designation,
            Land_Code           AS Code,
            Land_Sort           AS SortOrder,
    
            Land_Beschreibung   AS Description

        FROM Land
        )
    ")]
public class CountrySw: CountryV10
{
    /// <summary>
    /// Description of the country
    /// </summary>
    [DataMember(Name= "description")]
    public string Description { get; set; }
}