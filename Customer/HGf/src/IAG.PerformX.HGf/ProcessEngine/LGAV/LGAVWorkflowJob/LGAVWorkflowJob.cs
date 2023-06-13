using IAG.Infrastructure.Exception;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.PerformX.HGf.ProcessEngine.LGAV.AttendancesJob;
using IAG.PerformX.HGf.ProcessEngine.LGAV.EventsJob;
using IAG.PerformX.HGf.ProcessEngine.LGAV.RegistrationsJob;
using IAG.PerformX.HGf.Resource;

using Microsoft.Extensions.Logging;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVWorkflowJob;

[JobInfo("F6F1E54D-5441-4F58-91A4-6CB053C940A2", JobName)]
public class LgavWorkflowJob : JobBase<LgavWorkflowConfig, JobParameter, LgavWorkflowResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "Workflow";

    private readonly ILoggerFactory _loggerFactory;

    public LgavWorkflowJob(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    protected override void ExecuteJob()
    {
        var eventJob = new EventsJob.EventsJob(_loggerFactory.CreateLogger<EventsJob.EventsJob>())
        {
            Config = new CommonConfigMapper<EventsConfig>().NewDestination(Config)
        };
        Result.EventsResult = eventJob.Result;
        if (!eventJob.Execute(Infrastructure))
        {
            throw new LocalizableException(ResourceIds.WorkflowEventsJobErrorMessage);
        }

        HeartbeatAndCheckCancellation();

        var registrationsJob = new RegistrationsJob.RegistrationsJob(_loggerFactory.CreateLogger<RegistrationsJob.RegistrationsJob>())
        {
            Config = new CommonConfigMapper<RegistrationsConfig>().NewDestination(Config)
        };
        Result.RegistrationsResult = registrationsJob.Result;
        if (!registrationsJob.Execute(Infrastructure))
        {
            throw new LocalizableException(ResourceIds.WorkflowRegistrationsJobErrorMessage);
        }

        HeartbeatAndCheckCancellation();

        var attendancesJob = new AttendancesJob.AttendancesJob(_loggerFactory.CreateLogger<AttendancesJob.AttendancesJob>())
        {
            Config = new CommonConfigMapper<AttendancesConfig>().NewDestination(Config)
        };
        Result.AttendancesResult = attendancesJob.Result;
        if (!attendancesJob.Execute(Infrastructure))
        {
            throw new LocalizableException(ResourceIds.WorkflowAttendancesJobErrorMessage);
        }

        base.ExecuteJob();
    }
}