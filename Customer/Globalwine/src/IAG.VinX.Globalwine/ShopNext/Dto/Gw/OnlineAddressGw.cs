using System;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Globalwine.ShopNext.Dto.Enum;
using IAG.VinX.Globalwine.ShopNext.Dto.Interfaces;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Gw;

/// <summary>
/// data structure to transmit new addresses and changes
/// </summary>
[Table("OnlineAdresse")]
public class OnlineAddressGw: OnlineAddress, IShopId
{ 
    /// <summary>
    /// timestamp utc last change, output
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column("Adr_ChangedOn")]
    public DateTime? ChangedOn { get; set; }

    /// <summary>
    /// free text field input, optional.
    /// </summary>
    [Column("Adr_Adressstruktur")]
    public AddressstructureGw CustomerCategory { get; set; }

    /// <summary>
    /// id in the shop
    /// </summary>
    [Column("Adr_ShopId")]
    public string ShopId { get; set; }

    /// <summary>
    /// salutation id (must be known in the shop) / Adr_AnrID
    /// </summary>
    [Column("Adr_KontaktAnrID")]
    public int? ContactSalutationId { get; set; }

    /// <summary>
    /// first name of the contact
    /// </summary>
    [Column("Adr_KontaktVorname")]
    public string ContactFirstName { get; set; }

    /// <summary>
    /// last name of the contact
    /// </summary>
    [Column("Adr_KontaktName")]
    public string ContactLastName{ get; set; }

    /// <summary>
    /// Birthday / Adr_Geburtsdatum
    /// </summary>
    [Column("Adr_Geburtsdatum")]
    public DateTime? Birthday { get; set; }
}