using System;

using IAG.Infrastructure.Configuration.Model;
using IAG.Infrastructure.CoreServer.Configuration;
using IAG.Infrastructure.CoreServer.Configuration.DataLayer;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.Test.CoreServer.Mock;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.Infrastructure.Test.CoreServer.Configuration;

public class ConfigStoreDbTest
{
    private readonly ConfigStoreDb _store;
    private readonly CoreServerDataStoreDbContext _context;

    public ConfigStoreDbTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<CoreServerDataStoreDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new CoreServerDataStoreDbContext(optionsBuilder.Options, new ExplicitUserContext("test", null));
        _store = new ConfigStoreDb(_context, null);
    }

    [Fact]
    public void GetTOnEmptyStoreTest()
    {
        var config = _store.Get<TestPluginConfig>(PluginInfoAttribute.GetPluginId(typeof(TestPlugin)));

        Assert.NotNull(config);
        Assert.Equal(default(string), config.TestString);
        Assert.Equal(default(int), config.TestNumber);
    }

    [Fact]
    public void GetTAfterWriteTTest()
    {
        var config = new TestPluginConfig()
        {
            TestNumber = 42,
            TestString = "FooBar"
        };

        _store.Write<TestPluginConfig>(config);
        var configFromStore = _store.Get<TestPluginConfig>(PluginInfoAttribute.GetPluginId(typeof(TestPlugin)));

        Assert.NotNull(configFromStore);
        Assert.Equal(config.TestString, configFromStore.TestString);
        Assert.Equal(config.TestNumber, configFromStore.TestNumber);
    }

    [Fact]
    public void GetOnEmptyStoreTest()
    {
        var config = _store.Get(PluginInfoAttribute.GetPluginId(typeof(TestPlugin)), typeof(TestPluginConfig));

        Assert.NotNull(config);
        var testConfig = Assert.IsAssignableFrom<TestPluginConfig>(config);
        Assert.Equal(default(string), testConfig.TestString);
        Assert.Equal(default(int), testConfig.TestNumber);
    }

    [Fact]
    public void GetAfterWriteTest()
    {
        var config = new TestPluginConfig() { TestNumber = 42, TestString = "FooBar" };

        _store.Write(config, typeof(TestPluginConfig));
        var configFromStore = _store.Get(PluginInfoAttribute.GetPluginId(typeof(TestPlugin)), typeof(TestPluginConfig));

        Assert.NotNull(configFromStore);
        var testConfig = Assert.IsAssignableFrom<TestPluginConfig>(configFromStore);
        Assert.Equal(config.TestString, testConfig.TestString);
        Assert.Equal(config.TestNumber, testConfig.TestNumber);
    }

    [Fact]
    public void WriteTTest()
    {
        var config = new TestPluginConfig() { TestNumber = 42, TestString = "FooBar" };

        _store.Write<TestPluginConfig>(new TestPluginConfig());
        var configFirstWrite = _store.Get<TestPluginConfig>(PluginInfoAttribute.GetPluginId(typeof(TestPlugin)));

        _store.Write<TestPluginConfig>(config);
        var configSecondWrite = _store.Get<TestPluginConfig>(PluginInfoAttribute.GetPluginId(typeof(TestPlugin)));

        Assert.NotNull(configFirstWrite);
        Assert.Equal(default(string), configFirstWrite.TestString);
        Assert.Equal(default(int), configFirstWrite.TestNumber);

        Assert.NotNull(configSecondWrite);
        Assert.Equal(config.TestString, configSecondWrite.TestString);
        Assert.Equal(config.TestNumber, configSecondWrite.TestNumber);
    }

    [Fact]
    public void WriteTest()
    {
        var config = new TestPluginConfig() { TestNumber = 42, TestString = "FooBar" };

        _store.Write(new TestPluginConfig(), typeof(TestPluginConfig));
        var configFirstWrite = _store.Get(PluginInfoAttribute.GetPluginId(typeof(TestPlugin)), typeof(TestPluginConfig));

        _store.Write(config, typeof(TestPluginConfig));
        var configSecondWrite = _store.Get(PluginInfoAttribute.GetPluginId(typeof(TestPlugin)), typeof(TestPluginConfig));

        Assert.NotNull(configFirstWrite);
        var testConfig = Assert.IsAssignableFrom<TestPluginConfig>(configFirstWrite);
        Assert.Equal(default(string), testConfig.TestString);
        Assert.Equal(default(int), testConfig.TestNumber);

        Assert.NotNull(configSecondWrite);
        testConfig = Assert.IsAssignableFrom<TestPluginConfig>(configSecondWrite);
        Assert.Equal(config.TestString, testConfig.TestString);
        Assert.Equal(config.TestNumber, testConfig.TestNumber);
    }

    [Fact]
    public void DeleteTest()
    {
        var config = new TestPluginConfig() { TestNumber = 42, TestString = "FooBar" };

        _store.Write<TestPluginConfig>(config);
        var configAfterWrite = _store.Read<TestPluginConfig>(config.Id);

        _store.Delete(config);

        Assert.NotNull(configAfterWrite);
        Assert.Equal(config.TestString, configAfterWrite.TestString);
        Assert.Equal(config.TestNumber, configAfterWrite.TestNumber);

        Assert.Equal(config.Id.ToString(), Assert.Throws<NotFoundException>(() => _store.Read<TestPluginConfig>(config.Id)).Message);
    }

    [Fact]
    public void DeleteNotExceptionFoundTest()
    {
        var config = new TestPluginConfig();
        Assert.Equal(config.Id.ToString(), Assert.Throws<NotFoundException>(() => _store.Delete(config)).Message);
    }

    [Fact]
    public void InsertTDuplicateKeyExceptionTest()
    {
        var config = new TestPluginConfig();

        _store.Insert<TestPluginConfig>(config);

        Assert.Equal(config.Id.ToString(), Assert.Throws<DuplicateKeyException>(() => _store.Insert<TestPluginConfig>(config)).Message);
    }

    [Fact]
    public void InsertDuplicateKeyExceptionTest()
    {
        var config = new TestPluginConfig();

        _store.Insert(config, typeof(TestPluginConfig));

        Assert.Equal(config.Id.ToString(), Assert.Throws<DuplicateKeyException>(() => _store.Insert(config, typeof(TestPluginConfig))).Message);
    }

    [Fact]
    public void InsertArgumentExceptionTest()
    {
        Assert.Throws<ArgumentException>(() => _store.Insert(new TestPluginConfig(), typeof(string)));
    }

    [Fact]
    public void UpdateArgumentExceptionTest()
    {
        Assert.Throws<ArgumentException>(() => _store.Update(new TestPluginConfig(), typeof(string)));
    }

    [Fact]
    public void GetArgumentExceptionTest()
    {
        Assert.Throws<ArgumentException>(() => _store.Get(new Guid(), typeof(string)));
        Assert.Throws<ArgumentException>(() => _store.Get(new Guid(), typeof(TestPluginConfig)));
    }

    [Fact]
    public void ConfigReplacementTest()
    {
        var config = new TestPluginConfig() { TestString = "Test $$Text$" };

        _store.Write(config, typeof(TestPluginConfig));
        _context.ConfigCommonEntries.Add(new ConfigCommon() { Name = "Text", Data = "42" });
        _context.SaveChanges();

        var configFromStore = _store.Get(PluginInfoAttribute.GetPluginId(typeof(TestPlugin)), typeof(TestPluginConfig));

        Assert.NotNull(configFromStore);
        var testConfig = Assert.IsAssignableFrom<TestPluginConfig>(configFromStore);
        Assert.Equal("Test 42", testConfig.TestString);
    }
}