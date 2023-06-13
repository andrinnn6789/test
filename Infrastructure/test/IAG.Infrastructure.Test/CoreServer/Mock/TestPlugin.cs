using IAG.Infrastructure.CoreServer.Plugin;
using IAG.Infrastructure.DI;

namespace IAG.Infrastructure.Test.CoreServer.Mock;

[PluginInfo("6D254369-252B-404B-8779-79AF7F708A22", "IAG.CoreServer.TestPlugin")]
public class TestPlugin : CoreServerPlugin<TestPluginConfig>
{
}