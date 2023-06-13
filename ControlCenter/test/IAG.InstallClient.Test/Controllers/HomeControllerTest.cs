using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.TestHelper.Globalization.Mocks;
using IAG.Infrastructure.TestHelper.Session;
using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.Controllers;
using IAG.InstallClient.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;

using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Execution;
using IAG.ProcessEngine.Execution.Model;

using Xunit;

namespace IAG.InstallClient.Test.Controllers;

public class HomeControllerTest
{
    private const string CurrentSelfVersion = "CurrentSelfVersion";

    private readonly HomeController _controller;
    private readonly CustomerInfo _testCustomer;
    private readonly Mock<ICustomerManager> _customerManagerMock;
    private readonly Mock<IJobService> _jobServiceMock;

    public HomeControllerTest()
    {
        _testCustomer = new CustomerInfo
        {
            Id = Guid.NewGuid(),
            CustomerName = "TestCustomer",
            Description = "TestDescription"
        };

        var installationManagerMock = new Mock<IInstallationManager>();
        installationManagerMock.Setup(m => m.CurrentSelfVersion).Returns(CurrentSelfVersion);

        _customerManagerMock = new Mock<ICustomerManager>();

        _jobServiceMock = new Mock<IJobService>();

        var localizer = new MockLocalizer<HomeController>();
        var httpContext = new DefaultHttpContext
        {
            Session = new MockSession()
        };

        _controller = new HomeController(installationManagerMock.Object, _customerManagerMock.Object, _jobServiceMock.Object, localizer)
        {
            ControllerContext = new ControllerContext(new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor()))
        };
    }

    [Fact]
    public void GetIndexWithoutCustomerTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync((CustomerInfo)null);

        var result = _controller.Index();

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Customer", redirectResult.ControllerName);
        Assert.Equal(nameof(CustomerController.Index), redirectResult.ActionName);
    }

    [Fact]
    public void GetIndexWithCustomerTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        var result = _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<HomeViewModel>(viewResult.Model);
        Assert.NotNull(model);
        Assert.NotNull(model.CurrentReleaseVersion);
        Assert.NotEmpty(model.CurrentReleaseVersion);
        Assert.Null(model.SelfUpdateJobId);
        Assert.Null(model.ErrorMessage);
    }

    [Fact]
    public void StartSelfUpdateTest()
    {
        var jobInstance = new JobInstance(null)
        {
            State = new JobState(),
            Job = new Mock<IJob>().Object
        };
        _jobServiceMock.Setup(m => m.CreateJobInstance(It.IsAny<Guid>(), It.IsAny<IJobState>()))
            .Returns(jobInstance);

        var result = _controller.StartSelfUpdate();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<HomeViewModel>(viewResult.Model);
        Assert.NotNull(model);
        Assert.NotNull(model.CurrentReleaseVersion);
        Assert.NotEmpty(model.CurrentReleaseVersion);
        Assert.True(model.SelfUpdateJobId.HasValue);
        Assert.Equal(jobInstance.Id, model.SelfUpdateJobId.Value);
        Assert.Null(model.ErrorMessage);
    }

    [Fact]
    public void StartSelfUpdateFailureTest()
    {
        var result = _controller.StartSelfUpdate();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<HomeViewModel>(viewResult.Model);
        Assert.NotNull(model);
        Assert.NotNull(model.CurrentReleaseVersion);
        Assert.NotEmpty(model.CurrentReleaseVersion);
        Assert.Null(model.SelfUpdateJobId);
        Assert.NotNull(model.ErrorMessage);
        Assert.NotEmpty(model.ErrorMessage);
    }
}