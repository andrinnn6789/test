using System;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Common.DataLayerSybase.AtlasType;
using IAG.VinX.BaseData.Dto.Sybase;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Gw;

/// <summary>
/// data structure of online orders
/// </summary>
[Table("OnlineBestellung")]
public class OnlineOrderGw: OnlineOrder
{ 
    /// <summary>
    /// id of the ordering contact input, optional.
    /// </summary>
    [Column("Best_BestellerID")]
    public int? OrderingContactId { get; set; }

    [Column("Best_SpedID")]
    public int CarrierId { get; set; }

    [Column("Best_BezeichnungCRIF")]
    public string CrifDescription { get; set; }

    [Column("Best_DatumCRIF")]
    public DateTime? CrifCheckDate { get; set; }

    /// <summary>
    /// Adhoc-overrides 
    /// </summary>
    [Column("Best_AdHocAdresse")]
    public string AdHocAddress { get; set; }
    [Column("Best_AdHocLand")]
    public string AdHocCountry { get; set; }
    [Column("Best_AdHocOrt")]
    public string AdHocCity { get; set; }
    [Column("Best_AdHocPLZ")]
    public string AdHocZip { get; set; }
    [Column("Best_AdHocStrasse")]
    public string AdHocStreet { get; set; }
    [Column("Best_AdHocTel")]
    public string AdHocPhoneNumber { get; set; }
    [Column("Best_HatAdHocLieferadresse")]
    public AtlasBoolean HasAdhoc { get; set; }
}