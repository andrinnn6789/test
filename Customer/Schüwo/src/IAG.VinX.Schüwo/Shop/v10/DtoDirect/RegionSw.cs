using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

namespace IAG.VinX.Schüwo.Shop.v10.DtoDirect;

/// <summary>
/// Regions
/// </summary>
[DataContract]
[DisplayName("RegionSw")]
[TableCte(@"
        WITH RegionSw
        AS 
        (
        SELECT             
            Reg_ID              AS Id, 
            Reg_Bezeichnung     AS Designation,
            Reg_LandID          AS CountryId,
            Reg_RegID           AS ParentId,
            Reg_Sort            AS SortOrder,

            Reg_Beschreibung    AS Description

        FROM Region
        )
    ")]
public class RegionSw: RegionV10
{
    /// <summary>
    /// Description, Reg_Beschreibung
    /// </summary>
    [DataMember(Name= "description")]
    public string Description { get; set; }
}