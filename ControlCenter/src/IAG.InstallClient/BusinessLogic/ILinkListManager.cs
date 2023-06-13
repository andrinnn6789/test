using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;

namespace IAG.InstallClient.BusinessLogic;

public interface ILinkListManager
{
    Task<IEnumerable<LinkInfo>> GetLinksAsync(Guid id);
}