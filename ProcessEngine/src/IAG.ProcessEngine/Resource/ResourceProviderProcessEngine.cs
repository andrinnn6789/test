using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Globalisation.ResourceProvider;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.ProcessEngine.Enum;

using JetBrains.Annotations;

namespace IAG.ProcessEngine.Resource;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class ResourceProviderProcessEngine : ResourceProvider
{
    public ResourceProviderProcessEngine()
    {
        AddTemplate(ResourceIds.JobExecuterJobCanceled, "en", "Job canceled");
        AddTemplate(ResourceIds.JobExecuterSingletonJobAlreadyRunning, "en", "Cannot start job {0}: another instance is already running");
        AddTemplate(ResourceIds.JobExecuterUnhandledException, "en", "Job failed with unhandled exception: {0}");
        AddTemplate(ResourceIds.JobDataStoreNotForConcurrentJobs, "en", "Job data store cannot be used by jobs with configuration AllowConcurrentInstances=true");

        AddTemplate(ResourceIds.ConditionParseExceptionUnknownOperator, "en", "Unknown operator {0}");
        AddTemplate(ResourceIds.ConditionParseExceptionInvalidOperatorUsage, "en", "Invalid operator usage: only '!=' allowed");
        AddTemplate(ResourceIds.ConditionParseExceptionInvalidExpression, "en", "Invalid constant expression '{0}'");
        AddTemplate(ResourceIds.ConditionParseExceptionEmptyExpression, "en", "Invalid empty expression");
        AddTemplate(ResourceIds.ConditionParseExceptionMissingClosingQuote, "en", "Missing closing quote");
        AddTemplate(ResourceIds.ConditionParseExceptionParenthesisMismatch, "en", "Parenthesis mismatch (rate={0})");
        AddTemplate(ResourceIds.ConditionParseExceptionUnknownCondition, "en", "Unknown constant condition '{0}'");

        AddEnumTemplates(typeof(JobResultEnum));
        AddEnumTemplates(typeof(JobExecutionStateEnum));
 
        AddTemplate(ResourceIds.MonitoringSendInfluxPointFailed, "en", "Failed to send data point to influx {0}");

        // jobs
        AddTemplate(ResourceIds.MonitoringJobName, "en", "Monitoring the system");
        AddTemplate(ResourceIds.MonitoringJobName, "de", "Überwachung des Systems");
        AddTemplate(ResourceIds.MonitoringJobName, "fr", "Surveillance du système");
        AddTemplate(ResourceIds.MonitoringJobName, "it", "Monitoraggio del sistema");

        AddTemplate(ResourceIds.CleanupJobName, "en", "Cleanup log PE-Job");
        AddTemplate(ResourceIds.CleanupJobName, "de", "Bereinigen alter Job-Logeinträge");
        AddTemplate(ResourceIds.CleanupJobName, "fr", "Nettoyer les anciennes entrées du journal des travaux");
        AddTemplate(ResourceIds.CleanupJobName, "it", "Ripulisci le vecchie voci del registro lavori");

        // JobExecutionStateEnum & JobResultEnum
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Success}", "en", "Success");   // &JobResultEnum
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Success}", "de", "Erfolgreich");   // &JobResultEnum
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Success}", "fr", "Succès");   // &JobResultEnum
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Success}", "it", "Successo");   // &JobResultEnum
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Failed}", "en", "Failed"); // &JobResultEnum
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Failed}", "de", "Fehlgeschlagen"); // &JobResultEnum
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Failed}", "fr", "Échoué"); // &JobResultEnum
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Failed}", "it", "Fallito"); // &JobResultEnum
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Aborted}", "en", "Aborted");   // &JobResultEnum
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Aborted}", "de", "Abgebrochen");   // &JobResultEnum
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Aborted}", "fr", "Annulé");   // &JobResultEnum
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Aborted}", "it", "Annulato");   // &JobResultEnum
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.New}", "en", "New");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.New}", "de", "Neu");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.New}", "fr", "Nouveau");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.New}", "it", "Nuovo");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Running}", "en", "Running...");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Running}", "de", "Läuft...");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Running}", "fr", "En cours...");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Running}", "it", "En corso...");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Scheduled}", "en", "Scheduled");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Scheduled}", "de", "Terminiert");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Scheduled}", "fr", "Programmé");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Scheduled}", "it", "Programmato");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Warning}", "en", "Warning");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Warning}", "de", "Warnung");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Warning}", "fr", "Attention");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobExecutionStateEnum.Warning}", "it", "Attenzione");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobResultEnum.NoResult}", "en", "No result");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobResultEnum.NoResult}", "de", "Kein Resultat");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobResultEnum.NoResult}", "fr", "Pas de résultat");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobResultEnum.NoResult}", "it", "Nessun risultato");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobResultEnum.PartialSuccess}", "en", "Terminated with errors");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobResultEnum.PartialSuccess}", "de", "Mit Fehler beendet");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobResultEnum.PartialSuccess}", "fr", "Terminé avec erreurs");
        AddTemplate($"{ResourceIds.ResourcePrefix}{JobResultEnum.PartialSuccess}", "it", "Terminato con errori");

        // Job-Result
        AddTemplate($"{ResourceIds.ResourcePrefix}Result", "en", "Job state");
        AddTemplate($"{ResourceIds.ResourcePrefix}Result", "de", "Job-Status");
        AddTemplate($"{ResourceIds.ResourcePrefix}Result", "fr", "État processus");
        AddTemplate($"{ResourceIds.ResourcePrefix}Result", "it", "Stato processo");
        AddTemplate($"{ResourceIds.ResourcePrefix}AddedCount", "en", "Nb added");
        AddTemplate($"{ResourceIds.ResourcePrefix}AddedCount", "de", "Anzahl hinzugefügt");
        AddTemplate($"{ResourceIds.ResourcePrefix}AddedCount", "fr", "Nb ajoutés");
        AddTemplate($"{ResourceIds.ResourcePrefix}AddedCount", "it", "Nb aggiunti");
        AddTemplate($"{ResourceIds.ResourcePrefix}UpdatedCount", "en", "Nb updated");
        AddTemplate($"{ResourceIds.ResourcePrefix}UpdatedCount", "de", "Anzahl aktualisiert");
        AddTemplate($"{ResourceIds.ResourcePrefix}UpdatedCount", "fr", "Nb mis à jour");
        AddTemplate($"{ResourceIds.ResourcePrefix}UpdatedCount", "it", "Nb aggiornati");
        AddTemplate($"{ResourceIds.ResourcePrefix}DeletedCount", "en", "Nb deleted");
        AddTemplate($"{ResourceIds.ResourcePrefix}DeletedCount", "de", "Anzahl gelöscht");
        AddTemplate($"{ResourceIds.ResourcePrefix}DeletedCount", "fr", "Nb effacés");
        AddTemplate($"{ResourceIds.ResourcePrefix}DeletedCount", "it", "Nb cancellati");
        AddTemplate($"{ResourceIds.ResourcePrefix}ErrorCount", "en", "Nb errors");
        AddTemplate($"{ResourceIds.ResourcePrefix}ErrorCount", "de", "Anzahl Fehler");
        AddTemplate($"{ResourceIds.ResourcePrefix}ErrorCount", "fr", "Nb d'erreurs");
        AddTemplate($"{ResourceIds.ResourcePrefix}ErrorCount", "it", "Nb d'errori");
        AddTemplate($"{ResourceIds.ResourcePrefix}SendCount", "en", "Nb sent");
        AddTemplate($"{ResourceIds.ResourcePrefix}SendCount", "de", "Anzahl gesendet");
        AddTemplate($"{ResourceIds.ResourcePrefix}SendCount", "fr", "Nb expédiés");
        AddTemplate($"{ResourceIds.ResourcePrefix}SendCount", "it", "Nb inviati");
        AddTemplate($"{ResourceIds.ResourcePrefix}SendErrors", "en", "Sending errors");
        AddTemplate($"{ResourceIds.ResourcePrefix}SendErrors", "de", "Sendefehler");
        AddTemplate($"{ResourceIds.ResourcePrefix}SendErrors", "fr", "Erreurs d'envoi");
        AddTemplate($"{ResourceIds.ResourcePrefix}SendErrors", "it", "Errori d'invio");
        AddTemplate($"{ResourceIds.ResourcePrefix}DocUpdateCount", "en", "Documents updated");
        AddTemplate($"{ResourceIds.ResourcePrefix}DocUpdateCount", "de", "Anzahl Dokumente aktualisiert");
        AddTemplate($"{ResourceIds.ResourcePrefix}DocUpdateCount", "fr", "Nb de documents mis à jour");
        AddTemplate($"{ResourceIds.ResourcePrefix}DocUpdateCount", "it", "Nb documenti aggiornati");
        AddTemplate($"{ResourceIds.ResourcePrefix}DocUpdateErrors", "en", "Nb erros document update");
        AddTemplate($"{ResourceIds.ResourcePrefix}DocUpdateErrors", "de", "Anzahl Fehler Dokumentaktualisierung");
        AddTemplate($"{ResourceIds.ResourcePrefix}DocUpdateErrors", "fr", "Nb d'erreurs mis à jour de documents");
        AddTemplate($"{ResourceIds.ResourcePrefix}DocUpdateErrors", "it", "Nb d'errori aggiornamento di documenti");
        AddTemplate($"{ResourceIds.ResourcePrefix}GenericErrors", "en", "Gereric errors");
        AddTemplate($"{ResourceIds.ResourcePrefix}GenericErrors", "de", "Allgemeine Fehler");
        AddTemplate($"{ResourceIds.ResourcePrefix}GenericErrors", "fr", "Erreurs génériques");
        AddTemplate($"{ResourceIds.ResourcePrefix}GenericErrors", "it", "Errori generali");
    }
}