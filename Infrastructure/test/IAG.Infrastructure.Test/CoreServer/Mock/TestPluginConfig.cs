using IAG.Infrastructure.CoreServer.Plugin;

namespace IAG.Infrastructure.Test.CoreServer.Mock;

public class TestPluginConfig : PluginConfig<TestPlugin>
{
    public string TestString { get; set; }

    public int TestNumber { get; set; }
}