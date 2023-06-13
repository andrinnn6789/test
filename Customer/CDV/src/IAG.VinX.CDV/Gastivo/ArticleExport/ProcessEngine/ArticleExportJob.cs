using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.CDV.Gastivo.ArticleExport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.Common.ProcessEngine;

namespace IAG.VinX.CDV.Gastivo.ArticleExport.ProcessEngine;

[JobInfo("2338B011-6E77-4BE3-899B-0457733273C3", JobName)]
public class ArticleExportJob : JobBase<ArticleExportJobConfig, JobParameter, GastivoExportJobResult>
{
    private const string JobName = Resource.ResourceIds.GastivoArticleExportJobName;

    private readonly IArticleExporter _articleExporter;

    public ArticleExportJob(IArticleExporter articleExporter)
    {
        _articleExporter = articleExporter;
    }

    protected override void ExecuteJob()
    {
        var imageUrlTemplate = !string.IsNullOrEmpty(Config.ImageUrlTemplate)
            ? Config.ImageUrlTemplate
            : "https://www.casadelvino.ch/ShopImage/artikel/list/{0}/0/{0}.jpg?width=1140&height=1140";
        
        _articleExporter.SetConfig(Config.GastivoFtpConfig, Config.ConnectionString, imageUrlTemplate, this);

        var result = _articleExporter.ExportArticles();

        Result.Result = result.Result;
        Result.ExportedCount = result.ExportedCount;
        Result.ErrorCount = result.ErrorCount;

        base.ExecuteJob();
    }
}