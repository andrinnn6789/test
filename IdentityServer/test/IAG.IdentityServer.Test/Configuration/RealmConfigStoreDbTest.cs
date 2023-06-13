using System;
using System.Linq;

using FluentAssertions;

using IAG.IdentityServer.Configuration;
using IAG.IdentityServer.Configuration.DataLayer.Settings;
using IAG.IdentityServer.Configuration.Model.Realm;
using IAG.IdentityServer.Test.Configuration.Mock;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Moq;

using Xunit;

namespace IAG.IdentityServer.Test.Configuration;

public class RealmConfigStoreDbTest
{
    private readonly IdentityDataStoreDbContext _context;
    private readonly RealmConfigStoreDb _store;

    public RealmConfigStoreDbTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityDataStoreDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new IdentityDataStoreDbContext(optionsBuilder.Options, new ExplicitUserContext("test", null));

        var configuration = new Mock<IConfigurationRoot>().Object;

        _store = new RealmConfigStoreDb(_context, configuration);
    }

    [Fact]
    public void InsertTest()
    {
        var config = new TestRealmConfig();
        var countBeforeInsert = _store.GetAll().Count();

        _store.Insert(config);

        Assert.Equal(0, countBeforeInsert);
        Assert.Single(_store.GetAll());
    }

    [Fact]
    public void InsertReadTest()
    {
        var config = new TestRealmConfig();
        _store.Insert(config);
        var readConfig = _store.Read(config.Realm, config.AuthenticationPluginConfig.GetType());

        Assert.NotNull(readConfig);
        readConfig.Should().BeEquivalentTo(config);
    }

    [Fact]
    public void LoadTest()
    {
        var config = new TestRealmConfig();
        _store.Insert(config);
        var readConfig = _store.Read(config.Realm, config.AuthenticationPluginConfig.GetType());
        var getConfig = _store.GetAll().FirstOrDefault();

        var loadedReadConfig = _store.LoadConfig(readConfig, config.AuthenticationPluginConfig.GetType());
        var loadedGetConfig = _store.LoadConfig(getConfig, config.AuthenticationPluginConfig.GetType());

        Assert.NotNull(readConfig);
        Assert.NotNull(getConfig);
        Assert.NotNull(loadedReadConfig);
        Assert.NotNull(loadedGetConfig);
        Assert.Equal(readConfig, loadedReadConfig);
        loadedReadConfig.Should().BeEquivalentTo(config);
        loadedGetConfig.Should().BeEquivalentTo(config);
    }

    [Fact]
    public void LoadInterfaceTest()
    {
        var configMock = new Mock<IRealmConfig>();
        configMock.SetupGet(m => m.Realm).Returns(TestRealmConfig.Name);

        var config = new TestRealmConfig();
        _store.Insert(config);

        var loadConfig = _store.LoadConfig(configMock.Object, config.AuthenticationPluginConfig.GetType());

        Assert.NotNull(loadConfig);
        loadConfig.Should().BeEquivalentTo(config);
    }

    [Fact]
    public void ReadFailTest()
    {
        Assert.Throws<NotFoundException>(() => _store.Read("NotAvailable", typeof(string)));
    }

    [Fact]
    public void InsertFailTest()
    {
        var config = new TestRealmConfig();
        _store.Insert(config);

        Assert.Throws<DuplicateKeyException>(() => _store.Insert(config));
    }

    [Fact]
    public void UpdateTest()
    {
        var config = new TestRealmConfig();
        _store.Insert(config);
        _store.Update(config);

        var configAfterUpdate = _store.Read(config.Realm, config.AuthenticationPluginConfig.GetType());

        Assert.NotNull(configAfterUpdate);
        configAfterUpdate.Should().BeEquivalentTo(config);
    }

    [Fact]
    public void UpdateFailTest()
    {
        var config = new TestRealmConfig();

        Assert.Throws<NotFoundException>(() => _store.Update(config));
    }

    [Fact]
    public void DeleteTest()
    {
        var config = new TestRealmConfig();
        _store.Insert(config);
        _store.Delete(config.Realm);

        Assert.Empty(_store.GetAll());
    }

    [Fact]
    public void DeleteFailTest()
    {
        var config = new TestRealmConfig();

        Assert.Throws<NotFoundException>(() => _store.Delete(config.Realm));
    }

    [Fact]
    public void IgnoreNoRealmConfigsTest()
    {
        _context.ConfigEntries.Add(new ConfigDb() {Name = "IdentityServerConfig"});
        _context.SaveChanges();

        var configs = _store.GetAll();

        Assert.Empty(configs);
    }
}