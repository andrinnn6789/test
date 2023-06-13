using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.CoreServer.Configuration;
using IAG.Infrastructure.CoreServer.Controller;
using IAG.Infrastructure.CoreServer.Plugin;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Test.CoreServer.Mock;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Newtonsoft.Json.Linq;

using Xunit;

namespace IAG.Infrastructure.Test.CoreServer.Controller;

public class PluginControllerTest
{
    private readonly Mock<IPluginCatalogue> _pluginCatalogue;
    private readonly Mock<IPluginConfigStore> _configStore;
    private readonly Mock<IPluginMetadata> _pluginMeta;
    private IPluginConfig _pluginConfig;

    public PluginControllerTest()
    {
        _pluginMeta = new Mock<IPluginMetadata>();
        _pluginMeta.SetupGet(m => m.PluginId).Returns(PluginInfoAttribute.GetPluginId(typeof(TestPlugin)));
        _pluginMeta.SetupGet(m => m.PluginConfigType).Returns(typeof(TestPluginConfig));

        _pluginConfig = new TestPluginConfig();

        var plugins = new List<IPluginMetadata> { _pluginMeta.Object };

        _pluginCatalogue = new Mock<IPluginCatalogue>();
        _pluginCatalogue.SetupGet(m => m.Plugins).Returns(plugins);
        _pluginCatalogue.Setup(m => m.GetPluginMeta(It.IsAny<Guid>()))
            .Returns<Guid>((id) => { return plugins.FirstOrDefault(p => p.PluginId == id); });

        _configStore = new Mock<IPluginConfigStore>();
        _configStore.Setup(m => m.Get(It.IsAny<Guid>(), It.IsAny<Type>()))
            .Returns(() => _pluginConfig);
        _configStore.Setup(m => m.Write(It.IsAny<IPluginConfig>(), It.IsAny<Type>()))
            .Callback<IPluginConfig, Type>((config, _) => { _pluginConfig = config; });
    }

    [Fact]
    public void GetAllTest()
    {
        var controller = new PluginController(_pluginCatalogue.Object, _configStore.Object);

        var allPlugins = controller.GetAll();

        Assert.NotNull(allPlugins);
        Assert.Equal(
            _pluginMeta.Object,
            Assert.Single(Assert.IsAssignableFrom<IEnumerable<IPluginMetadata>>(allPlugins.Value)));
    }

    [Fact]
    public void ReloadTest()
    {
        int reloads = 0;
        _pluginCatalogue.Setup(m => m.Reload()).Callback(() => reloads++);
        var controller = new PluginController(_pluginCatalogue.Object, _configStore.Object);

        var result = controller.Reload();

        Assert.NotNull(result);
        Assert.IsAssignableFrom<NoContentResult>(result);
        Assert.Equal(1, reloads);
    }

    [Fact]
    public void GetByIdTest()
    {
        var controller = new PluginController(_pluginCatalogue.Object, _configStore.Object);

        var result = controller.GetById(_pluginMeta.Object.PluginId);

        Assert.NotNull(result);
        Assert.Equal(
            _pluginMeta.Object,
            Assert.IsAssignableFrom<IPluginMetadata>(result.Value));
    }

    [Fact]
    public void GetByIdNoResultTest()
    {
        var controller = new PluginController(_pluginCatalogue.Object, _configStore.Object);

        var result = controller.GetById(new Guid());

        Assert.NotNull(result);
        Assert.Null(result.Value);
        Assert.IsAssignableFrom<NotFoundResult>(result.Result);
    }

    [Fact]
    public void GetConfigByIdTest()
    {
        var controller = new PluginController(_pluginCatalogue.Object, _configStore.Object);

        var result = controller.GetConfigById(_pluginMeta.Object.PluginId);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IPluginConfig>(result.Value);
    }

    [Fact]
    public void GetConfigByIdNoResultTest()
    {
        var controller = new PluginController(_pluginCatalogue.Object, _configStore.Object);

        var result = controller.GetConfigById(new Guid());

        Assert.NotNull(result);
        Assert.Null(result.Value);
        Assert.IsAssignableFrom<NotFoundResult>(result.Result);
    }

    [Fact]
    public void SetConfigByIdTest()
    {
        var controller = new PluginController(_pluginCatalogue.Object, _configStore.Object);

        var config = new TestPluginConfig { TestNumber = 42, TestString = "FooBar" };
        var result = controller.SetConfigById(config.Id, JObject.FromObject(config));

        Assert.IsAssignableFrom<NoContentResult>(result);
        var configFromStore = Assert.IsAssignableFrom<TestPluginConfig>(_pluginConfig);
        Assert.Equal(config.TestString, configFromStore.TestString);
        Assert.Equal(config.TestNumber, configFromStore.TestNumber);
    }

    [Fact]
    public void SetConfigByIdBadRequestTest()
    {
        var controller = new PluginController(_pluginCatalogue.Object, _configStore.Object);

        var result = controller.SetConfigById(Guid.NewGuid(), null);

        Assert.IsAssignableFrom<BadRequestResult>(result);
    }

    [Fact]
    public void SetConfigByIdNotFoundTest()
    {
        var controller = new PluginController(_pluginCatalogue.Object, _configStore.Object);

        var result = controller.SetConfigById(Guid.NewGuid(), JObject.FromObject(_pluginConfig));

        Assert.IsAssignableFrom<NotFoundResult>(result);
    }
}