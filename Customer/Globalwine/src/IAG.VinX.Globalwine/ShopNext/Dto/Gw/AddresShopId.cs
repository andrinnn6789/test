using System.ComponentModel.DataAnnotations.Schema;

using IAG.VinX.Globalwine.ShopNext.Dto.Interfaces;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Gw;

/// <summary>
/// Helper for finding shop id
/// </summary>
[Table("Adresse")]
public class AddresShopId: IShopId
{ 
    /// <summary>
    /// id in VinX
    /// </summary>
    [Column("Adr_Id")]
    public int? Id { get; set; }

    /// <summary>
    /// id in the shop
    /// </summary>
    [Column("Adr_ShopId")]
    public string ShopId { get; set; }
}