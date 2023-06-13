using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.EventsJob;

public class EventsResult : JobResult
{
    public int AtlasEventsCount { get; set; }

    public int LgavResultEventsCount { get; set; }

    public int SuccessfulWriteResultCount { get; set; }
}