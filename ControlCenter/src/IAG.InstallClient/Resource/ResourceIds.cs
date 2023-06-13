using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace IAG.InstallClient.Resource;

[ExcludeFromCodeCoverage]
public static class ResourceIds
{
    private const string ResourcePrefix = "InstallClient.";

    internal const string ConfigCcBaseUrlMissingError = ResourcePrefix + "ConfigCcBaseUrlMissing.Error";
    internal const string ConfigIdentityServerUrlMissingError = ResourcePrefix + "ConfigIdentityServerUrlMissing.Error";
    internal const string LoginFailedError = ResourcePrefix + "LoginFailed.Error";
    internal const string UnknownProductPathError = ResourcePrefix + "UnknownProductPath.Error";
    internal const string UnknownCustomerError = ResourcePrefix + "UnknownCustomer.Error";
    internal const string UnknownReleaseError = ResourcePrefix + "UnknownRelease.Error";
    internal const string UnknownTargetInstanceError = ResourcePrefix + "UnknownTargetInstance.Error";
    internal const string InstallServiceNoExeError = ResourcePrefix + "InstallServiceNoExe.Error";
    internal const string InstallServiceTooMuchExesError = ResourcePrefix + "InstallServiceTooMuchExes.Error";
    internal const string GetCustomerError = ResourcePrefix + "GetCustomer.Error";
    internal const string SelfUpdateStartProcessError = ResourcePrefix + "SelfUpdate.StartProcess.Error";
    internal const string ServiceNotFound = ResourcePrefix + "ServiceNotFound";

    internal const string ServiceControlErrorPrefix = ResourcePrefix + "ServiceControl.Error.";
    internal const string ServiceControlErrorCreate = ServiceControlErrorPrefix + "Create";
    internal const string ServiceControlErrorStart = ServiceControlErrorPrefix + "Start";
    internal const string ServiceControlErrorStop = ServiceControlErrorPrefix + "Stop";
    internal const string ServiceControlErrorDelete = ServiceControlErrorPrefix + "Delete";
    internal const string ServiceControlErrorCode = ServiceControlErrorPrefix + "Code";
    internal const string ServiceControlErrorCodeOther = ServiceControlErrorPrefix + "CodeOther";

    internal static readonly HashSet<int> ServiceControlErrorCodes = new() { 1053, 1056, 1062, 1067, 1073 };

    internal const string InstallationFailed = ResourcePrefix + "Installation." + "Installation failed: {0}";
    internal const string InstallationBackup = ResourcePrefix + "Installation." + "Backup existing installation";
    internal const string InstallationCleanup = ResourcePrefix + "Installation." + "Cleanup existing installation";
    internal const string InstallationInstallProduct = ResourcePrefix + "Installation." + "Install product";
    internal const string InstallationInstallExtension = ResourcePrefix + "Installation." + "Install customer extension";
    internal const string InstallationInstallConfiguration = ResourcePrefix + "Installation." + "Install configuration";
    internal const string InstallationGetFiles = ResourcePrefix + "Installation." + "Getting necessary files";
    internal const string InstallationStartService = ResourcePrefix + "Installation." + "Start service";

    internal const string SelfUpdateFailed = ResourcePrefix + "SelfUpdate." + "Self update failed: {0}";
    internal const string SelfUpdateNoCustomerInfo = ResourcePrefix + "SelfUpdate." + "No customer info available";
    internal const string SelfUpdateGetUpdaterProduct = ResourcePrefix + "SelfUpdate." + "get updater product";
    internal const string SelfUpdateNoUpdaterProduct = ResourcePrefix + "SelfUpdate." + "No updater product available";
    internal const string SelfUpdateGetUpdaterRelease = ResourcePrefix + "SelfUpdate." + "get updater release";
    internal const string SelfUpdateNoUpdaterRelease = ResourcePrefix + "SelfUpdate." + "No updater release available";
    internal const string SelfUpdateNoNewVersion = ResourcePrefix + "SelfUpdate." + "No new release available";
    internal const string SelfUpdateGetConfig = ResourcePrefix + "SelfUpdate." + "get newest configuration";
    internal const string SelfUpdateStartProcess = ResourcePrefix + "SelfUpdate." + "Start self update process";
    internal const string SelfUpdateDone = ResourcePrefix + "SelfUpdate." + "Updated to version {0} (configuration: {1})";

    internal const string InventoryFailed = ResourcePrefix + "Inventory." + "Inventory failed: {0}";
    internal const string InventoryNoCustomerInfo = ResourcePrefix + "Inventory." + "No customer info available";
    internal const string InventoryGetInstallationsFailed = ResourcePrefix + "Inventory." + "Failed to get installations: {0}";
    internal const string InventoryAddInstallationFailed = ResourcePrefix + "Inventory." + "Failed to send installation data point to influx: {0}";

    internal const string TransferFailed = ResourcePrefix + "Transfer." + "Transfer failed: {0}";
    internal const string TransferCopyFiles = ResourcePrefix + "Transfer." + "Copy files";
    internal const string TransferRegisterInstallation = ResourcePrefix + "Transfer." + "Register installation";

    // job resources
    internal const string ResourcePrefixJob = ResourcePrefix + "Job.";
}