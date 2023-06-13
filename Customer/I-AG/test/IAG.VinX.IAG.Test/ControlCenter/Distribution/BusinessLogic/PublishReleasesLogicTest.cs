using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

using Moq;

using Xunit;

namespace IAG.VinX.IAG.Test.ControlCenter.Distribution.BusinessLogic;

public class PublishReleasesLogicTest
{
    private readonly PublishReleasesLogic _logic;
    private readonly Mock<IReleaseManager> _releaseManagerMock;
    private readonly Mock<IMessageLogger> _loggerMock;

    public PublishReleasesLogicTest()
    {
        _releaseManagerMock = new Mock<IReleaseManager>();
        _releaseManagerMock.Setup(m => m.GetProductsAsync())
            .ReturnsAsync(() => new List<ProductInfo> { new()
            {
                Id = Guid.NewGuid(),
                ProductName = "JustARandomProduct",
                ProductType = ProductType.IagService
            }});
        _releaseManagerMock.Setup(m => m.GetReleasesAsync())
            .ReturnsAsync(() => new List<ReleaseInfo> { new()
            {
                Id = Guid.NewGuid(),
                ReleaseVersion = "TooOld",
                ArtifactPath = "WhatEver",
                ReleaseDate = DateTime.Today.AddDays(-10)
            }});

        var artifactsScannerMock = new Mock<IArtifactsScanner>();
        artifactsScannerMock.Setup(m => m.Scan(It.IsAny<string>())).Returns(
            new List<ArtifactInfo>
            {
                new()
                {
                    ProductName = "TestProduct", 
                    ArtifactPath = "TestProductArtifactPath", 
                    ProductType = ProductType.IagService
                },
                new()
                {
                    ProductName = "TestPlugin",
                    ArtifactPath = "TestPluginArtifactPath",
                    ProductType = ProductType.CustomerExtension,
                    DependingProductName = "TestProduct"
                }
            });

        var settingsScannerMock = new Mock<ISettingsScanner>();
        settingsScannerMock.Setup(m => m.Scan(It.IsAny<string>())).Returns(
            new List<ArtifactInfo>
            {
                new()
                {
                    ProductName = "TestSettings",
                    ArtifactPath = "TestSettingsPath",
                    ArtifactName = "Test",
                    ProductType = ProductType.ConfigTemplate,
                    DependingProductName = "TestProduct",
                }
            });

        _loggerMock = new Mock<IMessageLogger>();

        _logic = new PublishReleasesLogic(artifactsScannerMock.Object, settingsScannerMock.Object, _releaseManagerMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task HappyPathTest()
    {
        var createdProducts = new List<ProductInfo>();
        var disabledReleases = new List<string>();

        IDictionary<Guid, string> createdArtifactPaths = new Dictionary<Guid, string>();
        IDictionary<Guid, string> createdReleasePaths = new Dictionary<Guid, string>();
        IDictionary<Guid, string> createdReleaseVersions = new Dictionary<Guid, string>();

        _releaseManagerMock.Setup(m => m.CreateProductAsync(It.IsAny<string>(), It.IsAny<ProductType>(), It.IsAny<Guid?>()))
            .ReturnsAsync((string productName, ProductType productType, Guid? dependsOnProductId) =>
            {
                var product = new ProductInfo()
                {
                    Id = Guid.NewGuid(),
                    ProductName = productName,
                    ProductType = productType,
                    DependsOnProductId = dependsOnProductId
                };
                createdProducts.Add(product);
                    
                return product;
            });

        _releaseManagerMock.Setup(m => m.CreateReleaseAsync(It.IsAny<ProductInfo>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IJobHeartbeatObserver>()))
            .ReturnsAsync((ProductInfo product, string artifactPath, string releasePath, string releaseVersion, IJobHeartbeatObserver _) =>
            {
                createdArtifactPaths.Add(product.Id, artifactPath);
                createdReleasePaths.Add(product.Id, releasePath);
                createdReleaseVersions.Add(product.Id, releaseVersion);
                    
                return new ReleaseInfo {ProductId = product.Id};
            });

        _releaseManagerMock.Setup(m => m.RemoveReleaseAsync(It.IsAny<ReleaseInfo>()))
            .Callback<ReleaseInfo>(release =>
            {
                disabledReleases.Add(release.ReleaseVersion);
            });

        await _logic.PublishReleasesAsync("FakeArtifactsPath", "FakeSettingsPath",
            new Dictionary<string, string>
            {
                {"TestProduct", "TestReleasePath"},
                {"Settings", "TestSettingsReleasePath"}
            }, 
            new SyncResult(), new Mock<IJobHeartbeatObserver>().Object);

        var testProduct = Assert.Single(createdProducts, p => p.ProductName == "TestProduct");
        var testSettings = Assert.Single(createdProducts, p => p.ProductName == "TestSettings");
        Assert.NotNull(testProduct);
        Assert.NotNull(testSettings);
        Assert.Equal(ProductType.IagService, testProduct.ProductType);
        Assert.Equal(ProductType.ConfigTemplate, testSettings.ProductType);
        Assert.Null(testProduct.DependsOnProductId);
        Assert.Equal(testProduct.Id, testSettings.DependsOnProductId);
        Assert.All(createdProducts, p => Assert.Contains(p.Id, createdArtifactPaths));
        Assert.All(createdProducts, p => Assert.Contains(p.Id, createdReleasePaths));
        Assert.All(createdProducts, p => Assert.Contains(p.Id, createdReleaseVersions));
        Assert.Equal("TestProductArtifactPath", createdArtifactPaths[testProduct.Id]);
        Assert.Equal("TestReleasePath", createdReleasePaths[testProduct.Id]);
        Assert.Null(createdReleaseVersions[testProduct.Id]);
        Assert.Equal("TestSettingsPath", createdArtifactPaths[testSettings.Id]);
        Assert.Equal("TestSettingsReleasePath", createdReleasePaths[testSettings.Id]);
        Assert.Empty(createdReleaseVersions[testSettings.Id]);
        Assert.Equal("TooOld", Assert.Single(disabledReleases));
    }

    [Fact]
    public async Task RePublishReleasesTest()
    {
        var exceptionLogged = false;
        _loggerMock.Setup(m => m.AddMessage(It.IsAny<MessageTypeEnum>(), It.IsAny<string>(), It.IsAny<object[]>())).Callback<MessageTypeEnum, string, object[]>((type, _, _) =>
        {
            exceptionLogged = exceptionLogged || (type == MessageTypeEnum.Error);
        });

        _releaseManagerMock.Setup(m => m.CreateReleaseAsync(It.IsAny<ProductInfo>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IJobHeartbeatObserver>()))
            .ThrowsAsync(new LocalizableException("I-AG.Distribution.ReleaseAlreadyApproved.Error"));

        await _logic.PublishReleasesAsync("FakeArtifactsPath", "FakeSettingsPath", new Dictionary<string, string>(), new SyncResult(), new Mock<IJobHeartbeatObserver>().Object);

        Assert.False(exceptionLogged);
    }

    [Fact]
    public async Task PublishReleasesCreateProductErrorTest()
    {
        var exceptionLogged = false;
        _loggerMock.Setup(m => m.AddMessage(It.IsAny<MessageTypeEnum>(), It.IsAny<string>(), It.IsAny<object[]>())).Callback<MessageTypeEnum, string, object[]>((type, _, _) =>
        {
            exceptionLogged = exceptionLogged || (type == MessageTypeEnum.Error);
        });

        _releaseManagerMock.Setup(m => m.CreateProductAsync(It.IsAny<string>(), It.IsAny<ProductType>(), It.IsAny<Guid?>()))
            .ThrowsAsync(new Exception("Test"));

        await  _logic.PublishReleasesAsync("FakeArtifactsPath", "FakeSettingsPath", new Dictionary<string, string>(), new SyncResult(), new Mock<IJobHeartbeatObserver>().Object);

        Assert.True(exceptionLogged);
    }

    [Fact]
    public async Task PublishReleasesDisableReleaseErrorTest()
    {
        var warningLogged = false;
        _loggerMock.Setup(m => m.AddMessage(It.IsAny<MessageTypeEnum>(), It.IsAny<string>(), It.IsAny<object[]>())).Callback<MessageTypeEnum, string, object[]>((type, _, _) =>
        {
            warningLogged = warningLogged || (type == MessageTypeEnum.Warning);
        });

        _releaseManagerMock.Setup(m => m.RemoveReleaseAsync(It.IsAny<ReleaseInfo>()))
            .ThrowsAsync(new Exception("Test"));

        await _logic.PublishReleasesAsync("FakeArtifactsPath", "FakeSettingsPath", new Dictionary<string, string>(), new SyncResult(), new Mock<IJobHeartbeatObserver>().Object);

        Assert.True(warningLogged);
    }

    [Fact]
    public async Task PublishReleasesOperationCanceledTest()
    {
        _releaseManagerMock.Setup(m => m.CreateProductAsync(It.IsAny<string>(), It.IsAny<ProductType>(), It.IsAny<Guid?>()))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(() 
            => _logic.PublishReleasesAsync("FakeArtifactsPath", "FakeSettingsPath", new Dictionary<string, string>(), new SyncResult(), new Mock<IJobHeartbeatObserver>().Object));
    }
}