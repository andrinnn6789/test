using System;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.Common.DataLayerSybase.Attribute;
using IAG.VinX.Shop.v10.DtoDirect;

namespace IAG.VinX.Schüwo.Shop.v10.DtoDirect;

/// <summary>
/// Selection codes with customer extensions / SelektionsCode
/// </summary>
[DataContract]
[DisplayName("SelectionCodeSw")]
[TableCte(@"
        WITH SelectionCodeSw
        AS 
        (
        SELECT 
            Sel_ID                       AS Id, 
            Sel_Bezeichnung              AS Designation,
            Sel_SelektionscodeID         AS ParentId,

            ABS(Sel_WebShopExport)       AS ExportShop,
            Sel_WebShopVonDatum          AS ShopFromDate,
            Sel_WebShopBisDatum          AS ShopUntilDate
        FROM SelektionsCode
        )
    ")]
public class SelectionCodeSw: SelectionCodeV10
{
    /// <summary>
    /// Export to shop, Sel_WebShopExport
    /// </summary>
    [DataMember(Name= "exportShop")]
    public bool ExportShop { get; set; }

    /// <summary>
    /// Export to shop date from, Sel_WebShopVonDatum
    /// </summary>
    [DataMember(Name= "shopFromDate")]
    public DateTime? ShopFromDate { get; set; }

    /// <summary>
    /// Export to shop date until, Sel_WebShopBisDatum
    /// </summary>
    [DataMember(Name= "shopUntilDate")]
    public DateTime? ShopUntilDate { get; set; }
}