using System;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.Smith.BossExport.BusinessLogic;
using IAG.VinX.Smith.BossExport.Dto;
using IAG.VinX.Smith.Resource;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace IAG.VinX.Smith.BossExport.ProcessEngine;

[UsedImplicitly]
[JobInfo("17637E88-2B2F-43E1-BF95-8BC559FBD398", JobName)]
public class BossExportJob : JobBase<BossExportJobConfig, BossExportJobParameter, BossExportJobResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "BossExport";

    private readonly ISybaseConnectionFactory _sybaseConnectionFactory;
    private readonly ILogger<BossExportJob> _logger;

    public BossExportJob(ISybaseConnectionFactory sybaseConnectionFactory, ILogger<BossExportJob> logger)
    {
        _sybaseConnectionFactory = sybaseConnectionFactory;
        _logger = logger;
    }

    protected override void ExecuteJob()
    {
        var state = Infrastructure.GetJobData<BossExportSyncState>() ?? new BossExportSyncState();
        var allArticles = _sybaseConnectionFactory.CreateConnection(Config.VinXConnectionString, _logger)
            .GetQueryable<ArticleBoss>().ToList();
        var exportArticles = new NewIdFinder().GetNewArticles(allArticles, state.ExportedIds, Parameter.UpdateExported,
            Parameter.ExportAll);
        HeartbeatAndCheckCancellation();
        new SendAsMail().Send(
            Config.MailConfig,
            new ExcelWriter().GetExcel(exportArticles),
            $"Export {DateTime.Now.ToShortDateString()}.xlsx",
            Config.MailReceiver);
        HeartbeatAndCheckCancellation();
        Infrastructure.SetJobData(state);
        Result.Result = JobResultEnum.Success;
        Result.ExportCount = exportArticles.Count;
        base.ExecuteJob();
    }
}