using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.Rest;
using IAG.VinX.Smith.HelloTess.MainSyncConfig;

namespace IAG.VinX.Smith.HelloTess.ProcessEngine.ArticleCommonSyncJob;

public class HelloTessArticleCommonSyncConfig : JobConfig<HelloTessArticleCommonSyncJob>
{
    public HttpConfig HelloTessRestConfig { get; set; }

    public SyncSystemDefaults SyncSystemDefaults { get; set; }

    public int PriceGroupForProdCost { get; set; }

    public int CustomerForProdCost { get; set; }
}