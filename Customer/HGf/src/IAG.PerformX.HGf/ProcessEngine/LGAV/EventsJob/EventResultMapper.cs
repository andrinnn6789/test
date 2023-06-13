using System;

using IAG.Infrastructure.ObjectMapper;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Event;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Event;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.EventsJob;

public class EventResultMapper : ObjectMapper<EventListResponseItem, AtlasEventResult>
{
    protected override AtlasEventResult MapToDestination(EventListResponseItem source, AtlasEventResult destination)
    {
        destination.HgfLgavBestaetigung = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        destination.HgfLgavId = source.EventId;
        destination.HgfLgavUebermittelt = true;

        return destination;
    }
}