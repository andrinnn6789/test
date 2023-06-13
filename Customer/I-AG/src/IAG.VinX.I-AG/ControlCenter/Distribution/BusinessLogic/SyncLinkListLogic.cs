using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Distribution.Rest;

namespace IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

public class SyncLinkListLogic : ISyncLinkListLogic
{
    private readonly ILinkListScanner _linkListScanner;
    private readonly ILinkAdminClient _linkAdminClient;

    public SyncLinkListLogic(ILinkListScanner linkListScanner, ILinkAdminClient linkAdminClient)
    {
        _linkListScanner = linkListScanner;
        _linkAdminClient = linkAdminClient;
    }

    public async Task SyncLinkListAsync(string linkListPath, SyncResult syncResult, IJobHeartbeatObserver jobHeartbeatObserver)
    {
        var linkList = (await _linkListScanner.ScanAsync(linkListPath)).ToList();
        jobHeartbeatObserver.HeartbeatAndCheckJobCancellation();

        if (linkList.Any())
        {
            var links = await _linkAdminClient.SyncLinksAsync(linkList);
            syncResult.SuccessCount = links.Count();
        }
        else
        {
            syncResult.SuccessCount = 0;
        }
    }
}