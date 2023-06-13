using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.CoreServer.Configuration;
using IAG.Infrastructure.CoreServer.Plugin;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Test.CoreServer.Mock;

using Moq;

using Xunit;

namespace IAG.Infrastructure.Test.CoreServer.Configuration;

public class PluginCatalogueTest
{
    private readonly Mock<IPluginLoader> _pluginLoader;
    private readonly List<ICoreServerPlugin> _plugins;

    public PluginCatalogueTest()
    {
        _plugins = new List<ICoreServerPlugin>();

        _pluginLoader = new Mock<IPluginLoader>();
        _pluginLoader.Setup(m => m.GetImplementations<ICoreServerPlugin>(It.IsAny<string>(), It.IsAny<bool>())).Returns(_plugins.Select(p => p.GetType()));
    }

    [Fact]
    public void ConstructorTest()
    {
        var catalogue = new PluginCatalogue(_pluginLoader.Object);

        Assert.NotNull(catalogue);
    }

    [Fact]
    public void ReloadTest()
    {
        var catalogue = new PluginCatalogue(_pluginLoader.Object);
        var pluginBeforeReload = catalogue.GetPluginMeta(PluginInfoAttribute.GetPluginId(typeof(TestPlugin)));
        var metaBeforeReload = catalogue.GetPluginMeta(PluginInfoAttribute.GetPluginId(typeof(TestPlugin)));
        var countBeforeReload = catalogue.Plugins.Count;

        var testPlugin = new TestPlugin();
        _plugins.Add(testPlugin);
        catalogue.Reload();
        var pluginAfterReload = catalogue.GetPluginMeta(PluginInfoAttribute.GetPluginId(typeof(TestPlugin)));
        var metaAfterReload = catalogue.GetPluginMeta(PluginInfoAttribute.GetPluginId(typeof(TestPlugin)));

        Assert.Null(pluginBeforeReload);
        Assert.Null(metaBeforeReload);
        Assert.Equal(0, countBeforeReload);
        Assert.NotNull(pluginAfterReload);
        Assert.NotNull(metaAfterReload);
        Assert.Single(catalogue.Plugins);
        Assert.Equal(metaAfterReload, catalogue.Plugins.Single());
        Assert.Equal(typeof(TestPluginConfig), pluginAfterReload.PluginConfigType);
        Assert.Equal(PluginInfoAttribute.GetPluginId(typeof(TestPlugin)), metaAfterReload.PluginId);
        Assert.Equal(typeof(TestPluginConfig), metaAfterReload.PluginConfigType);
    }
}