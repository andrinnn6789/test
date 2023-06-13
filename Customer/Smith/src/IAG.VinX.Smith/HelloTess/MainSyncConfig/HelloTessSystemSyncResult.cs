using IAG.VinX.Smith.HelloTess.ProcessEngine.ArticleCommonSyncJob;

namespace IAG.VinX.Smith.HelloTess.MainSyncConfig;

public class HelloTessSystemSyncResult
{
    public HelloTessSystemSyncResult()
    {
        SyncResult = new HelloTessArticleCommonSyncResult();
    }

    public string Name { get; set; }

    public HelloTessArticleCommonSyncResult SyncResult { get; set; }
}