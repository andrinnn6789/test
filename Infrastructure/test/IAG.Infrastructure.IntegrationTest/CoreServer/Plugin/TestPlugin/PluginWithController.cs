using IAG.Infrastructure.CoreServer.ActionFilter;
using IAG.Infrastructure.CoreServer.Plugin;
using IAG.Infrastructure.DI;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace IAG.Infrastructure.IntegrationTest.CoreServer.Plugin.TestPlugin;

public class TestPluginWithControllerConfig : PluginConfig<TestPluginWithController>
{
}

[PluginInfo("FC5EEB09-5BE1-42C1-B0B0-B1B9903C4AEE", "Test plugin with controller")]
public class TestPluginWithController : CoreServerPlugin<TestPluginWithControllerConfig>
{
    public static int InitCalledCounter { get; set; }

    public override void Init(IServiceCollection services)
    {
        InitCalledCounter++;

        base.Init(services);
    }
}

[Route("Test/TestPluginWithController")]
[HideDisabledPluginActionFilter(typeof(TestPluginWithControllerConfig))]
public class TestPluginWithControllerController : ControllerBase
{
    [HttpGet("Test")]
#pragma warning disable CA1822 // Mark members as static
    public string Test()
#pragma warning restore CA1822 // Mark members as static
    {
        return "Ok";
    }
}