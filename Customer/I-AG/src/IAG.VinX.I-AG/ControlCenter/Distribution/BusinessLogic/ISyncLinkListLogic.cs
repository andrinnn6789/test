using System.Threading.Tasks;

using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.IAG.ControlCenter.Common;

namespace IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

public interface ISyncLinkListLogic
{
    Task SyncLinkListAsync(string linkListPath, SyncResult syncResult, IJobHeartbeatObserver jobHeartbeatObserver);
}