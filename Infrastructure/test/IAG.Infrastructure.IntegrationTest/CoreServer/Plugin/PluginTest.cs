using System.Net;
using System.Threading.Tasks;

using IAG.Infrastructure.CoreServer.Configuration;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.IntegrationTest.CoreServer.Plugin.TestPlugin;
using IAG.Infrastructure.TestHelper.Startup;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.CoreServer.Plugin;

[Collection("InfrastructureController")]
public class PluginWithControllerTest
{
    private readonly TestServerEnvironment _testEnvironment;
    private const string Uri = "/Test/TestPluginWithController/Test";

    public PluginWithControllerTest(TestServerEnvironment testServerEnvironment)
    {
        _testEnvironment = testServerEnvironment;
    }

    [Fact]
    public async Task CoreControllerPluginWithControllerTest()
    {
        var client = _testEnvironment.NewClient();
        var response = await client.GetAsync(Uri);
        var activatedCode = response.StatusCode;
        var content = await response.Content.ReadAsStringAsync();

        var pluginConfigStore = _testEnvironment.GetServices().GetRequiredService<IPluginConfigStore>();
        var pluginConfig =
            pluginConfigStore.Get<TestPluginWithControllerConfig>(
                PluginInfoAttribute.GetPluginId(typeof(TestPluginWithController)));
        pluginConfig.Active = false;
        pluginConfigStore.Write<TestPluginWithControllerConfig>(pluginConfig);

        client = _testEnvironment.NewClient();
        response = await client.GetAsync(Uri);
        var notActivatedCode = response.StatusCode;

        Assert.Equal(HttpStatusCode.NotFound, notActivatedCode);
        Assert.Equal(HttpStatusCode.OK, activatedCode);
        Assert.Equal(1, TestPluginWithController.InitCalledCounter);
        Assert.NotEmpty(content);
    }
}