using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AttendancesJob;
using IAG.PerformX.HGf.ProcessEngine.LGAV.EventsJob;
using IAG.PerformX.HGf.ProcessEngine.LGAV.RegistrationsJob;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVWorkflowJob;

public class LgavWorkflowResult : JobResult
{
    public EventsResult EventsResult { get; set; }

    public RegistrationsResult RegistrationsResult { get; set; }

    public AttendancesResult AttendancesResult { get; set; }
}