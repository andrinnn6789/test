using IAG.Infrastructure.ProcessEngine.JobModel;

namespace IAG.VinX.Smith.HelloTess.ProcessEngine.ArticleCommonSyncJob;

public class HelloTessArticleCommonSyncResult : JobResult
{
    public int ArticleGroupSyncUpdatedCount { get; set; }

    public int ArticleGroupSyncInsertCount { get; set; }

    public int ArticleGroupSyncSetInactiveCount { get; set; }

    public int ArticleGroupSyncErrorCount { get; set; }

    public int ArticleSyncUpdatedCount { get; set; }

    public int ArticleSyncInsertCount { get; set; }

    public int ArticleSyncSetInactiveCount { get; set; }

    public int ArticleSyncErrorCount { get; set; }
}