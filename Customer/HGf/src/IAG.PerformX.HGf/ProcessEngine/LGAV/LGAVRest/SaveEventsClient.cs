using IAG.Infrastructure.Rest;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest.Dto.Event;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest;

public class SaveEventsClient : BaseClient<EventListMainObject, EventListResponseMainObject>
{
    public SaveEventsClient(IHttpConfig config, IRequestResponseLogger logger) : base(config, "saveevents", logger)
    {
    }
}