using System;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.Schüwo.Resource;
using IAG.VinX.Schüwo.SV.BusinessLogic;

namespace IAG.VinX.Schüwo.SV.ProcessEngine.UploadImages;

[JobInfo("65F09661-E05E-42E8-97E9-1EBD1AA7A953", JobName)]
public class UploadImagesJob : SvBaseJob<UploadImagesJobConfig, JobParameter, UploadImagesJobResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "SVUploadImages";

    public UploadImagesJob(ISybaseConnectionFactory sybaseConnectionFactory)
        : base(sybaseConnectionFactory)
    {
    }

    protected override void Sync()
    {
        var state = Infrastructure.GetJobData<SyncState>();
        var timestampStart = DateTime.Now;
        using var ftpConnector = new FtpConnector(Config.FtpEndpointConfig, Config.FtpPathConfig);
        var imageSyncer = new ImageSyncer(ftpConnector, this, Result);

        imageSyncer.SyncImagesToFtp(
            Extractor.GetActiveArticles(), 
            Config.ArticleImageSourcePath, 
            Config.ArticleImageArchivePath, 
            state.LastSync);

        Result.Result = Result.ResultCounts.WarningCount == 0 && Result.ErrorCount == 0 ? JobResultEnum.Success : JobResultEnum.PartialSuccess;
        state.LastSync = timestampStart;
        if (Result.ErrorCount == 0)
            Infrastructure.SetJobData(state);
    }
}