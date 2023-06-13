using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Settings;
using IAG.VinX.IAG.Resource;

namespace IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

public class SettingsScanner : ISettingsScanner
{
    public const string SettingsDirectoryName = SettingsConst.SettingsPathName;

    public IEnumerable<ArtifactInfo> Scan(string settingsPath)
    {
        try
        {
            var results = new List<ArtifactInfo>();
            var dirInfo = new DirectoryInfo(settingsPath);
            foreach(var productDirectory in dirInfo.GetDirectories("*", SearchOption.AllDirectories))
            {
                var productName = productDirectory.Name;
                results.AddRange(productDirectory.EnumerateDirectories().Select(releaseDirectory
                    => new ArtifactInfo
                    {
                        ProductName = $"{productName} Standard-Einstellungen",
                        ArtifactPath = releaseDirectory.FullName,
                        ArtifactName = releaseDirectory.Name,
                        ProductType = ProductType.ConfigTemplate,
                        DependingProductName = productName,
                        Version = string.Empty
                    })
                );
            }

            return results;
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.ScanConfigurationsError, ex, settingsPath);
        }
    }
}