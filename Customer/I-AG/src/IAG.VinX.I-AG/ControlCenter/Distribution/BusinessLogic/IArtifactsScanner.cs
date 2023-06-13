using System.Collections.Generic;

namespace IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

public interface IArtifactsScanner
{
    IEnumerable<ArtifactInfo> Scan(string artifactsPath);
}