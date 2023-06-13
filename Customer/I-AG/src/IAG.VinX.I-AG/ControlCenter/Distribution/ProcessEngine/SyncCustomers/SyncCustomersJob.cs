using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;
using IAG.VinX.IAG.ControlCenter.Distribution.DataLayer;
using IAG.VinX.IAG.ControlCenter.Distribution.Rest;
using IAG.VinX.IAG.Resource;

using Microsoft.Extensions.Logging;

namespace IAG.VinX.IAG.ControlCenter.Distribution.ProcessEngine.SyncCustomers;

[JobInfo("2371C276-930B-4F23-BF19-870ABCB99D98", JobName)]
public class SyncCustomersJob : JobBase<SyncCustomersJobConfig, JobParameter, SyncResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "SyncCustomers";

    private readonly ISybaseConnectionFactory _sybaseConnectionFactory;
    private readonly IControlCenterTokenRequest _requestToken;
    private readonly ILogger<SyncCustomersJob> _logger;

    public SyncCustomersJob(ISybaseConnectionFactory sybaseConnectionFactory, IControlCenterTokenRequest requestToken, ILogger<SyncCustomersJob> logger)
    {
        _sybaseConnectionFactory = sybaseConnectionFactory;
        _requestToken = requestToken;
        _logger = logger;
    }

    protected override void ExecuteJob()
    {
        var backendClientFactory = new BackendClientFactory(Config.Backend, _requestToken, _logger);
        var productAdminClient = backendClientFactory.CreateRestClient<ProductAdminClient>(Endpoints.Distribution);
        var customerAdminClient = backendClientFactory.CreateRestClient<CustomerAdminClient>(Endpoints.Distribution);
        var sybaseConnection = _sybaseConnectionFactory.CreateConnection(Config.VinX.ConnectionString);
        using var customerRepository = new CustomerRepository(sybaseConnection);

        var jobLogic = new CustomerSyncLogic(customerAdminClient, productAdminClient, customerRepository, this, Result);

        jobLogic
            .DoSyncAsync(Infrastructure)
            .Wait();

        Result.Result = Result.ErrorCount == 0 ? JobResultEnum.Success : JobResultEnum.PartialSuccess;
        base.ExecuteJob();
    }
}