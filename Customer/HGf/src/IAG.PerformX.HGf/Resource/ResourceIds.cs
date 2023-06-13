using IAG.PerformX.HGf.ProcessEngine.LGAV.AttendancesJob;
using IAG.PerformX.HGf.ProcessEngine.LGAV.EventsJob;
using IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVWorkflowJob;
using IAG.PerformX.HGf.ProcessEngine.LGAV.RegistrationsJob;

namespace IAG.PerformX.HGf.Resource;

internal static class ResourceIds
{
    private const string ResourcePrefix = "LGAV.";

    // Events
    internal const string EventsGetFromAtlasErrorFormatMessage = ResourcePrefix + "Events.GetFromAtlasErrorFormatMessage";
    internal const string EventsMapToLgavErrorFormatMessage = ResourcePrefix + "Events.MapToLgavErrorFormatMessage";
    internal const string EventsWriteToLgavErrorFormatMessage = ResourcePrefix + "Events.WriteToLgavErrorFormatMessage";
    internal const string EventsWriteResultErrorFormatMessage = ResourcePrefix + "Events.WriteResultErrorFormatMessage";

    // Registrations
    internal const string RegistrationsGetFromAtlasErrorFormatMessage = ResourcePrefix + "Registrations.GetFromAtlasErrorFormatMessage";
    internal const string RegistrationsMapToLgavErrorFormatMessage = ResourcePrefix + "Registrations.MapToLgavErrorFormatMessage";
    internal const string RegistrationsLoadFileErrorFormatMessage = ResourcePrefix + "Registrations.LoadFileErrorFormatMessage '{0}'";
    internal const string RegistrationsWriteToLgavErrorFormatMessage = ResourcePrefix + "Registrations.WriteToLgavErrorFormatMessage";
    internal const string RegistrationsWriteResultErrorFormatMessage = ResourcePrefix + "Registrations.WriteResultErrorFormatMessage";

    // Attendances
    internal const string AttendancesGetFromAtlasErrorFormatMessage = ResourcePrefix + "Attendances.GetFromAtlasErrorFormatMessage";
    internal const string AttendancesMapToLgavErrorFormatMessage = ResourcePrefix + "Attendances.MapToLgavErrorFormatMessage '{0}'";
    internal const string AttendancesLoadFileErrorFormatMessage = ResourcePrefix + "Attendances.LoadFileErrorFormatMessage '{0}'";
    internal const string AttendancesWriteToLgavErrorFormatMessage = ResourcePrefix + "Attendances.WriteToLgavErrorFormatMessage";
    internal const string AttendancesWriteResultErrorFormatMessage = ResourcePrefix + "Attendances.WriteResultErrorFormatMessage '{0}'";

    // Workflow
    internal const string WorkflowEventsJobErrorMessage = ResourcePrefix + "Workflow.EventsJobErrorMessage";
    internal const string WorkflowRegistrationsJobErrorMessage = ResourcePrefix + "Workflow.RegistrationsJobErrorMessage";
    internal const string WorkflowAttendancesJobErrorMessage = ResourcePrefix + "Workflow.AttendancesJobErrorMessage";

    // Jobs
    public const string ResourcePrefixJob = ResourcePrefix + "Job.";
    internal const string AttendancesJobName = AttendancesJob.JobName;
    internal const string RegistrationsJobName = RegistrationsJob.JobName;
    internal const string EventsJobName = EventsJob.JobName;
    internal const string LgavWorkflowJobName = LgavWorkflowJob.JobName;
}