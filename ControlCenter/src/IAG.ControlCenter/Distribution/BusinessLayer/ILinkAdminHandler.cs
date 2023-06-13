using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;

namespace IAG.ControlCenter.Distribution.BusinessLayer;

public interface ILinkAdminHandler
{
    Task<List<LinkInfo>> SyncLinksAsync(List<LinkRegistration> links);
}