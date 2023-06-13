using System;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Gw;

/// <summary>
/// data structure to transmit new addresses and changes
/// </summary>
public class BasketGw<TOnlineAddressGw>: Basket.Dto.Basket<TOnlineAddressGw> 
    where TOnlineAddressGw: OnlineAddressGw, new()
{ 
    /// <summary>
    /// desired delivery time input, optional.
    /// </summary>
    public string DeliveryTime { get; set; }

    /// <summary>
    /// desired delivery location input, optional.
    /// </summary>
    public string DeliveryLocation { get; set; }

    /// <summary>
    /// desired delivery location remark input, optional.
    /// </summary>
    public string DeliveryLocationRemark { get; set; }

    /// <summary>
    /// id of the ordering contact input, optional.
    /// </summary>
    public int? OrderingContactId { get; set; }

    /// <summary>
    /// id of the carrier, reference to /carrier, input, required
    /// </summary>
    public int CarrierId { get; set; }

    /// <summary>
    /// description of the CRIF-Check
    /// </summary>
    public string CrifDescription { get; set; }

    /// <summary>
    /// date of the CRIF-Check
    /// </summary>
    public DateTime? CrifCheckDate { get; set; }
}