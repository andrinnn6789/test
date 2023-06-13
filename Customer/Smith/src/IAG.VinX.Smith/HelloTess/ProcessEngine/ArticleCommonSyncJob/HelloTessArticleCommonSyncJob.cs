using IAG.Common.DataLayerSybase;
using IAG.Common.Resource;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.Smith.HelloTess.SyncLogic;
using IAG.VinX.Smith.HelloTess.SyncLogic.Handler;
using IAG.VinX.Smith.HelloTess.VinX;

using Microsoft.Extensions.Logging;

namespace IAG.VinX.Smith.HelloTess.ProcessEngine.ArticleCommonSyncJob;

[JobInfo("69360FC9-6366-40A6-A2D1-063E9367D2E5", JobName)]
public class HelloTessArticleCommonSyncJob : JobBase<HelloTessArticleCommonSyncConfig, JobParameter, HelloTessArticleCommonSyncResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "helloTess.ArticleSync";

    private readonly ILogger _logger;
    private readonly ISybaseConnection _sybaseConnection;

    public HelloTessArticleCommonSyncJob(ISybaseConnection sybaseConnection, ILogger<HelloTessArticleCommonSyncJob> logger)
    {
        _sybaseConnection = sybaseConnection;
        _logger = logger;
    }

    protected override void ExecuteJob()
    {  
        // sync article-groups
        var vxGroupClient = new VxArticleCategoryClient(_sybaseConnection);
        ISyncHandler syncHandler = new ArticleGroupSync(Infrastructure, this, _logger, Result, vxGroupClient, Config.HelloTessRestConfig, Config.SyncSystemDefaults);
        syncHandler.DoSync().Wait();
        var articleGroupSyncResult = syncHandler.CheckSyncJobResult();

        // sync articles
        var vxArticleClient = new VxArticleClient(_sybaseConnection);
        vxArticleClient.SetFilter(Config.PriceGroupForProdCost, Config.CustomerForProdCost);
        syncHandler = new ArticleSync(Infrastructure, this, _logger, Result, vxArticleClient, Config);
        syncHandler.DoSync().Wait();
        var articleSyncResult = syncHandler.CheckSyncJobResult();

        if (articleSyncResult == JobResultEnum.NoResult && articleGroupSyncResult == JobResultEnum.NoResult)
        {
            Result.Result = JobResultEnum.NoResult;
        }
        else
        {
            Result.Result = articleGroupSyncResult >= articleSyncResult ? articleGroupSyncResult : articleSyncResult;
        }

        base.ExecuteJob();
    }
}