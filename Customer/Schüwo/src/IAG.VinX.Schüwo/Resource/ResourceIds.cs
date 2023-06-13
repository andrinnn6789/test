using IAG.VinX.Schüwo.SV.ProcessEngine.DownloadOrder;
using IAG.VinX.Schüwo.SV.ProcessEngine.UploadBaseData;
using IAG.VinX.Schüwo.SV.ProcessEngine.UploadImages;
using IAG.VinX.Schüwo.SV.ProcessEngine.UploadOrderData;

namespace IAG.VinX.Schüwo.Resource;

public static class ResourceIds
{
    private const string ResourcePrefix = "Schüwo.";

    // SV Sync Jobs
    internal const string SyncWarningMapUnitFormatMessage = ResourcePrefix + "Sync.WarningMapUnit {0}";
    internal const string SyncWarningUnsupportedImageExtensionFormatMessage = ResourcePrefix + "Sync.WarningUnsupportedImageExtensionFormatMessage {0}";
    internal const string SyncWarningInvalidImageFileNameFormatMessage = ResourcePrefix + "Sync.WarningInvalidImageFileNameFormatMessage {0}";
    internal const string SyncWarningSyncImageFormatMessage = ResourcePrefix + "Sync.WarningSyncImageFormatMessage {0} {1}";
    internal const string SyncWarningDeleteImageFormatMessage = ResourcePrefix + "Sync.WarningDeleteImageFormatMessage {0} {1}";
    internal const string SyncErrorUnknownArticle = ResourcePrefix + "Sync.ErrorUnknownArticle {0}";
    
    // SV Sync Jobs Debug
    internal const string SyncDebugStartInitFtpConnectorFormatMessage = ResourcePrefix + "Sync.StartInitFtpConnector";
    internal const string SyncDebugSuccessInitFtpConnectorFormatMessage = ResourcePrefix + "Sync.SuccessInitFtpConnector";
    internal const string SyncDebugStartInitOrderImporterFormatMessage = ResourcePrefix + "Sync.StartInitOrderImporter";
    internal const string SyncDebugSuccessInitOrderImporterFormatMessage = ResourcePrefix + "Sync.SuccessInitOrderImporter";
    internal const string SyncDebugStartGetDownloadListFormatMessage = ResourcePrefix + "Sync.StartGetDownloadList";
    internal const string SyncDebugSuccessGetDownloadListFormatMessage = ResourcePrefix + "Sync.SuccessGetDownloadList";
    internal const string SyncDebugStartDownloadFileFormatMessage = ResourcePrefix + "Sync.StartDownloadFile: {0}";
    internal const string SyncDebugSuccessDownloadFileFormatMessage = ResourcePrefix + "Sync.SuccessDownloadFile: {0}";
    internal const string SyncDebugStartImportOrderFormatMessage = ResourcePrefix + "Sync.StartImportOrder: {0}";
    internal const string SyncDebugSuccessImportOrderFormatMessage = ResourcePrefix + "Sync.SuccessImportOrder: {0}";
    internal const string SyncDebugStartDeleteFileFormatMessage = ResourcePrefix + "Sync.StartDeleteFile: {0}";
    internal const string SyncDebugSuccessDeleteFileFormatMessage = ResourcePrefix + "Sync.SuccessDeleteFile: {0}";

    // jobs
    public const string ResourcePrefixJob = ResourcePrefix + "Job.";
    internal const string UploadBaseDataJobName = UploadBaseDataJob.JobName;
    internal const string UploadImagesJobName = UploadImagesJob.JobName;
    internal const string UploadOrderDataJobName = UploadOrderDataJob.JobName;
    internal const string DownloadOrderJobName = DownloadOrderJob.JobName;
}