using System.Collections.Generic;
using System.IO;
using System.Linq;

using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Exception;
using IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

using Xunit;

namespace IAG.VinX.IAG.Test.ControlCenter.Distribution.BusinessLogic;

public class SettingsScannerTest
{
    [Fact]
    public void OverallSettingsScannerTest()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "SettingsScannerTest");
        List<ArtifactInfo> artifacts;
        try
        {
            Directory.CreateDirectory(Path.Combine(testDir, "PerformX", "Prod"));
            Directory.CreateDirectory(Path.Combine(testDir, "PerformX", "Test"));
            Directory.CreateDirectory(Path.Combine(testDir, "VinX", "Test"));

            var scanner = new SettingsScanner();
            artifacts = scanner.Scan(testDir).ToList();
        }
        finally
        {
            Directory.Delete(testDir, true);
        }

        Assert.NotNull(artifacts);
        Assert.Equal(3, artifacts.Count);
        Assert.All(artifacts, a => Assert.Equal(ProductType.ConfigTemplate, a.ProductType));
        Assert.All(artifacts, a => Assert.NotNull(a.DependingProductName));
        Assert.All(artifacts, a => Assert.Contains(a.DependingProductName, a.ProductName));
        Assert.Single(artifacts, a => a.DependingProductName == "PerformX" && a.ArtifactName == "Test");
        Assert.Single(artifacts, a => a.DependingProductName == "PerformX" && a.ArtifactName == "Prod");
        Assert.Single(artifacts, a => a.DependingProductName == "VinX" && a.ArtifactName == "Test");
    }

    [Fact]
    public void SettingsScannerErrorTest()
    {
        Assert.Throws<LocalizableException>(() => new SettingsScanner().Scan("FantasyDirectory"));
    }
}