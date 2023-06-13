using System.Collections.Generic;
using System.Threading.Tasks;

namespace IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

public interface ILinkListScanner
{
    Task<IEnumerable<LinkData>> ScanAsync(string linkListPath);
}