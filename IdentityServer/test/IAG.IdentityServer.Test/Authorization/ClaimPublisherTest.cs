using System;
using System.Collections.Generic;

using IAG.IdentityServer.Authorization;
using IAG.IdentityServer.Configuration;
using IAG.IdentityServer.Configuration.Model.Realm;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Plugin;

using Moq;

using Xunit;

namespace IAG.IdentityServer.Test.Authorization;

public class ClaimPublisherTest
{
    private bool _publishClaimsEnabled;
    private readonly List<ClaimDefinition> _publishedClaimDefinitions;
    private readonly ClaimPublisher _claimPublisher;

    public ClaimPublisherTest()
    {
        var catalogue = new List<IPluginMetadata>() {new PluginMetadata()};
        var pluginCatalogueMock = new Mock<IPluginCatalogue>();
        var realmCatalogueMock = new Mock<IRealmCatalogue>();
        var pluginMock = new Mock<IAuthenticationPlugin>();
        var pluginConfigMock = new Mock<IAuthenticationPluginConfig>();
        var realmConfigMock = new Mock<IRealmConfig>();

        pluginConfigMock.Setup(m => m.PublishClaims).Returns(() => _publishClaimsEnabled);
        pluginConfigMock.Setup(m => m.Active).Returns(() => true);
        pluginMock.Setup(m => m.Config).Returns(pluginConfigMock.Object);
        _publishedClaimDefinitions = new List<ClaimDefinition>();
        pluginMock.Setup(m => m.AddClaimDefinitions(It.IsAny<IEnumerable<ClaimDefinition>>()))
            .Callback<IEnumerable<ClaimDefinition>>(cds => { _publishedClaimDefinitions.AddRange(cds); });

        pluginCatalogueMock.Setup(m => m.Plugins).Returns(catalogue);
        pluginCatalogueMock.Setup(m => m.GetAuthenticationPlugin(It.IsAny<Guid>())).Returns(pluginMock.Object);

        realmConfigMock.Setup(m => m.AuthenticationPluginConfig).Returns(pluginConfigMock.Object);
        realmCatalogueMock.Setup(m => m.Realms).Returns(new List<IRealmConfig>() { realmConfigMock.Object });

        _claimPublisher = new ClaimPublisher(pluginCatalogueMock.Object, realmCatalogueMock.Object);
    }

    [Fact]
    public void PublishClaimsEnabledTest()
    {
        _publishClaimsEnabled = true;
        var claimDefinition = new ClaimDefinition()
        {
            ScopeName = "scope", ClaimName = "claim", TenantId = Guid.NewGuid(),
            AvailablePermissions = PermissionKind.Read
        };

        _claimPublisher.PublishClaimsAsync(new[] {claimDefinition});

        var publishedClaimDefinition = Assert.Single(_publishedClaimDefinitions);
        Assert.NotNull(publishedClaimDefinition);
        Assert.Equal(claimDefinition.ScopeName, publishedClaimDefinition.ScopeName);
        Assert.Equal(claimDefinition.ClaimName, publishedClaimDefinition.ClaimName);
        Assert.Equal(claimDefinition.TenantId, publishedClaimDefinition.TenantId);
        Assert.Equal(claimDefinition.AvailablePermissions, publishedClaimDefinition.AvailablePermissions);
    }


    [Fact]
    public void PublishClaimsDisabledTest()
    {
        _publishClaimsEnabled = false;
        var claimDefinition = new ClaimDefinition();

        _claimPublisher.PublishClaimsAsync(new[] {claimDefinition});

        Assert.Empty(_publishedClaimDefinitions);
    }
}