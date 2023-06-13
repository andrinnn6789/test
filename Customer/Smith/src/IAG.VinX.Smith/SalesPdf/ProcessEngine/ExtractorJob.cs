using System;

using IAG.Common.DataLayerSybase;
using IAG.Common.Resource;
using IAG.Common.WoD.Interfaces;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace IAG.VinX.Smith.SalesPdf.ProcessEngine;

[UsedImplicitly]
[JobInfo("50C54F50-ABD6-4911-945F-BF707757F59E", JobName)]
public class ExtractorJob : JobBase<ExtractorJobConfig<ExtractorJob>, ExtractorJobParameter, ExtractorJobResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "SalesPdf";

    private readonly ISybaseConnectionFactory _sybaseConnectionFactory;
    private readonly ILogger<ExtractorJob> _logger;
    private readonly IWodConnector _wodConnector;

    public ExtractorJob(ISybaseConnectionFactory sybaseConnectionFactory, IWodConnector wodConnector, ILogger<ExtractorJob> logger)
    {
        _sybaseConnectionFactory = sybaseConnectionFactory;
        _logger = logger;
        _wodConnector = wodConnector;
    }

    protected override void ExecuteJob()
    {
        var state = Infrastructure.GetJobData<SyncState>();
        if (Parameter.RebuildAll)
            state.LastSync = DateTime.MinValue;
        var timestampStart = DateTime.Now;
        var sybaseConnection = _sybaseConnectionFactory.CreateConnection(Config.VinXConnectionString, _logger);
        var extractor = new DataExtractor(sybaseConnection, _wodConnector, _logger, Config.ExtractorWodConfig);
        bool anyError = false;
        foreach (var jobResult in extractor.Extract(state.LastSync))
        {
            anyError |= jobResult.ErrorCount > 0;
            Result.SyncResults.Add(jobResult);
            HeartbeatAndCheckCancellation();
        }
        state.LastSync = timestampStart;
        Result.Result = anyError ? JobResultEnum.PartialSuccess : JobResultEnum.Success;
        if (Result.Result == JobResultEnum.Success)
            Infrastructure.SetJobData(state);
        base.ExecuteJob();
    }
}