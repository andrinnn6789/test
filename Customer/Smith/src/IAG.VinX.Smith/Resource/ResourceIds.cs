using IAG.VinX.Smith.BossExport.ProcessEngine;
using IAG.VinX.Smith.HelloTess.ProcessEngine;
using IAG.VinX.Smith.HelloTess.ProcessEngine.ArticleCommonSyncJob;
using IAG.VinX.Smith.SalesPdf.ProcessEngine;

namespace IAG.VinX.Smith.Resource;

public static class ResourceIds
{
    private const string ResourcePrefix = "Smith.";

    // MainJob
    internal const string JobErrorMainSync = ResourcePrefix + "JobError.MainSync";


    // CommonJob

    internal const string SyncErrorGetSource = ResourcePrefix + "Sync.ErrorGetSource";

    internal const string SyncErrorGetTarget = ResourcePrefix + "Sync.ErrorGetTarget";

    internal const string SyncErrorUpdateFormatMessage = ResourcePrefix + "Sync.ErrorUpdateFormatMessage";

    internal const string SyncErrorInsertFormatMessage = ResourcePrefix + "Sync.ErrorInsertFormatMessage";

    internal const string SyncErrorDeleteFormatMessage = ResourcePrefix + "Sync.ErrorDeleteFormatMessage";

    internal const string ArticleGroupSingular = ResourcePrefix + "ArticleGroup.Singular";

    internal const string ArticleGroupPlural = ResourcePrefix + "ArticleGroup.Plural";

    internal const string ArticleSingular = ResourcePrefix + "Article.Singular";

    internal const string ArticlePlural = ResourcePrefix + "Article.Plural";

    // jobs
    public const string ResourcePrefixJob = ResourcePrefix + "Job.";
    internal const string ExtractorJobName = ExtractorJob.JobName;
    internal const string HelloTessMainSyncJobName = HelloTessMainSyncJob.JobName;
    internal const string HelloTessArticleCommonSyncJobName = HelloTessArticleCommonSyncJob.JobName;
    internal const string BossExportName = BossExportJob.JobName;
}