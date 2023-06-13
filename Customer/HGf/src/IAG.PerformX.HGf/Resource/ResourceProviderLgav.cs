using IAG.Infrastructure.Globalisation.ResourceProvider;

using JetBrains.Annotations;

namespace IAG.PerformX.HGf.Resource;

[UsedImplicitly]
public class ResourceProviderLgav : ResourceProvider
{
    public ResourceProviderLgav()
    {
        // Events
        AddTemplate(ResourceIds.EventsGetFromAtlasErrorFormatMessage, "de", "Kurse konnten nicht von PerformX geladen werden");
        AddTemplate(ResourceIds.EventsMapToLgavErrorFormatMessage, "de", "Kurs konnte nicht in das Datenformat von L-GAV umgewandelt werden. Atlas-Kurs: {0}");
        AddTemplate(ResourceIds.EventsWriteToLgavErrorFormatMessage, "de", "Kurse konnten nicht nach L-GAV geschrieben werden");
        AddTemplate(ResourceIds.EventsWriteResultErrorFormatMessage, "de", "Das Rückschreiben vom Resultat ist fehlgeschlagen");

        // Registrations
        AddTemplate(ResourceIds.RegistrationsGetFromAtlasErrorFormatMessage, "de", "Anträge konnten nicht von PerformX geladen werden");
        AddTemplate(ResourceIds.RegistrationsMapToLgavErrorFormatMessage, "de", "Antrag konnte nicht in das Datenformat von L-GAV umgewandelt werden. Atlas-Antrag: {0}");
        AddTemplate(ResourceIds.RegistrationsLoadFileErrorFormatMessage, "de", "Beim Laden der Antrag-Datei '{0}' ist ein Fehler aufgetreten.");
        AddTemplate(ResourceIds.RegistrationsWriteToLgavErrorFormatMessage, "de", "Anträge konnten nicht nach L-GAV geschrieben werden");
        AddTemplate(ResourceIds.RegistrationsWriteResultErrorFormatMessage, "de", "Das Rückschreiben vom Resultat ist fehlgeschlagen");

        // Attendances
        AddTemplate(ResourceIds.AttendancesGetFromAtlasErrorFormatMessage, "de", "Präsenzmeldungen konnten nicht von PerformX geladen werden");
        AddTemplate(ResourceIds.AttendancesMapToLgavErrorFormatMessage, "de", "Präsenzmeldung konnte nicht in das Datenformat von L-GAV umgewandelt werden. Atlas-Präsenzmeldung: {0}");
        AddTemplate(ResourceIds.AttendancesLoadFileErrorFormatMessage, "de", "Beim Laden der Präsenzmeldung-Datei '{0}' ist ein Fehler aufgetreten.");
        AddTemplate(ResourceIds.AttendancesWriteToLgavErrorFormatMessage, "de", "Präsenzmeldungen konnten nicht nach L-GAV geschrieben werden.");
        AddTemplate(ResourceIds.AttendancesWriteResultErrorFormatMessage, "de", "Das Rückschreiben vom Resultat ist fehlgeschlagen. L-GAV Attendances-Resultat: {0}");

        // Workflow
        AddTemplate(ResourceIds.WorkflowEventsJobErrorMessage, "de", "Workflow abgebrochen: Job zum Abgleich der Kurse war nicht erfolgreich");
        AddTemplate(ResourceIds.WorkflowRegistrationsJobErrorMessage, "de", "Workflow abgebrochen: Job zum Abgleich der Anträge war nicht erfolgreich");
        AddTemplate(ResourceIds.WorkflowAttendancesJobErrorMessage, "de", "Workflow abgebrochen: Job zum Abgleich der Präsenzmeldungen war nicht erfolgreich");

        // jobs
        AddTemplate(ResourceIds.LgavWorkflowJobName, "de", "L-GAV: Haupt-Job Export aller Daten");
        AddTemplate(ResourceIds.AttendancesJobName, "de", "L-GAV: Hilfsjob (Export Präsenzmeldungen)");
        AddTemplate(ResourceIds.EventsJobName, "de", "L-GAV: Hilfsjob (Export Kurse)");
        AddTemplate(ResourceIds.RegistrationsJobName, "de", "L-GAV: Hilfsjob (Export Anträge)");
    }
}