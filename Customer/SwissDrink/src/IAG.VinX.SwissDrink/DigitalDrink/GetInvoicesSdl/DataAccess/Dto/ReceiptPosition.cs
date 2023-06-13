using System.ComponentModel.DataAnnotations.Schema;

using IAG.VinX.Basket.Enum;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess.Dto;

[Table("Artikelposition")]
public class ReceiptPosition
{
    [Column("BelPos_BelegID")]
    public int ReceiptId { get; set; }

    [Column("BelPos_Position")]
    public int SequenceNumber { get; set; }

    [Column("BelPos_ArtikelID")]
    public int ArticleId { get; set; }

    [Column("BelPos_AbfID")]
    public int? FillingId { get; set; }

    [Column("BelPos_GrossID")]
    public int? PackageId { get; set; }

    [Column("BelPos_Text")]
    public string Text { get; set; }

    [Column("BelPos_MengeGG")]
    public decimal QuantityPackages { get; set; }

    [Column("BelPos_MengeAbf")]
    public decimal QuantityUnits { get; set; }

    [Column("BelPos_MWSTID")]
    public int? VatId { get; set; }

    [Column("BelPos_MWSTProzent")]
    public decimal VatRate { get; set; }

    [Column("BelPos_Preis")]
    public decimal UnitPrice { get; set; }

    [Column("BelPos_MWSTProzentInPreis")]
    public decimal VatRateInPrice { get; set; }

    [Column("BelPos_IstNetto")]
    public bool IsNet { get; set; }

    [Column("BelPos_PreisErmittlung")]
    public PriceCalculationKind PriceCalculationKind { get; set; }

    [Column("BelPos_DDTotal")]
    public decimal DdTotal { get; set; }

    [Column("BelPos_DDPreis")]
    public decimal DdPrice { get; set; }
}