using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

namespace IAG.VinX.Schüwo.Shop.v10.DtoDirect;

/// <summary>
/// E-commerce grouping of the articles
/// </summary>
[DataContract]
[DisplayName("ECommerceGroupSw")]
[TableCte(@"
        WITH EcommerceGroupSw
        AS 
        (
        SELECT 
            ArtEGrp_ID              AS Id, 
            ArtEGrp_Bezeichnung     AS Designation,
            ArtEGrp_ObergruppeID    AS ParentId,
            ArtEGrp_Sort            AS SortKey,

            ArtEGrp_Beschreibung   AS Description

        FROM ArtikelEGruppe
        )
    ")]
public class EcommerceGroupSw: ECommerceGroupV10
{
    /// <summary>
    /// Description of the e-commerce group
    /// </summary>
    [DataMember(Name= "description")]
    public string Description { get; set; }
}