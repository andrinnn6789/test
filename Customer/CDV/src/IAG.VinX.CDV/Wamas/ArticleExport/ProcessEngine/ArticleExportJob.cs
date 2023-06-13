using System;

using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.CDV.Wamas.ArticleExport.BusinessLogic;
using IAG.VinX.CDV.Wamas.Common.ProcessEngine;

namespace IAG.VinX.CDV.Wamas.ArticleExport.ProcessEngine;

[JobInfo("82FB09AF-6263-4D7F-87D2-F6FDE111B236", JobName)]
public class ArticleExportJob : JobBase<ArticleExportJobConfig, JobParameter, WamasExportJobResult>
{
    private const string JobName = Resource.ResourceIds.WamasArticleExportJobName;

    private readonly IArticleExporter _articleExporter;

    public ArticleExportJob(IArticleExporter articleExporter)
    {
        _articleExporter = articleExporter;
    }

    protected override void ExecuteJob()
    {
        var state = Infrastructure.GetJobData<ArticleExportJobState>();
        var timestampStart = DateTime.Now;
        
        _articleExporter.SetConfig(Config.WamasFtpConfig, Config.ConnectionString, this);

        var result = _articleExporter.ExportArticles(state.LastSync);

        Result.Result = result.Result;
        Result.ExportedCount = result.ExportedCount;
        Result.ErrorCount = result.ErrorCount;

        state.LastSync = timestampStart;

        if(Result.Result != JobResultEnum.Failed)
            Infrastructure.SetJobData(state);

        base.ExecuteJob();
    }
}