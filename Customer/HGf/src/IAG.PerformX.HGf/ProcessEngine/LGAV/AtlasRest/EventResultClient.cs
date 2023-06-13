using IAG.Infrastructure.Rest;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest.Dto.Event;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest;

public class EventResultClient : BaseResultClient<AtlasEventResult>
{
    public EventResultClient(IHttpConfig config, IRequestResponseLogger logger) : base(config, "VertragDef", logger)
    {
    }
}