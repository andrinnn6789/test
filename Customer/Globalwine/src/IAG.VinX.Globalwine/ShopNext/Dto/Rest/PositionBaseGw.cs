using System.Runtime.Serialization;

using IAG.VinX.Globalwine.ShopNext.Dto.Enum;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

[DataContract]
public class PositionBaseGw
{
    /// <summary>
    /// Gets or Sets PosType
    /// </summary>
    [DataMember(Name="posType")]
    public OrderPositionTypeGw PosType { get; set; }

    /// <summary>
    /// number of the position, must be unique
    /// </summary>
    [DataMember(Name="posNumber")]
    public string PosNumber { get; set; }

    /// <summary>
    /// id of the article in VinX, input required
    /// </summary>
    [DataMember(Name="articleId")]
    public int ArticleId { get; set; }

    /// <summary>
    /// ordered quantity in base units of the article
    /// </summary>
    [DataMember(Name="orderedQuantity")]
    public decimal OrderedQuantity { get; set; }

    /// <summary>
    /// billed quantity in base units of the article
    /// </summary>
    [DataMember(Name="billedQuantity")]
    public decimal BilledQuantity { get; set; }

    /// <summary>
    /// number of the article in VinX, output
    /// </summary>
    [DataMember(Name="articleNumber")]
    public string ArticleNumber { get; set; }

    /// <summary>
    /// description of the position, equals the description of the article, output
    /// </summary>
    [DataMember(Name="description")]
    public string Description { get; set; }

    /// <summary>
    /// Gets or Sets PriceCalculationRule
    /// </summary>
    [DataMember(Name="priceCalculationRule")]
    public PriceCalculationKindGw PriceKind { get; set; }

    /// <summary>
    /// unit price excl VAT, in-, output
    /// </summary>
    [DataMember(Name = "unitPrice")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// unit price incl VAT, in-, output
    /// </summary>
    [DataMember(Name = "unitPriceWithTax")]
    public decimal UnitPriceWithTax { get; set; }

    /// <summary>
    /// applicable tax rate in percent, in-, output
    /// </summary>
    [DataMember(Name="applicableTaxRate")]
    public decimal ApplicableTaxRate { get; set; }


    /// <summary>
    /// total amount of the order position excl. VAT, output
    /// </summary>
    /// <value>total amount of the order position excl. VAT, output</value>
    [DataMember(Name = "lineTotalAmount")]
    public decimal LineTotalAmount { get; set; }

    /// <summary>
    /// total amount of the order position excl. VAT, output
    /// </summary>
    /// <value>total amount of the order position excl. VAT, incl charges and discounts, output</value>
    [DataMember(Name = "lineCumulativeTotalAmount")]
    public decimal LineCumulativeTotalAmount { get; set; }

    /// <summary>
    /// total base for taxes, output
    /// </summary>
    [DataMember(Name = "taxBaseTotalAmount")]
    public decimal TaxBaseTotalAmount { get; set; }

    /// <summary>
    /// total taxes amount, output
    /// </summary>
    [DataMember(Name = "taxTotalAmount")]
    public decimal TaxTotalAmount { get; set; }

    /// <summary>
    /// grand total incl VAT if applicable, output
    /// </summary>
    [DataMember(Name = "grandTotalAmount")]
    public decimal GrandTotalAmount { get; set; }
}