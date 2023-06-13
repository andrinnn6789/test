using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

[DisplayName("Carrier")]
[Table("SpeditionsCode")]
public class CarrierGw
{
    [Column("Sped_Id")]
    public int Id { get; set; }

    [Column("Sped_Bezeichnung")]
    public string Name { get; set; }
}