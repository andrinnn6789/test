using System.Collections.Generic;

namespace IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

public interface ISettingsScanner
{
    IEnumerable<ArtifactInfo> Scan(string settingsPath);
}