using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

namespace IAG.VinX.IAG.ControlCenter.Distribution.Rest;

public interface ILinkAdminClient
{
    Task<IEnumerable<LinkInfo>> SyncLinksAsync(IEnumerable<LinkData> links);
}