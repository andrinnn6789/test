using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;
using IAG.VinX.IAG.ControlCenter.Distribution.Rest;
using IAG.VinX.IAG.Resource;

using Microsoft.Extensions.Logging;

namespace IAG.VinX.IAG.ControlCenter.Distribution.ProcessEngine.PublishReleases;

[JobInfo("EADF321B-A122-4DED-BAEF-3ACB46D251EC", JobName)]
public class PublishReleasesJob : JobBase<PublishReleasesJobConfig, JobParameter, SyncResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "PublishReleasesJob";
        
    private readonly IControlCenterTokenRequest _requestToken;
    private readonly ILogger<PublishReleasesJob> _logger;

    public PublishReleasesJob(IControlCenterTokenRequest requestToken, ILogger<PublishReleasesJob> logger)
    {
        _requestToken = requestToken;
        _logger = logger;
    }

    protected override void ExecuteJob()
    {
        var backendClientFactory = new BackendClientFactory(Config.Backend, _requestToken, _logger);
        var productAdminClient = backendClientFactory.CreateRestClient<ProductAdminClient>(Endpoints.Distribution);
        var customerAdminClient = backendClientFactory.CreateRestClient<CustomerAdminClient>(Endpoints.Distribution);
        var createReleaseLogic = new ReleaseManager(productAdminClient, customerAdminClient, this);
        var jobLogic = new PublishReleasesLogic(new ArtifactsScanner(), new SettingsScanner(), createReleaseLogic, this);

        jobLogic.PublishReleasesAsync(Config.ArtifactsPath, Config.SettingsPath, Config.ReleasePaths, Result, Infrastructure).Wait();
        Result.Result = Result.ErrorCount == 0 ? JobResultEnum.Success : JobResultEnum.PartialSuccess;

        base.ExecuteJob();
    }
}