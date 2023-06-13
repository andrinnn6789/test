using System.Collections.Generic;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.Rest;
using IAG.VinX.Smith.HelloTess.MainSyncConfig;
using IAG.VinX.Smith.HelloTess.ProcessEngine.ArticleCommonSyncJob;
using IAG.VinX.Smith.Resource;

using Microsoft.Extensions.Logging;

namespace IAG.VinX.Smith.HelloTess.ProcessEngine;

[JobInfo("67DEDFED-1FBF-47D6-900C-DBBA3864BCAA", JobName)]
public class HelloTessMainSyncJob : JobBase<HelloTessMainSyncConfig, JobParameter, HelloTessMainSyncResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "helloTess.MainSync";

    private int _targetSyncErrorCount;
    private readonly ILogger<HelloTessArticleCommonSyncJob> _logger;
    private readonly ISybaseConnectionFactory _sybaseConnectionFactory;

    public HelloTessMainSyncJob(ISybaseConnectionFactory sybaseConnectionFactory, ILogger<HelloTessArticleCommonSyncJob> logger)
    {
        _sybaseConnectionFactory = sybaseConnectionFactory;
        _logger = logger;
    }

    protected override void ExecuteJob()
    {
        var sybaseConnection = _sybaseConnectionFactory.CreateConnection(Config.VinXConnectionString, _logger);
        // loop over target-systems
        foreach (var config in Config.HelloTessSystemConfigs)
        {
            var restConfig = GetHelloTessRestConfig(config.Url, config.ApiKey);

            var syncJob = new HelloTessArticleCommonSyncJob(sybaseConnection, _logger)
            {
                Config = GetArticleCommonSyncConfig(restConfig, config.PriceGroupForProdCost, config.CustomerForProdCost)
            };
            if (!syncJob.Execute(Infrastructure))
            {
                AddMessage(MessageTypeEnum.Error, ResourceIds.JobErrorMainSync, config.Name);
                _targetSyncErrorCount++;
            }

            var helloTessSystemSyncResult = new HelloTessSystemSyncResult
            {
                Name = config.Name,
                SyncResult = syncJob.Result
            };

            Result.HelloTessSystemSyncResults.Add(helloTessSystemSyncResult);
            HeartbeatAndCheckCancellation();
        }
            
        if (_targetSyncErrorCount >= 1)
        {
            Result.Result =  _targetSyncErrorCount == Config.HelloTessSystemConfigs.Count ? JobResultEnum.Failed : JobResultEnum.PartialSuccess;
        }

        base.ExecuteJob();
    }

    private static HttpConfig GetHelloTessRestConfig(string helloTessApiUrl, string helloTessApiKey)
    {
        var helloTessConfig = new HttpConfig
        {
            BaseUrl = helloTessApiUrl, HttpHeaders = new Dictionary<string, string>
            {
                {"hellotess-api-key", helloTessApiKey}
            }
        };

        return helloTessConfig;
    }

    private HelloTessArticleCommonSyncConfig GetArticleCommonSyncConfig(
        HttpConfig httpRestConfig, int priceGroupForProdCost, int customerForProdCost)
    {
        return new()
        {
            HelloTessRestConfig = httpRestConfig,
            SyncSystemDefaults = Config.SyncSystemDefaults,
            PriceGroupForProdCost = priceGroupForProdCost,
            CustomerForProdCost = customerForProdCost
        };
    }
}