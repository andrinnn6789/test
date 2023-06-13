using System.Collections.Generic;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Event;

public class EventList
{
    public EventList()
    {
        Header = new Header();
        EventListItems = new List<EventListItem>();
    }

    public Header Header { get; set; }

    public List<EventListItem> EventListItems { get; set; }
}