using IAG.VinX.IAG.ControlCenter.Distribution.ProcessEngine.PublishReleases;
using IAG.VinX.IAG.ControlCenter.Distribution.ProcessEngine.SyncCustomers;
using IAG.VinX.IAG.ControlCenter.Mobile.ProcessEngine;

namespace IAG.VinX.IAG.Resource;

internal static class ResourceIds
{
    private const string ResourcePrefix = "I-AG.";

    // Distribution
    private const string ResourcePrefixDist = ResourcePrefix + "Distribution.";
    internal const string AuthenticationError = ResourcePrefixDist + "Authentication.Error";
    internal const string ScanArtifactsError = ResourcePrefixDist + "ScanArtifacts.Error '{0}'";
    internal const string ScanConfigurationsError = ResourcePrefixDist + "ScanConfigurations.Error '{0}'";
    internal const string GetProductsError = ResourcePrefixDist + "GetProducts.Error";
    internal const string GetReleasesError = ResourcePrefixDist + "GetReleases.Error";
    internal const string RegisterProductError = ResourcePrefixDist + "RegisterProduct.Error";
    internal const string RegisterReleaseError = ResourcePrefixDist + "RegisterRelease.Error";
    internal const string ReleaseAlreadyApprovedError = ResourcePrefixDist + "ReleaseAlreadyApproved.Error";
    internal const string AddFilesToReleaseError = ResourcePrefixDist + "AddFilesToRelease.Error for '{0}' and '{1}'";
    internal const string SetFileContentError = ResourcePrefixDist + "SetFileContent.Error '{0}'";
    internal const string ApproveReleaseError = ResourcePrefixDist + "ApproveRelease.Error for '{0}' and '{1}'";
    internal const string RemoveReleaseError = ResourcePrefixDist + "RemoveRelease.Error for '{0}' and '{1}'";
    internal const string RegisterCustomerError = ResourcePrefixDist + "RegisterCustomer.Error for '{0}' and cat-id '{1}'";
    internal const string AddProductsForCustomerError = ResourcePrefixDist + "AddProductsForCustomer.Error '{0}'";
    internal const string GetCustomersError = ResourcePrefixDist + "GetCustomers.Error";
    internal const string PublishReleaseError = ResourcePrefixDist + "PublishRelease.Error";
    internal const string CleanupOldReleaseWarning = ResourcePrefixDist + "DisableRelease.Error";
    internal const string StartAddingFilesToReleaseInfo = ResourcePrefixDist + "StartAddingFilesToRelease.Info";
    internal const string SetFileContentInfo = ResourcePrefixDist + "SetFileContent.Info";
    internal const string AddCustomerInfo = ResourcePrefixDist + "AddCustomer.Info";
    internal const string ReleaseAlreadyPublishedInfo = ResourcePrefixDist + "ReleaseAlreadyPublished.Info";
    internal const string ReleaseSuccessfullyPublishedInfo = ResourcePrefixDist + "ReleaseSuccessfullyPublished.Info";
    internal const string NoProductForConfigurationWarning = ResourcePrefixDist + "NoProductForConfiguration.Warning";
    internal const string SyncLinkListError = ResourcePrefixDist + "SyncLinkList.Error";

    // JiraToVinX
    private const string ResourcePrefixJtV = ResourcePrefix + "JiraVinXSync.";
    internal const string LoadPendenzSettingsErrorFormatMessage = ResourcePrefixJtV + "LoadPendenzSettingsErrorFormatMessage";
    internal const string InitMapperErrorFormatMessage = ResourcePrefixJtV + "InitMapperErrorFormatMessage";
    internal const string LoadJiraIssuesErrorFormatMessage = ResourcePrefixJtV + "LoadJiraIssuesErrorFormatMessage";
    internal const string LoadPendenzenErrorFormatMessage = ResourcePrefixJtV + "LoadPendenzenErrorFormatMessage";
    internal const string CreatePendenzErrorFormatMessage = ResourcePrefixJtV + "CreatePendenzErrorFormatMessage";
    internal const string UpdatePendenzErrorFormatMessage = ResourcePrefixJtV + "UpdatePendenzErrorFormatMessage";
    internal const string MultiplePendenzenErrorFormatMessage = ResourcePrefixJtV + "MultiplePendenzenErrorFormatMessage";
    internal const string NoPendenzForLinkFoundErrorFormatMessage = ResourcePrefixJtV + "NoPendenzForLinkFoundErrorFormatMessage";
    internal const string MultiplePendenzenForLinkErrorFormatMessage = ResourcePrefixJtV + "MultiplePendenzenForLinkErrorFormatMessage";
    internal const string MultipleBillingLinksErrorFormatMessage = ResourcePrefixJtV + "MultipleBillingLinksErrorFormatMessage";
    internal const string SetLastSyncErrorFormatMessage = ResourcePrefixJtV + "SetLastSyncErrorFormatMessage";
        
    internal const string FailedWorklogProcessingErrorFormatMessage = ResourcePrefixJtV + "FailedWorklogProcessingErrorFormatMessage";
    internal const string LoadJiraWorklogsErrorFormatMessage = ResourcePrefixJtV + "LoadJiraWorklogsErrorFormatMessage";

    internal const string LoadVersionsToJiraErrorFormatMessage = ResourcePrefixJtV + "LoadVersionsToJiraErrorFormatMessage";
    internal const string LoadComponentsToJiraErrorFormatMessage = ResourcePrefixJtV + "LoadComponentsToJiraErrorFormatMessage";


    internal const string VinXJiraIssueSyncJobName = ResourcePrefixJob + "VinXJiraIssueSync";
    internal const string VinXJiraWorklogSyncJobName = ResourcePrefixJob + "VinXJiraWorklogSync";
    internal const string VinXJiraComponentSyncJobName = ResourcePrefixJob + "VinXJiraComponentSync";
    internal const string VinXJiraVersionSyncJobName = ResourcePrefixJob + "VinXJiraVersionSync";

    // jobs
    public const string ResourcePrefixJob = ResourcePrefix + "Job.";
    internal const string PublishReleasesJobName = PublishReleasesJob.JobName;
    internal const string SyncCustomersJobName = SyncCustomersJob.JobName;
    internal const string LicenceSyncerJobName = LicenceSyncerJob.JobName;
}