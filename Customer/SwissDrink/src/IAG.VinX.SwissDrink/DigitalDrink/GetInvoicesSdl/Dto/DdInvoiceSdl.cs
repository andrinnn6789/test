using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.Dto;

public class DdInvoiceSdl
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("wholesaler_gln")]
    public string WholeSalerGln { get; set; }

    [JsonProperty("customer_number")]
    public string CustomerNumber { get; set; }

    [ExcludeFromCodeCoverage]  // currently not used
    [JsonProperty("invoice_number")]
    public string InvoiceNumber { get; set; }

    [JsonProperty("invoice_date")]
    public DateTime InvoiceDate { get; set; }

    [JsonProperty("delivery_number")]
    public string DeliveryNumber { get; set; }

    [JsonProperty("delivery_date")]
    public DateTime DeliveryDate { get; set; }

    [ExcludeFromCodeCoverage]  // currently not used
    [JsonProperty("order_number")]
    public string OrderNumber { get; set; }

    [JsonProperty("order_date")]
    public DateTime? OrderDate { get; set; }

    [JsonProperty("total_value")]
    public decimal TotalValue { get; set; }

    [JsonProperty("payment_reference_number")]
    public string PaymentReferenceNumber { get; set; }

    [JsonProperty("items")]
    public IEnumerable<DdArticlePositionSdl> ArticlePositions { get; set; } = new List<DdArticlePositionSdl>();

    [JsonProperty("extras")]
    public IEnumerable<DdPackagePositionSdl> PackagePositions { get; set; } = new List<DdPackagePositionSdl>();

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }
}