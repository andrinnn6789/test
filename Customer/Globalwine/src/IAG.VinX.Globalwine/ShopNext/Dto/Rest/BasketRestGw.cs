using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.Globalwine.ShopNext.Dto.Enum;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Rest;

/// <summary>
/// header information of the basket
/// </summary>
[DisplayName("Basket")]
[DataContract]
public class BasketRestGw
{
    /// <summary>
    /// id of the ordering contact input, optional.
    /// </summary>
    [DataMember(Name = "orderingContactId")]
    public int? OrderingContactId { get; set; }

    /// <summary>
    /// id of the carrier, reference to /carrier, input, required
    /// </summary>
    [DataMember(Name = "carrierId")]
    public int CarrierId { get; set; }

    /// <summary>
    /// description of the CRIF-Check
    /// </summary>
    [DataMember(Name = "crifDescription")]
    public string CrifDescription { get; set; }

    /// <summary>
    /// date of the CRIF-Check
    /// </summary>
    [DataMember(Name = "crifCheckDate")]
    public DateTime? CrifCheckDate { get; set; }

    /// <summary>
    /// gets or sets the action to perform (calculate, order,..)
    /// </summary>
    [DataMember(Name = "action")]
    public BasketActionTypeGw Action { get; set; }

    /// <summary>
    /// id of the stored basket in VinX, if action = order
    /// </summary>
    [DataMember(Name = "id")]
    public int? Id { get; set; }

    /// <summary>
    /// arbitrary id attributed by the shop, input, optional
    /// </summary>
    [DataMember(Name = "shopId")]
    public string ShopId { get; set; }

    /// <summary>
    /// id of the division to calculate the prices for, input, optional
    /// </summary>
    [DataMember(Name = "divisionId")]
    public int? DivisionId { get; set; }

    /// <summary>
    /// free text to the order / Best_Hinweis
    /// </summary>
    [DataMember(Name = "orderText")]
    public string OrderText { get; set; }

    /// <summary>
    /// Prices are to show with VAT, output
    /// </summary>
    [DataMember(Name = "vatCalculation")]
    public VatCalculationType VatCalculation { get; set; }

    /// <summary>
    /// id of the condition address. Required to determine the price conditions if no address-Ids are transmitted
    /// </summary>
    [DataMember(Name = "conditionAddressId")]
    public int? ConditionAddressId { get; set; }

    /// <summary>
    /// id of the ordering address. Required if no online address is transmitted
    /// </summary>
    [DataMember(Name = "orderingAddressId")]
    public int? OrderingAddressId { get; set; }

    /// <summary>
    /// id of the billing address, input, optional
    /// </summary>
    [DataMember(Name = "billingAddressId")]
    public int? BillingAddressId { get; set; }

    /// <summary>
    /// id of the delivery address, input, optional
    /// </summary>
    [DataMember(Name = "deliveryAddressId")]
    public int? DeliveryAddressId { get; set; }

    /// <summary>
    /// Gets or Sets OrderingOnlineAddress
    /// </summary>
    [DataMember(Name = "orderingOnlineAddress")]
    public OnlineAddressRestGw OrderingOnlineAddress { get; set; }

    /// <summary>
    /// Gets or Sets BillingOnlineAddress
    /// </summary>
    [DataMember(Name = "billingOnlineAddress")]
    public OnlineAddressRestGw BillingOnlineAddress { get; set; }

    /// <summary>
    /// Gets or Sets DeliveryOnlineAddress
    /// </summary>
    [DataMember(Name = "deliveryOnlineAddress")]
    public OnlineAddressRestGw DeliveryOnlineAddress { get; set; }

    /// <summary>
    /// id of the payment condition, reference to /paymentCondition, input, required
    /// </summary>
    [DataMember(Name = "paymentConditionId")]
    public int? PaymentConditionId { get; set; }

    /// <summary>
    /// id of the delivery condition, reference to /deliveryCondition, input, required
    /// </summary>
    [DataMember(Name = "deliveryConditionId")]
    public int DeliveryConditionId { get; set; }

