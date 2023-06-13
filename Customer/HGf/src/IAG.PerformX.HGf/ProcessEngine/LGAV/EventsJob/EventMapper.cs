using IAG.Infrastructure.ObjectMapper;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Event;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Event;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.EventsJob;

public class EventMapper : ObjectMapper<AtlasEvent, EventListItem>
{
    protected override EventListItem MapToDestination(AtlasEvent source, EventListItem destination)
    {
        destination.Mode = "upsert";
        destination.EventUid = source.EventUid;

        destination.Event.Number = source.Number;
        destination.Event.Description = source.Description;

        destination.Event.EventDates.EventBegin = source.EventBegin?.ToString("s");
        destination.Event.EventDates.EventEnd = source.EventEnd?.ToString("s");

        return destination;
    }
}