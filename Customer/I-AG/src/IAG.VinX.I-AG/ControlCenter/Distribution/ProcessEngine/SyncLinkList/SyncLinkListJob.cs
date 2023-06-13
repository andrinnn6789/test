using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;
using IAG.VinX.IAG.ControlCenter.Distribution.Rest;
using IAG.VinX.IAG.Resource;

using Microsoft.Extensions.Logging;

namespace IAG.VinX.IAG.ControlCenter.Distribution.ProcessEngine.SyncLinkList;

[JobInfo("FD9E7BEB-CC14-45C5-9C5C-16A77E03C993", JobName)]
public class SyncLinkListJob : JobBase<SyncLinkListJobConfig, JobParameter, SyncResult>
{
    public const string JobName = ResourceIds.ResourcePrefixJob + "SyncLinkListJob";
        
    private readonly IControlCenterTokenRequest _requestToken;
    private readonly ILogger<SyncLinkListJob> _logger;

    public SyncLinkListJob(IControlCenterTokenRequest requestToken, ILogger<SyncLinkListJob> logger)
    {
        _requestToken = requestToken;
        _logger = logger;
    }

    protected override void ExecuteJob()
    {
        var backendClientFactory = new BackendClientFactory(Config.Backend, _requestToken, _logger);
        var linkAdminClient = backendClientFactory.CreateRestClient<LinkAdminClient>(Endpoints.Distribution);
        var jobLogic = new SyncLinkListLogic(new LinkListScanner(), linkAdminClient);

        jobLogic.SyncLinkListAsync(Config.LinkListsPath, Result, Infrastructure).Wait();
        Result.Result = Result.ErrorCount == 0 ? JobResultEnum.Success : JobResultEnum.PartialSuccess;

        base.ExecuteJob();
    }
}