    /// <summary>
    /// in the shop payed amount, input
    /// </summary>
    [DataMember(Name = "amountPayed")]
    public decimal AmountPayed { get; set; }

    /// <summary>
    /// Date for the price calculation, default = today
    /// </summary>
    [DataMember(Name = "validDate")]
    public DateTime ValidDate { get; set; } = DateTime.Now;

    /// <summary>
    /// desired delivery location input, optional.
    /// </summary>
    [DataMember(Name = "deliveryLocation")]
    public string DeliveryLocation { get; set; }

    /// <summary>
    /// desired delivery location remark input, optional.
    /// </summary>
    [DataMember(Name = "deliveryLocationRemark")]
    public string DeliveryLocationRemark { get; set; }

    /// <summary>
    /// desired delivery time input, optional.
    /// </summary>
    [DataMember(Name = "deliveryTimeRequested")]
    public string DeliveryTime { get; set; }

    /// <summary>
    /// requested delivery date by the customer, default = today / Best_Lieferdatum
    /// </summary>
    [DataMember(Name = "deliveryDateRequested")]
    public DateTime DeliveryDateRequested { get; set; } = DateTime.Now;

    /// <summary>
    /// saferpay id, input
    /// </summary>
    [DataMember(Name = "saferPayId")]
    public string SaferPayId { get; set; }

    /// <summary>
    /// saferpay token id, input
    /// </summary>
    [DataMember(Name = "saferPayToken")]
    public string SaferPayToken { get; set; }

    /// <summary>
    /// payment terms, output
    /// </summary>
    [DataMember(Name = "paymentTerms")]
    public string PaymentTerms { get; set; }

    /// <summary>
    /// id of the provider for this order, input, optional
    /// </summary>
    [DataMember(Name = "providerId")]
    public int? ProviderId { get; set; }

    /// <summary>
    /// list of the positions of the basket
    /// </summary>
    [DataMember(Name = "positions")]
    public IList<BasketPositionGw> Positions { get; set; } = new List<BasketPositionGw>();

    /// <summary>
    /// list of the charges of the basket at header level, calculated by VinX. Inputs are ignored.
    /// </summary>
    [DataMember(Name = "charges")]
    public IList<ChargePositionGw> Charges { get; set; } = new List<ChargePositionGw>();

    /// <summary>
    /// list of the discounts of the basket at header level, calculated by VinX. Inputs are ignored.
    /// </summary>
    [DataMember(Name = "discounts")]
    public IList<DiscountPositionGw> Discounts { get; set; } = new List<DiscountPositionGw>();

    /// <summary>
    /// list of the packages of the basket at header level, calculated by VinX. Inputs are ignored.
    /// </summary>
    [DataMember(Name = "packages")]
    public IList<PackagePositionGw> Packages { get; set; } = new List<PackagePositionGw>();

    /// <summary>
    /// total amount of the order positions, output
    /// </summary>
    [DataMember(Name = "lineTotalAmount")]
    public decimal LineTotalAmount { get; set; }

    /// <summary>
    /// total amount of the order position excl. VAT, output
    /// </summary>
    /// <value>total amount of the order position excl. VAT, incl charges and discounts, output</value>
    [DataMember(Name = "lineCumulativeTotalAmount")]
    public decimal LineCumulativeTotalAmount { get; set; }

    /// <summary>
    /// total amount charges, output
    /// </summary>
    [DataMember(Name = "chargeTotalAmount")]
    public decimal ChargeTotalAmount { get; set; }

    /// <summary>
    /// total amount discounts, output
    /// </summary>
    [DataMember(Name = "discountTotalAmount")]
    public decimal DiscountTotalAmount { get; set; }

    /// <summary>
    /// total amount packages, output
    /// </summary>
    [DataMember(Name = "packagesTotalAmount")]
    public decimal PackagesTotalAmount { get; set; }

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
    /// rounding amount, output
    /// </summary>
    [DataMember(Name = "roundingAmount")]
    public decimal RoundingAmount { get; set; }

    /// <summary>
    /// grand total, output
    /// </summary>
    [DataMember(Name = "grandTotalAmount")]
    public decimal GrandTotalAmount { get; set; }
}