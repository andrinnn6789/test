using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.TestHelper.Globalization.Mocks;
using IAG.Infrastructure.TestHelper.Session;
using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.BusinessLogic.Model;
using IAG.InstallClient.Controllers;
using IAG.InstallClient.Models;
using IAG.ProcessEngine.Execution;
using IAG.ProcessEngine.Execution.Model;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

using Moq;

using Xunit;

namespace IAG.InstallClient.Test.Controllers;

public class InstallationControllerTest
{
    private readonly InstallationController _controller;
    private readonly CustomerInfo _testCustomer;
    private readonly ProductInfo _testProduct;
    private readonly ReleaseInfo _testRelease;
    private readonly IJobInstance _jobInstance;
    private readonly Mock<ICustomerManager> _customerManagerMock;
    private readonly Mock<IReleaseManager> _releaseManagerMock;
    private readonly Mock<IInstallationManager> _installationManagerMock;
    private readonly Mock<IServiceManager> _serviceManagerMock;
    private readonly Mock<IInventoryManager> _inventoryManagerMock;

    public InstallationControllerTest()
    {
        _testCustomer = new CustomerInfo
        {
            Id = Guid.NewGuid(),
            CustomerName = "TestCustomer",
            Description = "TestDescription"
        };
        _testProduct = new ProductInfo
        {
            Id = Guid.NewGuid(),
            ProductName = "TestProduct"
        };
        _testRelease = new ReleaseInfo
        {
            Id = Guid.NewGuid(),
            ProductId = _testProduct.Id,
            ReleaseVersion = "1.0",
            ReleaseDate = DateTime.Today.AddDays(-1),
            ReleasePath = "TestReleasePath"
        };

        _installationManagerMock = new Mock<IInstallationManager>();
        _releaseManagerMock = new Mock<IReleaseManager>();
        _serviceManagerMock = new Mock<IServiceManager>();
        _customerManagerMock = new Mock<ICustomerManager>();
        _inventoryManagerMock = new Mock<IInventoryManager>();

        _jobInstance = new JobInstance(null)
        {
            State = new JobState(),
            Job = new Mock<IJob>().Object
        };
        var jobServiceMock = new Mock<IJobService>();
        jobServiceMock.Setup(m => m.CreateJobInstance(It.IsAny<Guid>(), It.IsAny<IJobState>()))
            .Returns(_jobInstance);
        jobServiceMock.Setup(m => m.GetJobInstanceState(It.IsAny<Guid>()))
            .Returns(_jobInstance.State);

        var localizer = new MockLocalizer<InstallationController>();
        var httpContext = new DefaultHttpContext
        {
            Session = new MockSession()
        };
        httpContext.Session.Set("UpdateCheck.Done", new byte[] {1});

        _controller = new InstallationController(_installationManagerMock.Object, _releaseManagerMock.Object, _serviceManagerMock.Object, _customerManagerMock.Object, _inventoryManagerMock.Object, jobServiceMock.Object, localizer)
        {
            ControllerContext = new ControllerContext(new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor()))
        };
    }

    [Fact]
    public async Task GetIndexWithoutCustomerTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync((CustomerInfo)null);

        var result = await _controller.Index();

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Customer", redirectResult.ControllerName);
        Assert.Equal(nameof(CustomerController.Index), redirectResult.ActionName);
    }

    [Fact]
    public async Task GetIndexNormalTest()
    {
        var testInstallation = new InstalledRelease
        {
            InstanceName = "TestInstance",
            ProductName = "TestProduct",
            Version = "TestVersion"
        };

        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _serviceManagerMock.Setup(m => m.GetServiceName(It.IsAny<string>()))
            .Returns("TestServiceName");

        _installationManagerMock.Setup(m => m.GetInstallationsAsync())
            .ReturnsAsync(new [] { testInstallation});

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var overviewModel = Assert.IsType<InstallationOverviewViewModel>(viewResult.Model);
        var singleInstallationModel = Assert.Single(overviewModel.Installations);
        Assert.NotNull(singleInstallationModel);
        Assert.Equal(testInstallation.InstanceName, singleInstallationModel.InstanceName);
        Assert.Equal(testInstallation.ProductName, singleInstallationModel.ProductName);
        Assert.Equal(testInstallation.Version, singleInstallationModel.Version);
        Assert.Null(testInstallation.CustomerPluginName);
        Assert.Equal("TestServiceName", singleInstallationModel.ServiceName);
        Assert.Null(overviewModel.ErrorMessage);
    }

    [Fact]
    public async Task GetIndexWithExceptionTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _installationManagerMock.Setup(m => m.GetInstallationsAsync())
            .Returns(() => throw new Exception("TestError"));

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var overviewModel = Assert.IsType<InstallationOverviewViewModel>(viewResult.Model);
        Assert.Empty(overviewModel.Installations);
        Assert.Equal("TestError", overviewModel.ErrorMessage);
    }

    [Fact]
    public async Task PostIndexStartServiceTest()
    {
        var testInstallationModel = new InstallationViewModel
        {
            ServiceName = "TestServiceName",
        };
        string startedService = null;

        _serviceManagerMock.Setup(m => m.StartService(It.IsAny<string>()))
            .Callback<string>(service => startedService = service);

        var result = await _controller.Index(testInstallationModel, InstallationController.InstallationAction.StartService);

        Assert.Equal(testInstallationModel.ServiceName, startedService);
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<InstallationOverviewViewModel>(viewResult.Model);
    }

    [Fact]
    public async Task PostIndexStartServiceFailTest()
    {
        var testInstallationModel = new InstallationViewModel();

        _serviceManagerMock.Setup(m => m.StartService(It.IsAny<string>()))
            .Callback(() => throw new Exception("StartServiceTestError"));

        _installationManagerMock.Setup(m => m.GetInstallationsAsync())
            .Returns(() => throw new Exception("GetInstallationsTestError"));

        var result = await _controller.Index(testInstallationModel, InstallationController.InstallationAction.StartService);

        var viewResult = Assert.IsType<ViewResult>(result);
        var overviewModel = Assert.IsType<InstallationOverviewViewModel>(viewResult.Model);
        Assert.Contains("StartServiceTestError", overviewModel.ErrorMessage);
        Assert.Contains("GetInstallationsTestError", overviewModel.ErrorMessage);
    }

    [Fact]
    public async Task PostIndexStopServiceTest()
    {
        var testInstallationModel = new InstallationViewModel
        {
            ServiceName = "TestServiceName",
        };
        string stoppedService = null;

        _serviceManagerMock.Setup(m => m.StopService(It.IsAny<string>()))
            .Callback<string>(service => stoppedService = service);

        var result = await _controller.Index(testInstallationModel, InstallationController.InstallationAction.StopService);

        Assert.Equal(testInstallationModel.ServiceName, stoppedService);
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<InstallationOverviewViewModel>(viewResult.Model);
    }

    [Fact]
    public async Task PostIndexInstallServiceTest()
    {
        var testInstallationModel = new InstallationViewModel
        {
            ProductName = "TestProductName",
            InstanceName = "TestInstanceName"
        };
        string installedProduct = null;
        string installedInstance = null;

        _serviceManagerMock.Setup(m => m.InstallService(It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string>((product, instance) =>
            {
                installedProduct = product;
                installedInstance = instance;
            });

        var result = await _controller.Index(testInstallationModel, InstallationController.InstallationAction.InstallService);

        Assert.Equal(testInstallationModel.ProductName, installedProduct);
        Assert.Equal(testInstallationModel.InstanceName, installedInstance);
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<InstallationOverviewViewModel>(viewResult.Model);
    }

    [Fact]
    public async Task PostIndexDeleteTest()
    {
        var testInstallationModel = new InstallationViewModel
        {
            InstanceName = "TestInstance",
            ServiceName = "TestServiceName"
        };
        string uninstalledService = null;
        string deletedInstanceName = null;
        string deRegisteredInstallationInstanceName = null;

        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _serviceManagerMock.Setup(m => m.UninstallService(It.IsAny<string>()))
            .Callback<string>(service => uninstalledService = service);
        _installationManagerMock.Setup(m => m.DeleteInstance(It.IsAny<string>()))
            .Callback<string>(instanceName => deletedInstanceName = instanceName);

        _inventoryManagerMock.Setup(m => m.DeRegisterInstallationAsync(_testCustomer.Id, It.IsAny<string>()))
            .Callback<Guid, string>((_, instance) =>
            {
                deRegisteredInstallationInstanceName = instance;
            });

        var result = await _controller.Index(testInstallationModel, InstallationController.InstallationAction.Delete);

        Assert.Equal(testInstallationModel.InstanceName, deletedInstanceName);
        Assert.Equal(testInstallationModel.ServiceName, uninstalledService);
        Assert.Equal(testInstallationModel.InstanceName, deRegisteredInstallationInstanceName);
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<InstallationOverviewViewModel>(viewResult.Model);
    }

    [Fact]
    public async Task PostIndexUnknownActionTest()
    {
        var testInstallationModel = new InstallationViewModel();

        var result = await _controller.Index(testInstallationModel, InstallationController.InstallationAction.Undefined);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<InstallationOverviewViewModel>(viewResult.Model);
    }

    [Fact]
    public async Task GetNewNormalTest()
    {
        var testInstallation = new InstalledRelease
        {
            InstanceName = "TestInstance",
            ProductName = "TestProduct",
            Version = "TestVersion"
        };

        var updaterProduct = new ProductInfo()
        {
            Id = Guid.NewGuid(),
            ProductType = ProductType.Updater
        };
        var updaterConfigTemplate = new ProductInfo()
        {
            Id = Guid.NewGuid(),
            DependsOnProductId = updaterProduct.Id,
            ProductType = ProductType.ConfigTemplate
        };


        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _installationManagerMock.Setup(m => m.GetInstallationsAsync())
            .ReturnsAsync(new[] { testInstallation });

        _releaseManagerMock.Setup(m => m.GetProductsAsync(_testCustomer.Id))
            .ReturnsAsync(new[] { _testProduct, updaterProduct, updaterConfigTemplate });
        _releaseManagerMock.Setup(m => m.GetReleasesAsync(_testCustomer.Id, _testProduct.Id))
            .ReturnsAsync(new [] {_testRelease});

        var result = await _controller.New();

        var viewResult = Assert.IsType<ViewResult>(result);
        var newInstallationModel = Assert.IsType<NewInstallationViewModel>(viewResult.Model);
        var availableRelease = Assert.Single(newInstallationModel.AvailableReleases);
        Assert.NotNull(availableRelease);
        Assert.Contains(testInstallation.InstanceName, newInstallationModel.ExistingInstances);
        Assert.Equal(_testProduct.Id, availableRelease.ProductId);
        Assert.Equal(_testProduct.ProductName, availableRelease.ProductName);
        Assert.Equal(_testRelease.Id, availableRelease.ReleaseId);
        Assert.Equal(_testRelease.ReleaseVersion, availableRelease.ReleaseVersion);
        Assert.Equal(_testRelease.ReleaseDate, availableRelease.ReleaseDate);
        Assert.Null(newInstallationModel.SelectedRelease);
        Assert.Null(newInstallationModel.SelectedInstance);
        Assert.Null(newInstallationModel.ErrorMessage);
    }

    [Fact]
    public async Task GetNewWithExceptionTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _installationManagerMock.Setup(m => m.GetInstallationsAsync())
            .Returns(() => throw new Exception("TestError"));

        var result = await _controller.New();

        var viewResult = Assert.IsType<ViewResult>(result);
        var newInstallationModel = Assert.IsType<NewInstallationViewModel>(viewResult.Model);
        Assert.Equal("TestError", newInstallationModel.ErrorMessage);
    }

    [Fact]
    public async Task PostNewInstallationNormalTest()
    {
        var installations = new List<InstalledRelease>();
        var testInstallationPath = "TestInstallationPath";

        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _installationManagerMock.Setup(m => m.CreateOrUpdateInstallationAsync(It.IsAny<InstallationSetup>(), It.IsAny<IMessageLogger>()))
            .ReturnsAsync((InstallationSetup setup, IMessageLogger _) =>
            {
                installations.Add(new InstalledRelease
                {
                    InstanceName = setup.InstanceName,
                });
                    
                return testInstallationPath;
            });
        _installationManagerMock.Setup(m => m.GetInstallationsAsync()).ReturnsAsync(installations);

        _releaseManagerMock.Setup(m => m.GetProductsAsync(_testCustomer.Id))
            .ReturnsAsync(new[] { _testProduct });
        _releaseManagerMock.Setup(m => m.GetReleasesAsync(_testCustomer.Id, _testProduct.Id))
            .ReturnsAsync(new[] { _testRelease });

        var newInstallationViewModel = new NewInstallationViewModel
        {
            SelectedInstance = "NewInstance",
            SelectedRelease = _testRelease.Id
        };

        var result = await _controller.NewInstallation(newInstallationViewModel);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<NewInstallationViewModel>(viewResult.Model);
        Assert.NotNull(model);
        Assert.Null(model.ErrorMessage);
        Assert.Equal(_jobInstance.Id, model.InstallationJobId);
    }


    [Fact]
    public async Task PostNewInstallationInvalidModelTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _installationManagerMock.Setup(m => m.GetInstallationsAsync()).ReturnsAsync(Enumerable.Empty<InstalledRelease>());

        _releaseManagerMock.Setup(m => m.GetProductsAsync(_testCustomer.Id))
            .ReturnsAsync(new[] { _testProduct });
        _releaseManagerMock.Setup(m => m.GetReleasesAsync(_testCustomer.Id, _testProduct.Id))
            .ReturnsAsync(new[] { _testRelease });

        var newInstallationViewModel = new NewInstallationViewModel
        {
            SelectedRelease = _testRelease.Id
        };

        var result = await _controller.NewInstallation(newInstallationViewModel);

        var viewResult = Assert.IsType<ViewResult>(result);
        var installationOverviewModel = Assert.IsType<NewInstallationViewModel>(viewResult.Model);
        Assert.NotNull(installationOverviewModel);
        Assert.Single(installationOverviewModel.AvailableReleases);
        Assert.Empty(installationOverviewModel.ExistingInstances);
        Assert.Null(installationOverviewModel.SelectedInstance);
        Assert.Equal(_testRelease.Id, installationOverviewModel.SelectedRelease);
    }

    [Fact]
    public async Task PostNewInstallationWithNotExistingReleaseTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _releaseManagerMock.Setup(m => m.GetProductsAsync(_testCustomer.Id))
            .ReturnsAsync(new[] { _testProduct });
        _releaseManagerMock.Setup(m => m.GetReleasesAsync(_testCustomer.Id, _testProduct.Id))
            .ReturnsAsync(new[] { _testRelease });

        var newInstallationViewModel = new NewInstallationViewModel
        {
            SelectedInstance = "NewInstance",
            SelectedRelease = Guid.NewGuid()
        };

        var result = await _controller.NewInstallation(newInstallationViewModel);

        var viewResult = Assert.IsType<ViewResult>(result);
        var newInstallationModel = Assert.IsType<NewInstallationViewModel>(viewResult.Model);
        Assert.NotEmpty(newInstallationModel.ErrorMessage);
    }
        
    [Fact]
    public async Task GetUpdateNormalTest()
    {
        var testInstallation = new InstalledRelease
        {
            InstanceName = "TestInstance",
            ProductName = "TestProduct",
            Version = "TestVersion"
        };

        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _installationManagerMock.Setup(m => m.GetInstallationsAsync())
            .ReturnsAsync(new[] {testInstallation});

        _releaseManagerMock.Setup(m => m.GetProductsAsync(_testCustomer.Id))
            .ReturnsAsync(new[] {_testProduct});
        _releaseManagerMock.Setup(m => m.GetReleasesAsync(_testCustomer.Id, _testProduct.Id))
            .ReturnsAsync(new[] {_testRelease});

        var result = await _controller.Update(testInstallation.InstanceName, testInstallation.ProductName);

        var viewResult = Assert.IsType<ViewResult>(result);
        var updateInstallationModel = Assert.IsType<UpdateInstallationViewModel>(viewResult.Model);
        var availableRelease = Assert.Single(updateInstallationModel.AvailableReleases);
        Assert.NotNull(availableRelease);
        Assert.Equal(_testProduct.Id, availableRelease.ProductId);
        Assert.Equal(_testProduct.ProductName, availableRelease.ProductName);
        Assert.Equal(_testRelease.Id, availableRelease.ReleaseId);
        Assert.Equal(_testRelease.ReleaseVersion, availableRelease.ReleaseVersion);
        Assert.Equal(_testRelease.ReleaseDate, availableRelease.ReleaseDate);
        Assert.Equal(testInstallation.InstanceName, updateInstallationModel.SelectedInstance);
        Assert.Null(updateInstallationModel.SelectedRelease);
        Assert.Null(updateInstallationModel.ErrorMessage);
    }

    [Fact]
    public async Task GetUpdateWithExceptionTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _installationManagerMock.Setup(m => m.GetInstallationsAsync())
            .Returns(() => throw new Exception("TestError"));

        var result = await _controller.Update(string.Empty, string.Empty);

        var viewResult = Assert.IsType<ViewResult>(result);
        var updateInstallationModel = Assert.IsType<UpdateInstallationViewModel>(viewResult.Model);
        Assert.Equal("TestError", updateInstallationModel.ErrorMessage);
    }

    [Fact]
    public async Task PostUpdateInstallationNormalTest()
    {
        string stoppedService = null;

        var updateInstallationViewModel = new UpdateInstallationViewModel
        {
            SelectedInstance = "TestInstance",
            SelectedRelease = _testRelease.Id,
            ServiceName = "TestService"
        };

        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _releaseManagerMock.Setup(m => m.GetProductsAsync(_testCustomer.Id))
            .ReturnsAsync(new[] { _testProduct });
        _releaseManagerMock.Setup(m => m.GetReleasesAsync(_testCustomer.Id, _testProduct.Id))
            .ReturnsAsync(new[] { _testRelease });

        _serviceManagerMock.Setup(m => m.GetServiceState(updateInstallationViewModel.ServiceName))
            .Returns(ServiceStatus.Running);
        _serviceManagerMock.Setup(m => m.GetServiceName(updateInstallationViewModel.SelectedInstance))
            .Returns(updateInstallationViewModel.ServiceName);
        _serviceManagerMock.Setup(m => m.StopService(It.IsAny<string>()))
            .Callback<string>(service => stoppedService = service);

        var result = await _controller.UpdateInstallation(updateInstallationViewModel);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UpdateInstallationViewModel>(viewResult.Model);
        Assert.NotNull(model);
        Assert.Null(model.ErrorMessage);
        Assert.NotNull(model.InstallationJobId);
        Assert.Equal(_jobInstance.Id, model.InstallationJobId);
        Assert.Equal(model.ServiceName, stoppedService);
    }

    [Fact]
    public async Task PostUpdateInstallationWithoutServiceTest()
    {
        var testInstallation = new InstalledRelease
        {
            InstanceName = "TestInstance",
        };

        var updateInstallationViewModel = new UpdateInstallationViewModel
        {
            SelectedInstance = "TestInstance",
            SelectedRelease = _testRelease.Id,
        };

        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _installationManagerMock.Setup(m => m.GetInstallationsAsync()).ReturnsAsync(new[] { testInstallation });

        _releaseManagerMock.Setup(m => m.GetProductsAsync(_testCustomer.Id))
            .ReturnsAsync(new[] { _testProduct });
        _releaseManagerMock.Setup(m => m.GetReleasesAsync(_testCustomer.Id, _testProduct.Id))
            .ReturnsAsync(new[] { _testRelease });

        _serviceManagerMock.Setup(m => m.GetServiceName(updateInstallationViewModel.SelectedInstance))
            .Returns(updateInstallationViewModel.ServiceName);
        _serviceManagerMock.Setup(m => m.InstallService(It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string>((product, instance) =>
            {
                _ = product;
                _ = instance;
            });

        var result = await _controller.UpdateInstallation(updateInstallationViewModel);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<UpdateInstallationViewModel>(viewResult.Model);
    }

    [Fact]
    public async Task PostUpdateInstallationInvalidModelTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _releaseManagerMock.Setup(m => m.GetProductsAsync(_testCustomer.Id))
            .ReturnsAsync(new[] {_testProduct});
        _releaseManagerMock.Setup(m => m.GetReleasesAsync(_testCustomer.Id, _testProduct.Id))
            .ReturnsAsync(new[] {_testRelease});

        var updateInstallationViewModel = new UpdateInstallationViewModel
        {
            SelectedRelease = _testRelease.Id
        };

        var result = await _controller.UpdateInstallation(updateInstallationViewModel);

        var viewResult = Assert.IsType<ViewResult>(result);
        var installationOverviewModel = Assert.IsType<UpdateInstallationViewModel>(viewResult.Model);
        Assert.NotNull(installationOverviewModel);
        Assert.Single(installationOverviewModel.AvailableReleases);
        Assert.Null(installationOverviewModel.SelectedInstance);
        Assert.Equal(_testRelease.Id, installationOverviewModel.SelectedRelease);
    }

    [Fact]
    public async Task UpdateInstallationWithNotExistingReleaseTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _releaseManagerMock.Setup(m => m.GetProductsAsync(_testCustomer.Id))
            .ReturnsAsync(new[] {_testProduct});
        _releaseManagerMock.Setup(m => m.GetReleasesAsync(_testCustomer.Id, _testProduct.Id))
            .ReturnsAsync(new[] {_testRelease});

        var updateInstallationViewModel = new UpdateInstallationViewModel
        {
            SelectedInstance = "NewInstance",
            SelectedRelease = Guid.NewGuid()
        };

        var result = await _controller.UpdateInstallation(updateInstallationViewModel);

        var viewResult = Assert.IsType<ViewResult>(result);
        var newInstallationModel = Assert.IsType<UpdateInstallationViewModel>(viewResult.Model);
        Assert.NotEmpty(newInstallationModel.ErrorMessage);
    }

    [Fact]
    public async Task GetTransferNormalTest()
    {
        var testInstallation = new InstalledRelease
        {
            InstanceName = "TestInstance",
            ProductName = _testProduct.ProductName,
            Version = "TestVersion"
        };
        var prodInstallation = new InstalledRelease
        {
            InstanceName = "ProdInstance",
            ProductName = _testProduct.ProductName,
            Version = "ProdVersion"
        };

        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _installationManagerMock.Setup(m => m.GetInstallationsAsync())
            .ReturnsAsync(new[] { testInstallation, prodInstallation });

        var result = await _controller.Transfer(prodInstallation.InstanceName, prodInstallation.ProductName);

        var viewResult = Assert.IsType<ViewResult>(result);
        var transferInstallationViewModel = Assert.IsType<TransferInstallationViewModel>(viewResult.Model);
        var availableSourceInstance = Assert.Single(transferInstallationViewModel.AvailableSourceInstances);
        Assert.NotNull(availableSourceInstance);
        Assert.Equal(testInstallation.InstanceName, availableSourceInstance.InstanceName);
        Assert.Equal(testInstallation.ProductName, availableSourceInstance.ProductName);
        Assert.Equal(testInstallation.Version, availableSourceInstance.Version);
        Assert.Equal(testInstallation.ProductName, transferInstallationViewModel.ProductName);
        Assert.Equal(prodInstallation.InstanceName, transferInstallationViewModel.TargetInstanceName);
        Assert.Null(transferInstallationViewModel.TargetServiceName);
        Assert.Null(transferInstallationViewModel.SourceInstanceName);
        Assert.Null(transferInstallationViewModel.ErrorMessage);
    }

    [Fact]
    public async Task GetTransferWithExceptionTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _installationManagerMock.Setup(m => m.GetInstallationsAsync())
            .Returns(() => throw new Exception("TestError"));

        var result = await _controller.Transfer(string.Empty, string.Empty);

        var viewResult = Assert.IsType<ViewResult>(result);
        var updateInstallationModel = Assert.IsType<TransferInstallationViewModel>(viewResult.Model);
        Assert.Equal("TestError", updateInstallationModel.ErrorMessage);
    }

    [Fact]
    public async Task PostTransferInstallationNormalTest()
    {
        var testInstallation = new InstalledRelease
        {
            InstanceName = "TestInstance",
            ProductName = _testProduct.ProductName,
            Version = "TestVersion"
        };
        var prodInstallation = new InstalledRelease
        {
            InstanceName = "ProdInstance",
            ProductName = _testProduct.ProductName,
            Version = "ProdVersion"
        };
            
        string stoppedService = null;

        var transferInstallationViewModel = new TransferInstallationViewModel
        {
            ProductName = _testProduct.ProductName,
            SourceInstanceName = testInstallation.InstanceName,
            TargetInstanceName = prodInstallation.InstanceName,
            TargetServiceName = "TargetService"
        };

        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _installationManagerMock.Setup(m => m.GetInstallationsAsync())
            .ReturnsAsync(new[] { testInstallation, prodInstallation });

        _serviceManagerMock.Setup(m => m.GetServiceState(transferInstallationViewModel.TargetServiceName))
            .Returns(ServiceStatus.Running);
        _serviceManagerMock.Setup(m => m.StopService(It.IsAny<string>()))
            .Callback<string>(service => stoppedService = service);

        var result = await _controller.TransferInstallation(transferInstallationViewModel);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TransferInstallationViewModel>(viewResult.Model);

        Assert.NotNull(model);
        Assert.Null(model.ErrorMessage);
        Assert.NotNull(model.TransferJobId);
        Assert.Equal(_jobInstance.Id, model.TransferJobId);
        Assert.Equal(transferInstallationViewModel.TargetServiceName, stoppedService);
    }

    [Fact]
    public async Task PostTransferInstallationInvalidModelTest()
    {
        var testInstallation = new InstalledRelease
        {
            InstanceName = "TestInstance",
            ProductName = _testProduct.ProductName,
            Version = "TestVersion"
        };
        var prodInstallation = new InstalledRelease
        {
            InstanceName = "ProdInstance",
            ProductName = _testProduct.ProductName,
            Version = "ProdVersion"
        };

        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _installationManagerMock.Setup(m => m.GetInstallationsAsync())
            .ReturnsAsync(new[] { testInstallation, prodInstallation });

        var transferInstallationViewModel = new TransferInstallationViewModel
        {
            ProductName = _testProduct.ProductName,
            TargetInstanceName = prodInstallation.InstanceName
        };

        var result = await _controller.TransferInstallation(transferInstallationViewModel);

        var viewResult = Assert.IsType<ViewResult>(result);
        var resultModel = Assert.IsType<TransferInstallationViewModel>(viewResult.Model);
        Assert.NotNull(resultModel);
        Assert.Single(resultModel.AvailableSourceInstances);
        Assert.Null(resultModel.SourceInstanceName);
        Assert.Null(resultModel.ErrorMessage);
    }

    [Fact]
    public async Task PostTransferInstallationUnknownTargetTest()
    {
        var testInstallation = new InstalledRelease
        {
            InstanceName = "TestInstance",
            ProductName = _testProduct.ProductName,
            Version = "TestVersion"
        };

        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _installationManagerMock.Setup(m => m.GetInstallationsAsync())
            .ReturnsAsync(new[] { testInstallation });

        var transferInstallationViewModel = new TransferInstallationViewModel
        {
            ProductName = _testProduct.ProductName,
            TargetInstanceName = "NotExisting",
            SourceInstanceName = testInstallation.InstanceName
        };

        var result = await _controller.TransferInstallation(transferInstallationViewModel);

        var viewResult = Assert.IsType<ViewResult>(result);
        var resultModel = Assert.IsType<TransferInstallationViewModel>(viewResult.Model);
        Assert.NotNull(resultModel);
        Assert.NotEmpty(resultModel.ErrorMessage);
    }
}