namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Event;

public class Event
{
    public Event()
    {
        EventDates = new EventDates();
    }

    public string Number { get; set; }
        
    public string Description { get; set; }

    public EventDates EventDates { get; set; }
}