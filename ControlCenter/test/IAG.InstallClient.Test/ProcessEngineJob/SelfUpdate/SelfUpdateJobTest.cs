using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.ProcessEngineJob.SelfUpdate;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace IAG.InstallClient.Test.ProcessEngineJob.SelfUpdate;

public class SelfUpdateJobTest
{
    private const string CurrentSelfVersion = "CurrentSelfVersion";

    private readonly Mock<IJobInfrastructure> _jobInfrastructureMock;
    private readonly SelfUpdateJob _selfUpdateJob;

    private readonly CustomerInfo _testCustomer;
    private readonly Mock<ICustomerManager> _customerManagerMock;

    private readonly List<ProductInfo> _products = new();
    private readonly List<ReleaseInfo> _releases = new();

    public SelfUpdateJobTest()
    {
        _jobInfrastructureMock = new Mock<IJobInfrastructure>();

        _testCustomer = new CustomerInfo
        {
            Id = Guid.NewGuid(),
            CustomerName = "TestCustomer",
            Description = "TestDescription"
        };
        _customerManagerMock = new Mock<ICustomerManager>();

        var releaseManagerMock = new Mock<IReleaseManager>();
        releaseManagerMock.Setup(m => m.GetProductsAsync(_testCustomer.Id))
            .ReturnsAsync(() => _products);
        releaseManagerMock.Setup(m => m.GetReleasesAsync(_testCustomer.Id, It.IsAny<Guid>()))
            .ReturnsAsync(() => _releases);

        var installationManagerMock = new Mock<IInstallationManager>();
        installationManagerMock.Setup(m => m.CurrentSelfVersion).Returns(CurrentSelfVersion);

        _selfUpdateJob = new SelfUpdateJob(_customerManagerMock.Object, releaseManagerMock.Object, installationManagerMock.Object);
    }

    [Fact]
    public void ExecuteJobWithoutCustomerTest()
    {
        _selfUpdateJob.Execute(_jobInfrastructureMock.Object);

        Assert.Equal(JobResultEnum.Failed, _selfUpdateJob.Result.Result);
    }

    [Fact]
    public void ExecuteJobWithoutUpdaterProductTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _selfUpdateJob.Execute(_jobInfrastructureMock.Object);

        Assert.Equal(JobResultEnum.Failed, _selfUpdateJob.Result.Result);
    }

    [Fact]
    public void ExecuteJobWithoutUpdaterReleaseTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        var updaterProduct = new ProductInfo
        {
            Id = Guid.NewGuid(),
            ProductType = ProductType.Updater
        };
        _products.Add(updaterProduct);

        _selfUpdateJob.Execute(_jobInfrastructureMock.Object);

        Assert.Equal(JobResultEnum.Failed, _selfUpdateJob.Result.Result);
    }

    [Fact]
    public void ExecuteJobWithoutNewUpdaterReleaseTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        var updaterProduct = new ProductInfo
        {
            Id = Guid.NewGuid(),
            ProductType = ProductType.Updater
        };
        _products.Add(updaterProduct);

        var updaterRelease = new ReleaseInfo
        {
            Id = Guid.NewGuid(),
            ProductId = updaterProduct.Id,
            ReleaseDate = DateTime.Today,
            ReleaseVersion = CurrentSelfVersion
        };
        _releases.Add(updaterRelease);

        _selfUpdateJob.Execute(_jobInfrastructureMock.Object);

        Assert.Equal(JobResultEnum.Success, _selfUpdateJob.Result.Result);
    }

    [Fact]
    public void ExecuteJobWithNewUpdaterReleaseTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        var updaterProduct = new ProductInfo
        {
            Id = Guid.NewGuid(),
            ProductType = ProductType.Updater
        };
        _products.Add(updaterProduct);

        var updaterRelease = new ReleaseInfo
        {
            Id = Guid.NewGuid(),
            ProductId = updaterProduct.Id,
            ReleaseDate = DateTime.Today,
            ReleaseVersion = "NewVersion"
        };
        _releases.Add(updaterRelease);

        _selfUpdateJob.Execute(_jobInfrastructureMock.Object);

        Assert.Equal(JobResultEnum.Success, _selfUpdateJob.Result.Result);
    }
}