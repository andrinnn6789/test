using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.VinX.IAG.Resource;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class ResourceProvider : Infrastructure.Globalisation.ResourceProvider.ResourceProvider
{
    public ResourceProvider()
    {
        // Distribution
        AddTemplate(ResourceIds.AuthenticationError, "en", "Authentication failed");
        AddTemplate(ResourceIds.ScanArtifactsError, "en", "Failed to scan '{0}' for artifacts");
        AddTemplate(ResourceIds.ScanConfigurationsError, "en", "Failed to scan '{0}' for configurations");
        AddTemplate(ResourceIds.GetProductsError, "en", "Failed to get products");
        AddTemplate(ResourceIds.GetReleasesError, "en", "Failed to get releases");
        AddTemplate(ResourceIds.RegisterProductError, "en", "Failed to register product");
        AddTemplate(ResourceIds.RegisterReleaseError, "en", "Failed to register release");
        AddTemplate(ResourceIds.ReleaseAlreadyApprovedError, "en", "Release {0} exists and is already approved");
        AddTemplate(ResourceIds.AddFilesToReleaseError, "en", "Failed to add files to release: {0} and {1}");
        AddTemplate(ResourceIds.SetFileContentError, "en", "Failed to set content of file '{0}'");
        AddTemplate(ResourceIds.ApproveReleaseError, "en", "Failed to approve release: {0} and {1}");
        AddTemplate(ResourceIds.RegisterCustomerError, "en", "Failed to register customer: {0} and cat-id '{1}'");
        AddTemplate(ResourceIds.AddProductsForCustomerError, "en", "Failed to assign products to customer {0}");
        AddTemplate(ResourceIds.GetCustomersError, "en", "Failed to get customers");
        AddTemplate(ResourceIds.PublishReleaseError, "en", "Failed to publish release in '{0}' for product '{1}'");
        AddTemplate(ResourceIds.CleanupOldReleaseWarning, "en", "Failed to cleanup old release");
        AddTemplate(ResourceIds.StartAddingFilesToReleaseInfo, "en", "Start adding {0} files to release {1}");
        AddTemplate(ResourceIds.SetFileContentInfo, "en", "Set content of file '{0}' ({1} bytes)");
        AddTemplate(ResourceIds.AddCustomerInfo, "en", "Add customer '{0}' for products {1} failed");
        AddTemplate(ResourceIds.ReleaseAlreadyPublishedInfo, "en", "Release in '{0}' for product '{1}' already published");
        AddTemplate(ResourceIds.ReleaseSuccessfullyPublishedInfo, "en", "Release in '{0}' for product '{1}' successfully published");
        AddTemplate(ResourceIds.NoProductForConfigurationWarning, "en", "Ignore configuration '{0}' for unknown product '{1}'");
        AddTemplate(ResourceIds.SyncLinkListError, "en", "Failed to sync link lists");

        // JiraToVinX
        AddTemplate(ResourceIds.LoadPendenzSettingsErrorFormatMessage, "de", "Issue-Sync-Job abgebrochen: Pendenz-Settings konnten nicht geladen werden ({0}).");
        AddTemplate(ResourceIds.InitMapperErrorFormatMessage, "de", "Issue-Sync-Job abgebrochen: Jira zu VinX Mapper konnte nicht initialisiert werden ({0}).");
        AddTemplate(ResourceIds.LoadJiraIssuesErrorFormatMessage, "de", "Issue-Sync-Job abgebrochen: Jira-Tickets konnten nicht geladen werden ({0}).");
        AddTemplate(ResourceIds.LoadPendenzenErrorFormatMessage, "de", "Issue-Sync von Jira-Ticket '{0}' übersprungen. Das Laden von VinX-Pendenzen ist fehlgeschlagen ({1}).");
        AddTemplate(ResourceIds.CreatePendenzErrorFormatMessage, "de", "Issue-Sync von Jira-Ticket '{0}' übersprungen. Das Erstellen der VinX-Pendenz ist fehlgeschlagen ({1}).");
        AddTemplate(ResourceIds.UpdatePendenzErrorFormatMessage, "de", "Issue-Sync von Jira-Ticket '{0}' übersprungen. Das Aktualisieren der VinX-Pendenz ist fehlgeschlagen ({1}).");
        AddTemplate(ResourceIds.MultiplePendenzenErrorFormatMessage, "de", "Issue-Sync von Jira-Ticket '{0}' übersprungen. Es wurden mehrere VinX-Pendenzen gefunden.");
        AddTemplate(ResourceIds.NoPendenzForLinkFoundErrorFormatMessage, "de", "Issue-Sync von Jira-Ticket '{0}' unvollständig. Es wurde keine VinX-Pendenz für das Verknüpfungsticket '{1}' gefunden");
        AddTemplate(ResourceIds.MultiplePendenzenForLinkErrorFormatMessage, "de", "Issue-Sync von Jira-Ticket '{0}' unvollständig. Es wurden mehrere VinX-Pendenzen für das Verknüpfungsticket '{1}' gefunden");
        AddTemplate(ResourceIds.MultipleBillingLinksErrorFormatMessage, "de", "Issue-Sync von Jira-Ticket '{0}' unvollständig. Es wurden mehrere Verrechnungsverknüpfungen gefunden.");
        AddTemplate(ResourceIds.SetLastSyncErrorFormatMessage, "de", "Issue-Sync-Job: Last-Sync konnte nicht gespeichert werden: Jira-Ticket ({0})");
        AddTemplate(ResourceIds.FailedWorklogProcessingErrorFormatMessage, "de", "Component-Sync für VinX-Arbeitszeit ({0}) von User ({1}) konnte nicht erstellt oder aktualisiert werden");
        AddTemplate(ResourceIds.LoadJiraWorklogsErrorFormatMessage, "de", "Component-Sync-Job abgebrochen: Worklogs konnten nicht geladen werden");

        // jobs
        AddTemplate(ResourceIds.VinXJiraIssueSyncJobName, "de", "JIRA Sync: Pendenzen");
        AddTemplate(ResourceIds.VinXJiraWorklogSyncJobName, "de", "JIRA Sync: Worklogs");
        AddTemplate(ResourceIds.LicenceSyncerJobName, "de", "ControlCenter: Abgleich Mobile-Lizenzen");
        AddTemplate(ResourceIds.PublishReleasesJobName, "de", "ControlCenterPublizieren von neuen Releases");
        AddTemplate(ResourceIds.SyncCustomersJobName, "de", "ControlCenter: Publizieren von Kunden");
    }
}