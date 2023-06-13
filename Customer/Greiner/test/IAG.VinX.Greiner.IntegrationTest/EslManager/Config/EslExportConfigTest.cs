using IAG.VinX.Greiner.EslManager.Config;

using Xunit;

namespace IAG.VinX.Greiner.IntegrationTest.EslManager.Config;

public class EslExportConfigTest
{
    [Fact]
    public void GetEslExportConfigTest()
    {
        var config = new EslExportConfig
        {
            ExportRoot = "\\test\\path\for\\eslExportConfig"
        };
        Assert.Equal("\\test\\path\for\\eslExportConfig", config.ExportRoot);
    }
}