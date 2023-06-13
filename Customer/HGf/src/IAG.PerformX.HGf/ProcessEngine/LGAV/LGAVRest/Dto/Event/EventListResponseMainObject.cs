using System.Collections.Generic;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Event;

public class EventListResponseMainObject
{
    public EventListResponseMainObject()
    {
        EventListResponseItems = new List<EventListResponseItem>();
    }

    public List<EventListResponseItem> EventListResponseItems { get; set; }
}