using IAG.Infrastructure.Rest;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Event;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest;

public class EventRequestClient : BaseRequestClient<AtlasEvent>
{
    public EventRequestClient(IHttpConfig config, IRequestResponseLogger logger) : base(config, "view/QEventList", logger)
    {
    }
}