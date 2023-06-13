using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.IdentityServer.Configuration;
using IAG.IdentityServer.Configuration.Model.Realm;
using IAG.IdentityServer.SeedImportExport;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;
using IAG.Infrastructure.IdentityServer.Plugin;

using Moq;

using Newtonsoft.Json.Linq;

using Xunit;

namespace IAG.IdentityServer.Test.SeedImporterExporter;

public class RealmSeedImporterExporterTest
{
    private static readonly string TestRealmId = "TestRealm";
    private static readonly Guid TestPluginId = Guid.NewGuid();


    private readonly RealmSeedImporterExporter _realmSeedImporterExporter;
    private List<IRealmConfig> _realmConfigs = new();
    private JObject _importedPluginData;


    public RealmSeedImporterExporterTest()
    {
        var pluginMetadataMock = new Mock<IPluginMetadata>();
        pluginMetadataMock.Setup(m => m.PluginId).Returns(TestPluginId);
        pluginMetadataMock.Setup(m => m.PluginConfigType).Returns(typeof(TestPluginConfig));

        var realmCatalogueMock = new Mock<IRealmCatalogue>();
        realmCatalogueMock.Setup(m => m.Realms).Returns(_realmConfigs);
        realmCatalogueMock.Setup(m => m.GetRealm(It.IsAny<string>())).Returns<string>((id) => _realmConfigs.FirstOrDefault(r => r.Realm == id));
        realmCatalogueMock.Setup(m => m.Save(It.IsAny<IRealmConfig>())).Callback<IRealmConfig>((config) => _realmConfigs.Add(config));

        var authenticationPluginMock = new Mock<IAuthenticationPlugin>();
        authenticationPluginMock.Setup(m => m.Config).Returns(new TestPluginConfig());
        authenticationPluginMock.Setup(m => m.ImportData(It.IsAny<JObject>(), It.IsAny<IUserContext>())).Callback<JObject, IUserContext>((data,_) => _importedPluginData = data);

        var pluginCatalogueMock = new Mock<IPluginCatalogue>();
        pluginCatalogueMock.Setup(m => m.GetAuthenticationPlugin(It.IsAny<Guid>())).Returns<Guid>((id) => id == TestPluginId ? authenticationPluginMock.Object : null);
        pluginCatalogueMock.Setup(m => m.GetPluginMeta(It.IsAny<Guid>())).Returns<Guid>((id) => id == TestPluginId ? pluginMetadataMock.Object : null);

        var claimCollectorMock = new Mock<IClaimCollector>();
        claimCollectorMock.Setup(m => m.CollectAndUpdateAsync()).Returns(Task.CompletedTask);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IRealmCatalogue)))).Returns(realmCatalogueMock.Object);
        serviceProviderMock.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IPluginCatalogue)))).Returns(pluginCatalogueMock.Object);
        serviceProviderMock.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IClaimCollector)))).Returns(claimCollectorMock.Object);
        serviceProviderMock.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IUserContext)))).Returns(new ExplicitUserContext("Test", null));

        _realmSeedImporterExporter = new RealmSeedImporterExporter(serviceProviderMock.Object);
    }


    [Fact]
    public void SeedFilePatternTest()
    {
        Assert.NotNull(_realmSeedImporterExporter.SeedFilePattern);
        Assert.NotEmpty(_realmSeedImporterExporter.SeedFilePattern);
        Assert.Contains('*', _realmSeedImporterExporter.SeedFilePattern);
    }

    [Fact]
    public void ExportTest()
    {
        _realmConfigs.Add(new RealmConfig()
        {
            Realm = TestRealmId,
            AuthenticationPluginId = TestPluginId,
        });

        var export = _realmSeedImporterExporter.Export(TestRealmId, out string fileName);

        Assert.NotNull(export);
        Assert.NotNull(fileName);
        Assert.NotEmpty(fileName);
        Assert.Contains(TestRealmId, fileName);
    }

    [Fact]
    public void ExportRealmNotFoundTest()
    {
        Assert.Throws<NotFoundException>(() => _realmSeedImporterExporter.Export("UnknownRealm", out string _));
        Assert.Throws<NotFoundException>(() => _realmSeedImporterExporter.Export(TestRealmId, out string _));
    }

    [Fact]
    public void ExportPluginNotFoundTest()
    {
        var realmConfig = new RealmConfig()
        {
            Realm = TestRealmId,
            AuthenticationPluginId = Guid.NewGuid()
        };
        _realmConfigs.Add(realmConfig);

        Assert.Throws<NotFoundException>(() => _realmSeedImporterExporter.Export(TestRealmId, out string _));
    }

    [Fact]
    public void ImportTest()
    {
        var pluginConfig = new TestPluginConfig();
        var realmImport = new RealmImportExport()
        {
            RealmConfig = new RealmConfig()
            {
                AuthenticationPluginId = TestPluginId,
                AuthenticationPluginConfig = pluginConfig,
                AuthenticationPluginConfigJObject = JObject.FromObject(pluginConfig)
            },
            AuthenticationPluginData = JObject.FromObject(new TestPluginData())
        };

        _realmSeedImporterExporter.Import(JObject.FromObject(realmImport));

        Assert.NotEmpty(_realmConfigs);
        Assert.Equal(
            TestPluginConfig.TestValue, 
            Assert.IsType<TestPluginConfig>(Assert.Single(_realmConfigs)?.AuthenticationPluginConfig).TestSetting
        );
        Assert.NotNull(_importedPluginData);
        Assert.Equal(42, _importedPluginData.ToObject<TestPluginData>()?.TestNumber);
    }

    [Fact]
    public void ImportDataCheckFailsTest()
    {
        var realmImport = new 
        {
            Type = "SomeRandomType",
            RealmConfig = new RealmConfig() { AuthenticationPluginId = Guid.NewGuid() }
        };

        Assert.Throws<LocalizableException>(() => _realmSeedImporterExporter.Import(JObject.FromObject(realmImport)));
    }

    [Fact]
    public void ImportPluginNotFoundTest1()
    {
        var realmImport = new RealmImportExport()
        {
            RealmConfig = new RealmConfig() { AuthenticationPluginId = Guid.NewGuid() }
        };

        Assert.Throws<NotFoundException>(() => _realmSeedImporterExporter.Import(JObject.FromObject(realmImport)));
    }

    [Fact]
    public void ImportPluginNotFoundTest2()
    {
        var realmConfig = new RealmConfig()
        {
            Realm = TestRealmId,
            AuthenticationPluginId = Guid.NewGuid()
        };
        var realmImport = new RealmImportExport()
        {
            RealmConfig = realmConfig
        };

        _realmConfigs.Add(realmConfig);
            
        Assert.Throws<LocalizableException>(() => _realmSeedImporterExporter.Import(JObject.FromObject(realmImport)));
    }


    private class TestPluginConfig : IAuthenticationPluginConfig
    {
        public static readonly string TestValue = "JustATest";

        public TimeSpan? ValidityDuration => new TimeSpan(0, 42, 23);
        public bool PublishClaims => false;
        public bool Active { get; set; }

        public string TestSetting => TestValue;
    }

    private class TestPluginData
    {
        public int TestNumber { get; } = 42;
    }
}