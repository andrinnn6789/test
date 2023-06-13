using System.Threading.Tasks;

using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.Rest;
using IAG.VinX.Smith.HelloTess.DataMapper;
using IAG.VinX.Smith.HelloTess.Dto;
using IAG.VinX.Smith.HelloTess.HelloTessRest;
using IAG.VinX.Smith.HelloTess.HelloTessRest.Dto;
using IAG.VinX.Smith.HelloTess.MainSyncConfig;
using IAG.VinX.Smith.HelloTess.ProcessEngine.ArticleCommonSyncJob;
using IAG.VinX.Smith.HelloTess.VinX;
using IAG.VinX.Smith.Resource;

using Microsoft.Extensions.Logging;

namespace IAG.VinX.Smith.HelloTess.SyncLogic.Handler;

public class ArticleGroupSync : BaseSyncHandler<VxArticleCategory, ArticleGroup>, ISyncHandler
{
    private readonly IVinXClient<VxArticleCategory> _atlasClient;

    private readonly ArticleGroupClient _helloTessClient;

    private readonly ArticleGroupDataMapper _articleGroupDataMapper;
        
    public ArticleGroupSync(IJobHeartbeatObserver jobHeartbeatObserver, IMessageLogger msgLogger, ILogger logger,
        HelloTessArticleCommonSyncResult commonSyncResult,
        IVinXClient<VxArticleCategory> vxClient, IHttpConfig helloTessRestConfig, SyncSystemDefaults syncSystemDefaults)
        : base(jobHeartbeatObserver, msgLogger, commonSyncResult)
    {
        var requestLogger = new RequestResponseLogger(logger);
        _atlasClient = vxClient;
        _helloTessClient = new ArticleGroupClient(helloTessRestConfig, requestLogger);
        _articleGroupDataMapper = new ArticleGroupDataMapper(syncSystemDefaults);
    }

    protected override string AspectNameSingularResourceId => ResourceIds.ArticleGroupSingular;

    protected override string AspectNamePluralResourceId => ResourceIds.ArticleGroupPlural;

    protected override int UpdateCount
    {
        get => CommonSyncResult.ArticleGroupSyncUpdatedCount;
        set => CommonSyncResult.ArticleGroupSyncUpdatedCount = value;
    }

    protected override int InsertCount
    {
        get => CommonSyncResult.ArticleGroupSyncInsertCount;
        set => CommonSyncResult.ArticleGroupSyncInsertCount = value;
    }

    protected override int SetInactiveCount
    {
        get => CommonSyncResult.ArticleGroupSyncSetInactiveCount;
        set => CommonSyncResult.ArticleGroupSyncSetInactiveCount = value;
    }

    protected override int ErrorCount
    {
        get => CommonSyncResult.ArticleGroupSyncErrorCount;
        set => CommonSyncResult.ArticleGroupSyncErrorCount = value;
    }

    public async Task DoSync()
    {
        await DoSync(_atlasClient, _helloTessClient, _articleGroupDataMapper);
    }
}