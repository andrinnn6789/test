using System;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.Greiner.EslManager.BusinessLogic;
using IAG.VinX.Greiner.Resource;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace IAG.VinX.Greiner.EslManager.ProcessEngine;

[UsedImplicitly]
[JobInfo("0B3C48DE-8CCA-4204-9751-6BE0334509E0", JobName)]
public class ExtractorJob : JobBase<ExtractorJobConfig, JobParameter, ExtractorJobResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "EslExport";

    private readonly ISybaseConnectionFactory _sybaseConnectionFactory;
    private readonly ILogger<ExtractorJob> _logger;

    public ExtractorJob(ISybaseConnectionFactory sybaseConnectionFactory, ILogger<ExtractorJob> logger)
    {
        _sybaseConnectionFactory = sybaseConnectionFactory;
        _logger = logger;
    }

    protected override void ExecuteJob()
    {
        try
        {
            var sybaseConnection = _sybaseConnectionFactory.CreateConnection(Config.VinXConnectionString, _logger);
            var extractor = new DataExtractor(sybaseConnection);
            var exporter = new DataExporter(Config.EslExportConfig);
            exporter.ExportFile("Artikel", extractor.ExtractArticles());
            Result.SuccessCount++;
            exporter.ExportFile("ArtikelLöschen", extractor.ExtractArticlesToDelete());
            Result.SuccessCount++;
            Result.Result = JobResultEnum.Success;
        }
        catch (Exception e)
        {
            AddMessage(MessageTypeEnum.Error, IAG.Infrastructure.Resource.ResourceIds.GenericError, e.Message);
            AddMessage(MessageTypeEnum.Debug, IAG.Infrastructure.Resource.ResourceIds.GenericError, e);
            Result.Result = JobResultEnum.Failed;
            Result.ErrorCount++;
        }

        base.ExecuteJob();
    }
}