using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.Dto;

public class DdArticlePositionSdl
{
    [JsonProperty("sequence")]
    public int Sequence { get; set; }

    [JsonProperty("number")]
    public string Number { get; set; }

    [ExcludeFromCodeCoverage]  // currently not used
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("quantity")]
    public decimal Quantity { get; set; }

    [JsonProperty("quantity_each")]
    public decimal UnitQuantity { get; set; }

    [JsonProperty("value")]
    public decimal UnitPrice { get; set; }

    [JsonProperty("total_value")]
    public decimal TotalPrice => UnitQuantity * UnitPrice;

    [ExcludeFromCodeCoverage]  // currently not used
    [JsonProperty("type")]
    public string Type { get; set; }
}