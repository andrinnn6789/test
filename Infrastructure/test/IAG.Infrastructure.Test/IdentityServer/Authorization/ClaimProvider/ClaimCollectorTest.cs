using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.Infrastructure.DI;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

using Moq;

using Xunit;

namespace IAG.Infrastructure.Test.IdentityServer.Authorization.ClaimProvider;

public class ClaimCollectorTest
{
    private readonly ClaimCollector _claimCollector;
    private readonly List<Type> _claimProviderTypes;
    private readonly List<ClaimDefinition> _publishedClaimDefinitions;


    public ClaimCollectorTest()
    {
        _claimProviderTypes = new List<Type>();
        _publishedClaimDefinitions = new List<ClaimDefinition>();

        var pluginLoaderMock = new Mock<IPluginLoader>();
        pluginLoaderMock.Setup(m => m.GetImplementations<IClaimProvider>(It.IsAny<string>(), It.IsAny<bool>())).Returns(_claimProviderTypes);

        var claimPublisherMock = new Mock<IClaimPublisher>();
        claimPublisherMock.Setup(m => m.PublishClaimsAsync(It.IsAny<IEnumerable<ClaimDefinition>>()))
            .Callback<IEnumerable<ClaimDefinition>>(cds => { _publishedClaimDefinitions.AddRange(cds); });

        _claimCollector = new ClaimCollector(pluginLoaderMock.Object, claimPublisherMock.Object);
    }

    [Fact]
    public async Task SimpleClaimCollectingTest()
    {
        _claimProviderTypes.Add(typeof(DummyClaimProvider));
        await _claimCollector.CollectAndUpdateAsync();

        var publishedClaimDefinition = Assert.Single(_publishedClaimDefinitions);
        Assert.NotNull(publishedClaimDefinition);
        Assert.Equal(DummyClaimProvider.TestScopeName, publishedClaimDefinition.ScopeName);
        Assert.Equal(DummyClaimProvider.TestClaimName, publishedClaimDefinition.ClaimName);
        Assert.Equal(DummyClaimProvider.TestTenantId, publishedClaimDefinition.TenantId);
        Assert.Equal(PermissionKind.Crud, publishedClaimDefinition.AvailablePermissions);
    }

    [Fact]
    public async Task DuplicateExceptionClaimCollectingTest()
    {
        _claimProviderTypes.Add(typeof(DummyClaimProvider));
        _claimProviderTypes.Add(typeof(DummyClaimProvider));
            
        await Assert.ThrowsAsync<LocalizableException>(() => _claimCollector.CollectAndUpdateAsync());
    }

    private class DummyClaimProvider : Infrastructure.IdentityServer.Authorization.ClaimProvider.ClaimProvider
    {
        public static readonly string TestScopeName = "testScope";
        public static readonly string TestClaimName = "testScope";
        public static readonly Guid? TestTenantId = null;

        public DummyClaimProvider()
        {
            AddClaimDefinition(TestTenantId, TestScopeName, TestClaimName, PermissionKind.Crud);
        }
    }
}