using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Event;

public class EventListResponseItem
{
    [JsonProperty("EventUID")]
    public int EventUid { get; set; }

    [JsonProperty("EventID")]
    public int EventId { get; set; }

    public string Operation { get; set; }
}