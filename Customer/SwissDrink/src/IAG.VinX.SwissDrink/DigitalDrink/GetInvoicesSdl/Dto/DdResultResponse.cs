using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.Dto;

[ExcludeFromCodeCoverage]
public class DdResultResponse<T>
{
    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("previous")]
    public string Previous { get; set; }

    [JsonProperty("next")]
    public string Next { get; set; }

    [JsonProperty("results")]
    public IEnumerable<T> Results { get; set; }
}