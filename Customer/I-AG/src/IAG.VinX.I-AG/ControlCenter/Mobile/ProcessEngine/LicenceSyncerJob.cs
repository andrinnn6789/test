using System;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Config;
using IAG.VinX.IAG.ControlCenter.Mobile.BusinessLayer;
using IAG.VinX.IAG.ControlCenter.Mobile.DataSyncer;
using IAG.VinX.IAG.Resource;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace IAG.VinX.IAG.ControlCenter.Mobile.ProcessEngine;

/// <summary>
/// Syncs all in VinX-I-AG defined tenants with their licenses and installations to the backend and read the current state of the licenses back
/// </summary>
[UsedImplicitly]
[JobInfo("37692910-D119-4BA6-99EF-924B5C8F5024", JobName)]
public class LicenceSyncerJob : JobBase<LicenceSyncerConfig, JobParameter, SyncResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "LicenceSyncer";

    private readonly ISybaseConnectionFactory _sybaseConnectionFactory;
    private readonly ILogger _logger;
    private readonly IControlCenterTokenRequest _requestToken;

    public LicenceSyncerJob(ISybaseConnectionFactory sybaseConnectionFactory, ILogger<LicenceSyncerJob> logger, IControlCenterTokenRequest requestToken)
    {
        _sybaseConnectionFactory = sybaseConnectionFactory;
        _logger = logger;
        _requestToken = requestToken;
    }

    protected override void ExecuteJob()
    {
        Result.SyncName = nameof(LicenceSyncerJob);
        var state = Infrastructure.GetJobData<SyncState>();
        var startDate = DateTime.Now;
        var sybaseConnection = _sybaseConnectionFactory.CreateConnection(Config.VinXConfig.ConnectionString);
        using var connectorIag = new VinXConnectorIag(sybaseConnection);
        try
        {
            var httpConfig = _requestToken.GetConfig(Config.BackendConfig, Endpoints.Control, 
                new RequestResponseLogger(_logger));
            var doFullSync = state.SyncCounter % Config.DiffSyncsPerFullSync == 0;
            var syncer = new LicenceSyncer(connectorIag, httpConfig, _logger, state.LastUpload, doFullSync);
            Result.SuccessCount = syncer.Sync().Result;
            state.LastUpload = startDate;
            state.SyncCounter++;
            Infrastructure.SetJobData(state);
            Result.Result = JobResultEnum.Success;
        }
        catch (Exception e)
        {
            Result.ErrorCount = 1;
            Result.Result = JobResultEnum.Failed;
            AddMessage(e);
        }

        base.ExecuteJob();
    }
}