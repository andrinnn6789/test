using IAG.Infrastructure.DI;

using Xunit;

namespace IAG.Infrastructure.Test.DI;

public class ActivatorWithCheckTest
{
    [Fact]
    public void CreateInstanceTestFail()
    {
        Assert.Throws<System.Exception>(() => ActivatorWithCheck.CreateInstance<string>(typeof(int?)));
    }

    [Fact]
    public void CreateInstanceTestOk()
    {
        var instance = ActivatorWithCheck.CreateInstance<PluginMetadata>(typeof(PluginMetadata));
        Assert.NotNull(instance);
    }
}