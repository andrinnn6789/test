using System;
using System.Collections.Generic;
using System.Linq;

using IAG.IdentityServer.Configuration;
using IAG.IdentityServer.Configuration.Model.Realm;
using IAG.IdentityServer.Test.Configuration.Mock;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.TestHelper.xUnit;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace IAG.IdentityServer.Test.Configuration;

public class RealmCatalogueTest
{
    private Mock<IRealmConfigStore> _configStore;
    private Mock<IPluginMetadata> _pluginMeta;
    private Mock<IPluginCatalogue> _pluginCatalogue;
    private Mock<IRealmConfig> _realmConfig;
    private ILogger<RealmCatalogue> _logger;
    private List<IRealmConfig> _configs;
    private List<IPluginMetadata> _plugins;

    public RealmCatalogueTest()
    {
        InitMocks();
    }

    [Fact]
    public void ConstructorTest()
    {
        var catalogue = new RealmCatalogue(_configStore.Object, _pluginCatalogue.Object, _logger);

        Assert.NotNull(catalogue);
    }

    [Fact]
    public void ReLoadTest()
    {
        var catalogue = new RealmCatalogue(_configStore.Object, _pluginCatalogue.Object, _logger);
        var realmNotFound = catalogue.GetRealm(_realmConfig.Object.Realm);

        _configs.Add(_realmConfig.Object);
        _plugins.Add(_pluginMeta.Object);

        catalogue.Reload();
        var realmFound = catalogue.GetRealm(_realmConfig.Object.Realm);

        Assert.Null(realmNotFound);
        Assert.NotNull(realmFound);
    }

    [Fact]
    public void ReLoadFailedTest()
    {
        var catalogue = new RealmCatalogue(_configStore.Object, _pluginCatalogue.Object, _logger);

        _configs.Add(_realmConfig.Object);

        catalogue.Reload();
        var realmFound = catalogue.GetRealm(_realmConfig.Object.Realm) as FaultyRealmConfig;

        Assert.NotNull(realmFound);
        Assert.Null(realmFound.AuthenticationPluginConfig);
        Assert.NotEmpty(realmFound.ErrorMessage);
    }

    [Fact]
    public void SaveTest()
    {
        var catalogue = new RealmCatalogue(_configStore.Object, _pluginCatalogue.Object, _logger);

        catalogue.Save(_realmConfig.Object);
        var realmAfterFirstSave = catalogue.GetRealm(_realmConfig.Object.Realm);

        catalogue.Save(_realmConfig.Object);
        var realmAfterSecondSave = catalogue.GetRealm(_realmConfig.Object.Realm);

        Assert.NotNull(realmAfterFirstSave);
        Assert.NotNull(realmAfterSecondSave);
        Assert.Equal(realmAfterFirstSave, realmAfterSecondSave);
        Assert.Single(catalogue.Realms);
        Assert.Equal(realmAfterFirstSave, catalogue.Realms.Single());
        Assert.Single(_configs);
    }

    [Fact]
    public void DeleteTest()
    {
        var catalogue = new RealmCatalogue(_configStore.Object, _pluginCatalogue.Object, _logger);

        catalogue.Delete(_realmConfig.Object.Realm);

        var realmAfterDelete = catalogue.GetRealm(_realmConfig.Object.Realm);

        Assert.Null(realmAfterDelete);
    }

    private void InitMocks()
    {
        _configs = new List<IRealmConfig>();
        _configStore = new Mock<IRealmConfigStore>();
        _configStore.Setup(m => m.GetAll()).Returns(_configs);
        _configStore.Setup(m => m.LoadConfig(It.IsAny<IRealmConfig>(), It.IsAny<Type>()))
            .Returns<IRealmConfig, Type>((config, _) => config);
        _configStore.Setup(m => m.Update(It.IsAny<IRealmConfig>())).Callback<IRealmConfig>(
            (config) =>
            {
                if (!_configs.Exists(c => c.Realm == config.Realm))
                    throw new NotFoundException(config.Realm);
            });
        _configStore.Setup(m => m.Insert(It.IsAny<IRealmConfig>()))
            .Callback<IRealmConfig>((config) => _configs.Add(config));

        _pluginMeta = new Mock<IPluginMetadata>();
        _pluginMeta.SetupGet(m => m.PluginId).Returns(TestPlugin.Id);

        _plugins = new List<IPluginMetadata>();
        _pluginCatalogue = new Mock<IPluginCatalogue>();
        _pluginCatalogue.Setup(m => m.GetPluginMeta(It.IsAny<Guid>()))
            .Returns<Guid>(
                (id) => { return _plugins.FirstOrDefault(p => p.PluginId == id); });

        _realmConfig = new Mock<IRealmConfig>();
        _realmConfig.Setup(m => m.Realm).Returns("TestRealm");
        _realmConfig.SetupGet(m => m.AuthenticationPluginId).Returns(TestPlugin.Id);

        _logger = new MockILogger<RealmCatalogue>();
    }
}