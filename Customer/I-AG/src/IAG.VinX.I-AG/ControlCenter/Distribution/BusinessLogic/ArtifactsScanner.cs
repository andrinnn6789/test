using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using IAG.ControlCenter.Distribution.BusinessLayer;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Exception;
using IAG.VinX.IAG.Resource;

namespace IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

public class ArtifactsScanner : IArtifactsScanner
{
    private const string PublishDir = "publish";
    private const string MasterRelease = "master";
    private const string DevelopRelease = "develop";
    private const string RcRelease = "release-candidates";
    private const string InstallerToolDirectoryName = "IAG.InstallClient.HostWindows";
    private const string InstallerToolName = "Installer";
    public const string VinXDirectoryName = "VinX";
    public const string PerformXDirectoryName = "PerformX";
    public const string CustomerExtensionsDirectoryName = "Customer";

    public IEnumerable<ArtifactInfo> Scan(string artifactsPath)
    {
        try
        {
            var dirInfo = new DirectoryInfo(artifactsPath);
            var publishDirectories = dirInfo.GetDirectories($"*{PublishDir}", SearchOption.AllDirectories);
            var results = new List<ArtifactInfo>();

            foreach (var publishDir in publishDirectories)
            {
                var artifact = new ArtifactInfo
                {
                    ArtifactPath = publishDir.FullName
                };
                var dirParts = publishDir
                    .FullName
                    .Remove(0, artifactsPath.Length)
                    .TrimStart(Path.DirectorySeparatorChar)
                    .Split(Path.DirectorySeparatorChar);

                if (!IsArtifactToPublish(dirParts))
                {
                    continue;
                }
                if (Array.IndexOf(dirParts, VinXDirectoryName) >= 0)
                {
                    artifact.ProductName = VinXDirectoryName;
                    artifact.ProductType = ProductType.IagService;
                }
                else if (Array.IndexOf(dirParts, PerformXDirectoryName) >= 0)
                {
                    artifact.ProductName = PerformXDirectoryName;
                    artifact.ProductType = ProductType.IagService;
                }
                else if (Array.IndexOf(dirParts, InstallerToolDirectoryName) >= 0)
                {
                    artifact.ProductName = InstallerToolName;
                    artifact.ProductType = ProductType.Updater;
                }
                else
                {
                    int customerDirIndex;
                    if ((customerDirIndex = Array.IndexOf(dirParts, CustomerExtensionsDirectoryName)) >= 0)
                    {
                        artifact.ProductName = dirParts[customerDirIndex+1];
                        artifact.ProductType = ProductType.CustomerExtension;
                        artifact.DependingProductName = GetProductName(publishDir.FullName);
                    }
                    else
                    {
                        continue;
                    }
                }

                var fileCollector = new FileCollector(publishDir.FullName);
                artifact.Version = fileCollector.GetReleaseVersion();
                
                if(!results.Exists(a => a.Version == artifact.Version && a.ProductName == artifact.ProductName))
                    results.Add(artifact);
            }

            return results;
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.ScanArtifactsError, ex, artifactsPath);
        }
    }

    private static string GetProductName(string artifactPath)
    {
        return GetProductName(artifactPath, VinXDirectoryName)
               ?? GetProductName(artifactPath, PerformXDirectoryName);
    }

    private static string GetProductName(string artifactPath, string productName)
    {
        return artifactPath.Contains($".{productName}.") ? productName : null;
    }

    private static bool IsArtifactToPublish(string[] dirParts)
    {
        var publish = dirParts.Last() == PublishDir;
        var releaseName = dirParts[0].ToLower();
        var relToPublish = releaseName is MasterRelease or DevelopRelease or RcRelease;
        return publish && relToPublish;
    }
}