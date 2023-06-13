using System.Threading.Tasks;

using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.Smith.HelloTess.DataMapper;
using IAG.VinX.Smith.HelloTess.Dto;
using IAG.VinX.Smith.HelloTess.HelloTessRest;
using IAG.VinX.Smith.HelloTess.HelloTessRest.Dto;
using IAG.VinX.Smith.HelloTess.ProcessEngine.ArticleCommonSyncJob;
using IAG.VinX.Smith.HelloTess.VinX;
using IAG.VinX.Smith.Resource;

using Microsoft.Extensions.Logging;

namespace IAG.VinX.Smith.HelloTess.SyncLogic.Handler;

public class ArticleSync : BaseSyncHandler<VxArticle, Article>, ISyncHandler
{
    private readonly IVinXClient<VxArticle> _vxClient;
    private readonly ArticleClient _helloTessClient;
    private readonly ArticleDataMapper _articleDataMapper;

    public ArticleSync(IJobHeartbeatObserver jobHeartbeatObserver, IMessageLogger msgLogger, ILogger logger,
        HelloTessArticleCommonSyncResult commonSyncResult,
        IVinXClient<VxArticle> vxClient, HelloTessArticleCommonSyncConfig config)
        : base(jobHeartbeatObserver, msgLogger, commonSyncResult)
    {
        _vxClient = vxClient;
        var requestLogger = new RequestResponseLogger(logger);
        _helloTessClient = new ArticleClient(config.HelloTessRestConfig, requestLogger);
        _articleDataMapper = new ArticleDataMapper(config.SyncSystemDefaults);
    }

    protected override string AspectNameSingularResourceId => ResourceIds.ArticleSingular;

    protected override string AspectNamePluralResourceId => ResourceIds.ArticlePlural;

    protected override int UpdateCount
    {
        get => CommonSyncResult.ArticleSyncUpdatedCount;
        set => CommonSyncResult.ArticleSyncUpdatedCount = value;
    }

    protected override int InsertCount
    {
        get => CommonSyncResult.ArticleSyncInsertCount;
        set => CommonSyncResult.ArticleSyncInsertCount = value;
    }

    protected override int SetInactiveCount
    {
        get => CommonSyncResult.ArticleSyncSetInactiveCount;
        set => CommonSyncResult.ArticleSyncSetInactiveCount = value;
    }

    protected override int ErrorCount
    {
        get => CommonSyncResult.ArticleSyncErrorCount;
        set => CommonSyncResult.ArticleSyncErrorCount = value;
    }

    public async Task DoSync()
    {
        await DoSync(_vxClient, _helloTessClient, _articleDataMapper);
    }
}