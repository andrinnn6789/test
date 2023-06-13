using System;
using System.IO;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.Schüwo.Resource;
using IAG.VinX.Schüwo.SV.BusinessLogic;

namespace IAG.VinX.Schüwo.SV.ProcessEngine.DownloadOrder;

[JobInfo("35E85F74-E79D-4A83-B27D-C0F771EB8564", JobName)]
public class DownloadOrderJob : JobBase<DownloadOrderJobConfig, JobParameter, DownloadOrderJobResult>
{
    private readonly ISybaseConnectionFactory _sybaseConnectionFactory;

    public const string JobName = ResourceIds.ResourcePrefixJob + "SVDownloadOrder";

    public DownloadOrderJob(ISybaseConnectionFactory sybaseConnectionFactory)
    {
        _sybaseConnectionFactory = sybaseConnectionFactory;
    }

    protected override void ExecuteJob()
    {
        AddMessage(MessageTypeEnum.Debug, ResourceIds.SyncDebugStartInitFtpConnectorFormatMessage);
        using var ftpConnector = new FtpConnector(Config.FtpEndpointConfig, Config.FtpPathConfig, JobCancellationToken);
        AddMessage(MessageTypeEnum.Debug, ResourceIds.SyncDebugSuccessInitFtpConnectorFormatMessage);
        
        HeartbeatAndCheckCancellation();
        
        AddMessage(MessageTypeEnum.Debug, ResourceIds.SyncDebugStartInitOrderImporterFormatMessage);
        var orderImporter = new OrderImporter(_sybaseConnectionFactory.CreateConnection(), this, Result.ResultCounts, Config.ProviderId);
        AddMessage(MessageTypeEnum.Debug, ResourceIds.SyncDebugSuccessInitOrderImporterFormatMessage);
        
        HeartbeatAndCheckCancellation();
        
        AddMessage(MessageTypeEnum.Debug, ResourceIds.SyncDebugStartGetDownloadListFormatMessage);
        var fileNames = ftpConnector.GetDownloadList(JobCancellationToken);
        AddMessage(MessageTypeEnum.Debug, ResourceIds.SyncDebugSuccessGetDownloadListFormatMessage);

        HeartbeatAndCheckCancellation();
        
        foreach (var fileName in fileNames)
        {
            try
            {
                AddMessage(MessageTypeEnum.Debug, ResourceIds.SyncDebugStartDownloadFileFormatMessage, fileName);
                using var stream = new MemoryStream();
                ftpConnector.DownloadFile(stream, fileName, JobCancellationToken);
                AddMessage(MessageTypeEnum.Debug, ResourceIds.SyncDebugSuccessDownloadFileFormatMessage, fileName);
                
                AddMessage(MessageTypeEnum.Debug, ResourceIds.SyncDebugStartImportOrderFormatMessage, fileName);
                orderImporter.Import(stream);
                AddMessage(MessageTypeEnum.Debug, ResourceIds.SyncDebugSuccessImportOrderFormatMessage, fileName);
                
                AddMessage(MessageTypeEnum.Debug, ResourceIds.SyncDebugStartDeleteFileFormatMessage, fileName);
                ftpConnector.DeleteFile(fileName, JobCancellationToken);
                AddMessage(MessageTypeEnum.Debug, ResourceIds.SyncDebugSuccessDeleteFileFormatMessage, fileName);
            }
            catch (Exception e)
            {
                AddMessage(MessageTypeEnum.Error, e.Message);
                AddMessage(e);
                Result.ErrorCount++;
            }
            
            HeartbeatAndCheckCancellation();
        }

        Result.Result = Result.ErrorCount == 0 
            ? Result.ResultCounts.WarningCount == 0 
                ? JobResultEnum.Success 
                : JobResultEnum.PartialSuccess 
            : Result.OrderCount > 0 
                ? JobResultEnum.PartialSuccess 
                : JobResultEnum.Failed;

        base.ExecuteJob();
    }
}