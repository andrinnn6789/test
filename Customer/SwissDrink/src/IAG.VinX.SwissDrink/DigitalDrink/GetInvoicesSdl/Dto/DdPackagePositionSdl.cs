using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.Dto;

public class DdPackagePositionSdl
{
    [JsonProperty("number")] 
    public string Number { get; set; }

    [ExcludeFromCodeCoverage]  // currently not used
    [JsonProperty("name")] 
    public string Name { get; set; }

    [ExcludeFromCodeCoverage]  // currently not used
    [JsonProperty("quantity_delivered")]
    public decimal QuantityDelivered { get; set; }

    [JsonProperty("quantity_returned")]
    public decimal QuantityReturned { get; set; }

    [ExcludeFromCodeCoverage]  // currently not used
    [JsonProperty("value")]
    public decimal UnitPrice { get; set; }
}