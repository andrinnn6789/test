using Newtonsoft.Json;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Event;

public class EventListItem
{
    public EventListItem()
    {
        Event = new Event();
    }

    public string Mode { get; set; }

    [JsonProperty("EventUID")]
    public int EventUid { get; set; }

    public Event Event { get; set; }
}