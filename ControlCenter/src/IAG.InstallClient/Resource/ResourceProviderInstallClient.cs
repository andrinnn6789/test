using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Globalisation.ResourceProvider;

using JetBrains.Annotations;

namespace IAG.InstallClient.Resource;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class ResourceProviderInstallClient : ResourceProvider
{
    public ResourceProviderInstallClient()
    {
        AddTemplate(ResourceIds.ConfigCcBaseUrlMissingError, "en", "'ControlCenter:BaseUrl' not configured");
        AddTemplate(ResourceIds.ConfigIdentityServerUrlMissingError, "en", "'Authentication:IdentityServer' not configured and no integrated IS available");
        AddTemplate(ResourceIds.LoginFailedError, "en", "Login failed");
        AddTemplate(ResourceIds.UnknownProductPathError, "en", "Failed to determine the product path");
        AddTemplate(ResourceIds.UnknownCustomerError, "en", "Customer is unknown");
        AddTemplate(ResourceIds.UnknownReleaseError, "en", "Release is unknown");
        AddTemplate(ResourceIds.UnknownTargetInstanceError, "en", "Target instance is unknown");
        AddTemplate(ResourceIds.InstallServiceNoExeError, "en", "No exe file found in service path");
        AddTemplate(ResourceIds.InstallServiceTooMuchExesError, "en", "More than one exe file found in service path");
        AddTemplate(ResourceIds.GetCustomerError, "en", "Error during getting customer information: {0} ({1})");
        AddTemplate(ResourceIds.SelfUpdateStartProcessError, "en", "Failed to start the update process");
        AddTemplate(ResourceIds.ServiceNotFound, "en", "Service for instance {0} not found");

        AddTemplate(ResourceIds.ServiceControlErrorCreate, "en", "Failed to create service: {0}");
        AddTemplate(ResourceIds.ServiceControlErrorStart, "en", "Failed to start service: {0}");
        AddTemplate(ResourceIds.ServiceControlErrorStop, "en", "Failed to stop service: {0}");
        AddTemplate(ResourceIds.ServiceControlErrorDelete, "en", "Failed to delete service: {0}");

        // Texts from here: https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes--1000-1299-
        AddTemplate(ResourceIds.ServiceControlErrorCode + "1053", "en", "The service did not respond to the start or control request in a timely fashion");
        AddTemplate(ResourceIds.ServiceControlErrorCode + "1056", "en", "An instance of the service is already running");
        AddTemplate(ResourceIds.ServiceControlErrorCode + "1062", "en", "The service has not been started");
        AddTemplate(ResourceIds.ServiceControlErrorCode + "1067", "en", "The process terminated unexpectedly");
        AddTemplate(ResourceIds.ServiceControlErrorCode + "1073", "en", "The specified service already exists");
        AddTemplate(ResourceIds.ServiceControlErrorCodeOther, "en", "Exit code {0}");

        AddTemplate(ResourceIds.InstallationFailed, "en", "Installation failed: {0}");
        AddTemplate(ResourceIds.InstallationBackup, "en", "Backup existing installation");
        AddTemplate(ResourceIds.InstallationCleanup, "en", "Cleanup existing installation");
        AddTemplate(ResourceIds.InstallationInstallProduct, "en", "Install product");
        AddTemplate(ResourceIds.InstallationInstallExtension, "en", "Install customer extension");
        AddTemplate(ResourceIds.InstallationInstallConfiguration, "en", "Install configuration");
        AddTemplate(ResourceIds.InstallationGetFiles, "en", "Getting necessary files");
        AddTemplate(ResourceIds.InstallationStartService, "en", "Start service");

        AddTemplate(ResourceIds.SelfUpdateFailed, "en", "Self update failed: {0}");
        AddTemplate(ResourceIds.SelfUpdateNoCustomerInfo, "en", "No customer info available");
        AddTemplate(ResourceIds.SelfUpdateGetUpdaterProduct, "en", "Get updater product");
        AddTemplate(ResourceIds.SelfUpdateNoUpdaterProduct, "en", "No updater product available");
        AddTemplate(ResourceIds.SelfUpdateGetUpdaterRelease, "en", "Get updater release");
        AddTemplate(ResourceIds.SelfUpdateNoUpdaterRelease, "en", "No updater release available");
        AddTemplate(ResourceIds.SelfUpdateNoNewVersion, "en", "No new release available");
        AddTemplate(ResourceIds.SelfUpdateGetConfig, "en", "Get newest configuration");
        AddTemplate(ResourceIds.SelfUpdateStartProcess, "en", "Start self update process");
        AddTemplate(ResourceIds.SelfUpdateDone, "en", "Updated to version {0} (configuration: {1})");

        AddTemplate(ResourceIds.InventoryFailed, "en", "Inventory failed: {0}");
        AddTemplate(ResourceIds.InventoryNoCustomerInfo, "en", "No customer info available");
        AddTemplate(ResourceIds.InventoryGetInstallationsFailed, "en", "Failed to get installations: {0}");
        AddTemplate(ResourceIds.InventoryAddInstallationFailed, "en", "Failed to send installation data point to influx: {0}");

        AddTemplate(ResourceIds.TransferFailed, "en", "Transfer failed: {0}");
        AddTemplate(ResourceIds.TransferCopyFiles, "en", "Copy files");
        AddTemplate(ResourceIds.TransferRegisterInstallation, "en", "Register installation");
    }
}