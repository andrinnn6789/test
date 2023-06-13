using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Exception;
using IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

using Xunit;

namespace IAG.VinX.IAG.Test.ControlCenter.Distribution.BusinessLogic;

public class ArtifactsScannerTest
{
    [Fact]
    public void OverallArtifactsScannerTest()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "ArtifactsScannerTest");
        List<ArtifactInfo> artifacts;
        try
        {
            var publishDirectories = new []
            {
                Path.Combine(testDir, "develop", "PerformX", "src", "IAG.PerformX.HostWindows", "bin", "Release", "netcoreapp3.1", "win-x64", "publish"),
                Path.Combine(testDir, "master", "23.01.10.1234", "VinX", "src", "IAG.VinX.HostWindows", "bin", "Release", "netcoreapp3.1", "win-x64", "publish"),
                Path.Combine(testDir, "master", "23.01.10.1234", "Installer", "src", "IAG.InstallClient.HostWindows", "bin", "Release", "netcoreapp3.1", "win-x64", "publish"),
                Path.Combine(testDir, "master", "23.01.10.1234", "Base", "ControlCenter", "src", "IAG.ControlCenter.HostWindows", "bin", "Release", "netcoreapp3.1", "win-x64", "publish"),
                Path.Combine(testDir, "master", "23.01.10.1234", "Customer", "AeCS", "src", "IAG.PerformX.AeCS", "bin", "Release", "netcoreapp3.1", "publish"),
                Path.Combine(testDir, "release-candidates", "23.01.10.1234", "VinX", "src", "IAG.VinX.HostWindows", "bin", "Release", "netcoreapp3.1", "win-x64", "publish")
            };

            var mainFilePath = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location) ?? String.Empty, "IAG.Infrastructure.dll");
            foreach (var directory in publishDirectories)
            {
                Directory.CreateDirectory(directory);
                File.Copy(mainFilePath, Path.Combine(directory, "IAG.Infrastructure.dll"));
            }

            Directory.CreateDirectory(Path.Combine(publishDirectories[0], "Settings"));
            Directory.CreateDirectory(Path.Combine(testDir, "master", "23.01.10.1234", "VinX", "src", "IAG.VinX.HostWindows", "Setup", "VinX BPE Prod-SetupFiles"));
            Directory.CreateDirectory(Path.Combine(testDir, "fakePublish"));

            var scanner = new ArtifactsScanner();
            artifacts = scanner.Scan(testDir).ToList();
        }
        finally
        {
            Directory.Delete(testDir, true);
        }

        Assert.NotNull(artifacts);
        Assert.Equal(4, artifacts.Count);
        Assert.All(artifacts, a => Assert.EndsWith("publish", a.ArtifactPath));
        Assert.Equal(ProductType.IagService, Assert.Single(artifacts, a => a.ProductName == "PerformX").ProductType);
        Assert.Equal(ProductType.IagService, Assert.Single(artifacts, a => a.ProductName == "VinX").ProductType);
        Assert.Equal(ProductType.CustomerExtension, Assert.Single(artifacts, a => a.ProductName == "AeCS").ProductType);
        Assert.Equal(ProductType.Updater, Assert.Single(artifacts, a => a.ProductName == "Installer").ProductType);
    }

    [Fact]
    public void ArtifactsScannerErrorTest()
    {
        Assert.Throws<LocalizableException>(() => new ArtifactsScanner().Scan("FantasyDirectory"));
    }
}