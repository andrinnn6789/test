using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.VinX.BaseData.Dto.Enums;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess.Dto;

[Table("Beleg")]
public class Receipt
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Bel_Id")]
    public int Id { get; set; }

    [Column("Bel_MWSTVerrechnung")]
    public VatCalculationType VatCalculation { get; set; }

    [Column("Bel_Belegtyp")]
    public ReceiptType Type { get; set; }

    [Column("Bel_GLNLieferant")]
    public decimal? WholeSalerGln { get; set; }

    [Column("Bel_ExterneID")]
    public string ExternalId { get; set; }

    [Column("Bel_BelegNr")]
    public int Number { get; set; }

    [Column("Bel_Datum")]
    public DateTime ReceiptDate { get; set; }

    [Column("Bel_FaelligkeitNetto")]
    public DateTime DueDate { get; set; }

    [Column("Bel_LieferantID")]
    public int SupplierId { get; set; }

    [Column("Bel_KonditionenAdresseID")]
    public int? ConditionAddressId { get; set; }

    [Column("Bel_AdrID")]
    public int AddressId { get; set; }

    [Column("Bel_RechnungsAdresseID")]
    public int? InvoiceAddressId { get; set; }

    [Column("Bel_LieferAdresseID")]
    public int? DeliveryAddressId { get; set; }

    [Column("Bel_MitarbeiterID")]
    public int? SalesPersonId { get; set; } = 1;

    [Column("Bel_BestellDatum")]
    public DateTime? OrderDate { get; set; }

    [Column("Bel_ZahlkondID")]
    public int PaymentConditionId { get; set; }

    [Column("Bel_WaehrungID")]
    public int CurrencyId { get; set; }

    [Column("Bel_WaehrungKurs")]
    public decimal ExchangeRate { get; set; }

    [Column("Bel_Belegstatus")]
    public ReceiptStatusEnum ReceiptState { get; set; }

    [Column("Bel_ZahlungswegID")]
    public int? PaymentMeansId { get; set; }

    [Column("Bel_Sprache")]
    public string Language { get; set; }

    [Column("Bel_Aktionsberechtigung")]
    public int DiscountKind { get; set; }

    [Column("Bel_DatumPreise")]
    public DateTime PricingDate { get; set; }

    [Column("Bel_DatumRE")]
    public DateTime InvoiceDate { get; set; }

    [Column("Bel_BelegNrLieferant")]
    public string DeliveryNumber { get; set; }

    [Column("Bel_DatumLS")]
    public DateTime DeliveryDate { get; set; }

    [Column("Bel_DDTotal")]
    public decimal TotalDd { get; set; }

    [Column("Bel_ArtikelTotal")]
    public decimal TotalArticles { get; set; }

    [Column("Bel_GebindeTotal")]
    public decimal TotalPackages { get; set; }

    [Column("Bel_GebuehrenTotal")]
    public decimal TotalFee { get; set; }

    [Column("Bel_RabattTotal")]
    public decimal TotalDiscounts { get; set; }

    [Column("Bel_MWSTTotal")]
    public decimal TotalVat { get; set; }

    [Column("Bel_GesamtNetto")]
    public decimal TotalNet { get; set; }

    [Column("Bel_GesamtMitSkonto")]
    public decimal TotalNetSkonto { get; set; }

    [Column("Bel_BelegID")]
    public int? InvoiceId { get; set; }

    [Column("Bel_ESRReferenz")]
    public string PaymenReference { get; set; }

    [Column("Bel_KGrpPreisID")]
    public int PriceGroupId { get; set; }

    [NotMapped]
    public List<ReceiptPosition> Positions { get; } = new ();

    [NotMapped]
    public List<PackagePosition> Packages { get; } = new ();

    [NotMapped]
    public bool VatContextSell => Type switch
    {
        ReceiptType.CreditNote => false,
        ReceiptType.Invoice => true,
        _ => throw new ApplicationException($"vat context for receipt type {Type} not implemented")
    };
}