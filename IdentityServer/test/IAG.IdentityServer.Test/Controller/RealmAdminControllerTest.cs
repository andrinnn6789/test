using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using IAG.IdentityServer.Configuration;
using IAG.IdentityServer.Configuration.Model.Realm;
using IAG.IdentityServer.Controller;
using IAG.IdentityServer.SeedImportExport;
using IAG.IdentityServer.Test.Configuration.Mock;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.IdentityServer.Plugin;
using IAG.Infrastructure.Rest;
using IAG.Infrastructure.TestHelper.Globalization.Mocks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Moq;

using Newtonsoft.Json.Linq;

using Xunit;

namespace IAG.IdentityServer.Test.Controller;

public class RealmAdminControllerTest
{
    private string _password;
    private string _email;
    private Mock<IPluginCatalogue> _pluginCatalogue;
    private Mock<IRealmCatalogue> _realmCatalogue;
    private MockLocalizer<RealmAdminController> _localizer;
    private IRealmConfig _realmConfig;
    private List<IRealmConfig> _realms;
    private List<IPluginMetadata> _plugins;

    public RealmAdminControllerTest()
    {
        InitMocks();
    }

    [Fact]
    public void GetAllTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);

        var allRealms = controller.GetAll();

        Assert.NotNull(allRealms);
        Assert.Equal(
            _realmConfig,
            Assert.Single(Assert.IsAssignableFrom<IEnumerable<IRealmConfig>>(allRealms.Value)));
    }

    [Fact]
    public void ReloadTest()
    {
        int reloads = 0;
        _realmCatalogue.Setup(m => m.Reload()).Callback(() => reloads++);
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);

        var result = controller.Reload();

        Assert.NotNull(result);
        Assert.IsAssignableFrom<NoContentResult>(result);
        Assert.Equal(1, reloads);
    }

    [Fact]
    public void GetByIdTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);

        var result = controller.GetById(_realmConfig.Realm);

        Assert.NotNull(result);
        Assert.Equal(
            _realmConfig,
            Assert.IsAssignableFrom<IRealmConfig>(result.Value));
    }

    [Fact]
    public void GetByIdNoResultTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);

        var result = controller.GetById($"Not_{_realmConfig.Realm}");

        Assert.NotNull(result);
        Assert.Null(result.Value);
        Assert.IsAssignableFrom<NotFoundResult>(result.Result);
    }

    [Fact]
    public void InsertTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);
        var newRealmConfig = new RealmConfig()
        {
            Realm = "NewRealm",
            AuthenticationPluginId = TestPlugin.Id,
            AuthenticationPluginConfigJObject = JObject.FromObject(new TestPluginConfig() { ValidityDuration = new TimeSpan(0, 0, 42, 0) }),
            PasswordPolicy = new PasswordOptions()
        };
        var result = controller.Insert(newRealmConfig);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<NoContentResult>(result);
        Assert.Equal(2, _realms.Count);
        Assert.Equal(42, Assert.Single(_realms, r => r.Realm == "NewRealm")?.AuthenticationPluginConfig?.ValidityDuration?.Minutes);
    }

    [Fact]
    public void InsertWithUnknownPluginTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);
        var newRealmConfig = new RealmConfig()
        {
            Realm = "NewRealm",
            AuthenticationPluginId = Guid.Empty
        };
        var result = controller.Insert(newRealmConfig);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<NotFoundObjectResult>(result);
    }

    [Fact]
    public void InsertBadRequestsTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);
        var emptyRealmConfig = new RealmConfig() { Realm = string.Empty };

        var resultNullRealm = controller.Insert(null);
        var resultEmptyRealm = controller.Insert(emptyRealmConfig);

        Assert.NotNull(resultNullRealm);
        Assert.NotNull(resultEmptyRealm);
        Assert.IsAssignableFrom<BadRequestObjectResult>(resultNullRealm);
        Assert.IsAssignableFrom<BadRequestObjectResult>(resultEmptyRealm);
    }

    [Fact]
    public void UpdateTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);
        var newRealmConfig = new RealmConfig()
        {
            AuthenticationPluginId = TestPlugin.Id,
            AuthenticationPluginConfigJObject = JObject.FromObject(new TestPluginConfig() { ValidityDuration = new TimeSpan(0, 0, 42, 0) }),
            PasswordPolicy = new PasswordOptions() { RequiredLength = 42, RequiredUniqueChars = 23 }
        };
        var result = controller.Update(_realmConfig.Realm, newRealmConfig);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<NoContentResult>(result);
        Assert.Equal(42, Assert.Single(_realms)?.AuthenticationPluginConfig?.ValidityDuration?.Minutes);
        Assert.Equal(23, Assert.Single(_realms)?.PasswordPolicy?.RequiredUniqueChars);
        Assert.Equal(42, Assert.Single(_realms)?.PasswordPolicy?.RequiredLength);
    }

    [Fact]
    public void UpdateUnknownRealmTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);

        var result = controller.Update("UnknownRealm", new RealmConfig());

        Assert.NotNull(result);
        Assert.IsAssignableFrom<NotFoundObjectResult>(result);
    }

    [Fact]
    public void UpdateWithUnknownAuthenticationTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);
        var newRealmConfig = new RealmConfig()
        {
            AuthenticationPluginId = Guid.Empty
        };
        var result = controller.Update(_realmConfig.Realm, newRealmConfig);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<NotFoundObjectResult>(result);
    }

    [Fact]
    public void UpdateBadRequestsTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);

        var resultNullId = controller.Update(null, new RealmConfig());
        var resultNullRealm = controller.Update("Test", null);

        Assert.NotNull(resultNullId);
        Assert.NotNull(resultNullRealm);
        Assert.IsAssignableFrom<BadRequestResult>(resultNullId);
        Assert.IsAssignableFrom<BadRequestResult>(resultNullRealm);
    }

    [Fact]
    public void DeleteTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);

        var result = controller.Delete(_realmConfig.Realm);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<NoContentResult>(result);
        Assert.Empty(_realms);
    }

    [Fact]
    public void DeleteNotFoundTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);

        var result = controller.Delete("UnknownRealm");

        Assert.NotNull(result);
        Assert.Equal("UnknownRealm", Assert.IsAssignableFrom<NotFoundObjectResult>(result).Value?.ToString());
        Assert.Single(_realms);
    }

    [Fact]
    public void DeleteBadRequestTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);

        var result = controller.Delete(null);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<BadRequestResult>(result);
    }

    [Fact]
    public void ImportTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);
        var importData = new RealmImportExport()
        {
            RealmConfig = new RealmConfig() {Realm = "TestRealm"}
        };

        RealmImportExport importedData = null;
        var importerMock = new Mock<IRealmSeedImporterExporter>();
        importerMock.Setup(m => m.ImportRealm(It.IsAny<RealmImportExport>()))
            .Callback<RealmImportExport>(data => importedData = data);

        var result = controller.Import(importData, importerMock.Object);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<NoContentResult>(result);
        Assert.NotNull(importedData);
    }

    [Fact]
    public void ImportBadRequestTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);
        var importData = new RealmImportExport();
        var importerMock = new Mock<IRealmSeedImporterExporter>();

        var result = controller.Import(importData, importerMock.Object);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<BadRequestResult>(result);
    }

    [Fact]
    public void ExportByIdTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);

        string exportFileName = "test.json";
        var importerMock = new Mock<IRealmSeedImporterExporter>();
        importerMock.Setup(m => m.Export(It.IsAny<string>(), out exportFileName))
            .Returns(() => new RealmImportExport());

        var result = controller.ExportById("TestRealm", importerMock.Object);

        var exportFile = Assert.IsAssignableFrom<FileContentResult>(result);
        Assert.NotNull(exportFile);
        Assert.Equal(exportFileName, exportFile.FileDownloadName);
        Assert.Equal(ContentTypes.ApplicationJson, exportFile.ContentType);
        Assert.NotEmpty(exportFile.FileContents);
    }

    [Fact]
    public void ExportByIdNotFoundTest()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);

        string exportFileName = "test.json";
        var importerMock = new Mock<IRealmSeedImporterExporter>();
        importerMock.Setup(m => m.Export(It.IsAny<string>(), out exportFileName))
            .Returns(() => throw new NotFoundException(string.Empty));

        var result = controller.ExportById("TestRealm", importerMock.Object);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task PublishClaimsTestAsync()
    {
        var controller = new RealmAdminController(_realmCatalogue.Object, _pluginCatalogue.Object, _localizer);

        var publishCalled = false;
        var publisherMock = new Mock<IClaimPublisher>();
        publisherMock.Setup(m => m.PublishClaimsAsync(It.IsAny<IEnumerable<ClaimDefinition>>()))
            .Callback(() => publishCalled = true);

        var result = await controller.PublishClaims(Enumerable.Empty<ClaimDefinition>(), publisherMock.Object);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<NoContentResult>(result);
        Assert.True(publishCalled);
    }


    private void InitMocks()
    {
        var pluginMeta = new Mock<IPluginMetadata>();
        pluginMeta.SetupGet(m => m.PluginId).Returns(TestPlugin.Id);
        pluginMeta.SetupGet(m => m.PluginConfigType).Returns(typeof(TestPluginConfig));
        _plugins = new List<IPluginMetadata> { pluginMeta.Object };

        _password = "TopSecret";
        _email = "test@example.com";
        var authenticationPlugin = new Mock<IAuthenticationPlugin>();
        authenticationPlugin.Setup(m => m.Authenticate(It.IsAny<IRequestTokenParameter>())).Returns<IRequestTokenParameter>(
            (authParam) =>
            {
                if (authParam.Username != "user" || authParam.Password != _password) throw new AuthenticationFailedException();
                return new AuthenticationToken() { Claims = { new Claim("foo", "bar") } };
            });
        authenticationPlugin.Setup(m => m.GetEMail(It.IsAny<string>(), It.IsAny<Guid?>())).Returns(() => _email);
        authenticationPlugin.Setup(m => m.ChangePassword(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<bool>())).Callback<string, Guid?, string, bool>(
            (_, _, pwd, _) =>
            {
                _password = pwd; 
            });

        _pluginCatalogue = new Mock<IPluginCatalogue>();
        _pluginCatalogue.SetupGet(m => m.Plugins).Returns(_plugins);
        _pluginCatalogue.Setup(m => m.GetAuthenticationPlugin(It.IsAny<Guid>())).Returns<Guid>((id) => _plugins.Any(p => p.PluginId == id) ? authenticationPlugin.Object : null);
        _pluginCatalogue.Setup(m => m.GetPluginMeta(It.IsAny<Guid>()))
            .Returns<Guid>(
                (id) => { return _plugins.FirstOrDefault(p => p.PluginId == id); });

        _realmConfig = new TestRealmConfig();
        _realms = new List<IRealmConfig>() { _realmConfig };

        _realmCatalogue = new Mock<IRealmCatalogue>();
        _realmCatalogue.SetupGet(m => m.Realms).Returns(_realms);
        _realmCatalogue.Setup(m => m.GetRealm(It.IsAny<string>()))
            .Returns<string>((id) => { return _realms.FirstOrDefault(p => p.Realm == id); });
        _realmCatalogue.Setup(m => m.Save(It.IsAny<IRealmConfig>())).Callback<IRealmConfig>(
            (config) =>
            {
                _realms.RemoveAll(r => r.Realm == config.Realm);
                _realms.Add(config);
            });
        _realmCatalogue.Setup(m => m.Delete(It.IsAny<string>())).Callback<string>(
            (id) =>
            {
                if (_realms.RemoveAll(r => r.Realm == id) == 0)
                    throw new NotFoundException(id);
            });

        _localizer = new MockLocalizer<RealmAdminController>();
    }
}