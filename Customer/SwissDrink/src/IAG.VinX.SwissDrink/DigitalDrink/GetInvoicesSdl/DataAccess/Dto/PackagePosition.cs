using System.ComponentModel.DataAnnotations.Schema;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess.Dto;

[Table("Gebindeposition")]
public class PackagePosition
{
    [Column("GebPos_BelegID")]
    public int ReceiptId { get; set; }

    [Column("GebPos_ArtikelID")]
    public int ArticleId { get; set; }

    [Column("GebPos_Geliefert")]
    public decimal QuantityDelivered { get; set; }

    [Column("GebPos_Retour")]
    public decimal QuantityReturned { get; set; }

    [Column("GebPos_MWSTID")]
    public int? VatId { get; set; }

    [Column("GebPos_Preis")]
    public decimal UnitPrice { get; set; }

    [Column("GebPos_MWSTProzent")]
    public decimal VatRate { get; set; }

    [Column("GebPos_MWSTProzentInPreis")]
    public decimal VatRateInPrice { get; set; } = 0;

    [Column("GebPos_DDPreis")]
    public decimal DdPrice { get; set; }
}