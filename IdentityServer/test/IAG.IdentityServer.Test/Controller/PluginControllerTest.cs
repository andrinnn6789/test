using System;
using System.Collections.Generic;
using System.Linq;

using IAG.IdentityServer.Configuration;
using IAG.IdentityServer.Controller;
using IAG.IdentityServer.Test.Configuration.Mock;
using IAG.Infrastructure.DI;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace IAG.IdentityServer.Test.Controller;

public class PluginControllerTest
{
    private readonly Mock<IPluginCatalogue> _pluginCatalogue;
    private readonly Mock<IPluginMetadata> _pluginMeta;

    public PluginControllerTest()
    {
        _pluginMeta = new Mock<IPluginMetadata>();
        _pluginMeta.SetupGet(m => m.PluginId).Returns(TestPlugin.Id);
        _pluginMeta.SetupGet(m => m.PluginConfigType).Returns(typeof(TestPluginConfig));

        var plugins = new List<IPluginMetadata> { _pluginMeta.Object };

        _pluginCatalogue = new Mock<IPluginCatalogue>();
        _pluginCatalogue.SetupGet(m => m.Plugins).Returns(plugins);
        _pluginCatalogue.Setup(m => m.GetPluginMeta(It.IsAny<Guid>()))
            .Returns<Guid>((id) => { return plugins.FirstOrDefault(p => p.PluginId == id); });
    }

    [Fact]
    public void GetAllTest()
    {
        var controller = new PluginController(_pluginCatalogue.Object);

        var allPlugins = controller.GetAll();
        var authenticationPlugins = controller.GetAll();

        Assert.NotNull(allPlugins);
        Assert.NotNull(authenticationPlugins);
        Assert.Equal(
            _pluginMeta.Object,
            Assert.Single(Assert.IsAssignableFrom<IEnumerable<IPluginMetadata>>(allPlugins.Value)));
        Assert.Equal(
            _pluginMeta.Object,
            Assert.Single(Assert.IsAssignableFrom<IEnumerable<IPluginMetadata>>(authenticationPlugins.Value)));
    }

    [Fact]
    public void ReloadTest()
    {
        int reloads = 0;
        _pluginCatalogue.Setup(m => m.Reload()).Callback(() => reloads++);
        var controller = new PluginController(_pluginCatalogue.Object);

        var result = controller.Reload();

        Assert.NotNull(result);
        Assert.IsAssignableFrom<NoContentResult>(result);
        Assert.Equal(1, reloads);
    }

    [Fact]
    public void GetByIdTest()
    {
        var controller = new PluginController(_pluginCatalogue.Object);

        var result = controller.GetById(_pluginMeta.Object.PluginId);

        Assert.NotNull(result);
        Assert.Equal(_pluginMeta.Object, Assert.IsAssignableFrom<IPluginMetadata>(result.Value));
    }

    [Fact]
    public void GetByIdNoResultTest()
    {
        var controller = new PluginController(_pluginCatalogue.Object);

        var result = controller.GetById(Guid.NewGuid());

        Assert.NotNull(result);
        Assert.Null(result.Value);
        Assert.IsAssignableFrom<NotFoundResult>(result.Result);
    }

    [Fact]
    public void GetConfigByIdTest()
    {
        var controller = new PluginController(_pluginCatalogue.Object);

        var result = controller.GetConfigById(_pluginMeta.Object.PluginId);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<TestPluginConfig>(result.Value);
    }

    [Fact]
    public void GetConfigByIdNoResultTest()
    {
        var controller = new PluginController(_pluginCatalogue.Object);

        var result = controller.GetConfigById(Guid.NewGuid());

        Assert.NotNull(result);
        Assert.Null(result.Value);
        Assert.IsAssignableFrom<NotFoundResult>(result.Result);
    }
}