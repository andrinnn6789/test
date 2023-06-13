using System;

using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Event;

public class AtlasEvent
{
    [JsonProperty("EventUID")]
    public int EventUid { get; set; }

    public string Description { get; set; }

    public DateTime? EventBegin { get; set; }

    public DateTime? EventEnd { get; set; }

    public string Number { get; set; }
}