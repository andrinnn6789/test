using System;
using System.Collections.Generic;
using System.Linq;

using IAG.IdentityServer.Configuration;
using IAG.IdentityServer.Test.Configuration.Mock;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.IdentityServer.Plugin;

using Microsoft.Extensions.Hosting;

using Moq;

using Xunit;

namespace IAG.IdentityServer.Test.Configuration;

public class PluginCatalogueTest
{
    private readonly Mock<IPluginLoader> _pluginLoaderMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly List<IAuthenticationPlugin> _plugins;

    public PluginCatalogueTest()
    {
        _plugins = new List<IAuthenticationPlugin>();
        _pluginLoaderMock = new Mock<IPluginLoader>();

            
        _serviceProviderMock = new Mock<IServiceProvider>();
        _serviceProviderMock.Setup(m => m.GetService(It.IsAny<Type>())).Returns<Type>(t =>_plugins.FirstOrDefault(p => p.GetType() == t));

        _pluginLoaderMock.Setup(m => m.GetImplementations<IAuthenticationPlugin>(It.IsAny<string>(), It.IsAny<bool>())).Returns(_plugins.Select(p => p.GetType()));
    }

    [Fact]
    public void ConstructorTest()
    {
        var catalogue = new PluginCatalogue(_pluginLoaderMock.Object, _serviceProviderMock.Object);

        Assert.NotNull(catalogue);
    }

    [Fact]
    public void ReloadTest()
    {
        var catalogue = new PluginCatalogue(_pluginLoaderMock.Object, _serviceProviderMock.Object);
        var pluginBeforeReload = catalogue.GetAuthenticationPlugin(TestPlugin.Id);
        var meatBeforeReload = catalogue.GetPluginMeta(TestPlugin.Id);
        var countBeforeReload = catalogue.Plugins.Count;

        var testPlugin = new TestPlugin(new Mock<IHostEnvironment>().Object);
        _plugins.Add(testPlugin);
        catalogue.Reload();
        var pluginAfterReload = catalogue.GetAuthenticationPlugin(TestPlugin.Id);
        var metaAfterReload = catalogue.GetPluginMeta(TestPlugin.Id);

        Assert.Null(pluginBeforeReload);
        Assert.Null(meatBeforeReload);
        Assert.Equal(0, countBeforeReload);
        Assert.NotNull(pluginAfterReload);
        Assert.NotNull(metaAfterReload);
        Assert.Single(catalogue.Plugins);
        Assert.Equal(metaAfterReload, catalogue.Plugins.Single());
        Assert.Equal(typeof(TestPluginConfig), pluginAfterReload.Config.GetType());
        Assert.Equal(TestPlugin.Id, metaAfterReload.PluginId);
        Assert.Equal(TestPlugin.Name, metaAfterReload.PluginName);
        Assert.Equal(typeof(TestPlugin), metaAfterReload.PluginType);
        Assert.Equal(typeof(TestPluginConfig), metaAfterReload.PluginConfigType);
    }
}