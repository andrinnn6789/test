using System;
using System.Threading.Tasks;

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

using Xunit;

namespace IAG.InstallClient.Test.Controllers;

public class CustomerControllerTest
{
    private readonly CustomerController _controller;
    private readonly CustomerInfo _testCustomer;
    private readonly Mock<ICustomerManager> _customerManagerMock;


    public CustomerControllerTest()
    {
        _testCustomer = new CustomerInfo
        {
            Id = Guid.NewGuid(),
            CustomerName = "TestCustomer",
            Description = "TestDescription"
        };

        _customerManagerMock = new Mock<ICustomerManager>();
            
        var localizer = new MockLocalizer<CustomerController>();
        var httpContext = new DefaultHttpContext
        {
            Session = new MockSession()
        };

        _controller = new CustomerController(_customerManagerMock.Object, localizer)
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

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CustomerViewModel>(viewResult.Model);
        Assert.Null(model.CustomerId);
        Assert.Null(model.CustomerName);
        Assert.Null(model.Description);
        Assert.False(model.ForceEdit);
        Assert.Null(model.ErrorMessage);
    }

    [Fact]
    public async Task GetIndexWithCustomerTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CustomerViewModel>(viewResult.Model);
        Assert.Equal(_testCustomer.Id, model.CustomerId);
        Assert.Equal(_testCustomer.CustomerName, model.CustomerName);
        Assert.Equal(_testCustomer.Description, model.Description);
        Assert.False(model.ForceEdit);
        Assert.Null(model.ErrorMessage);
    }

    [Fact]
    public async Task GetIndexWithFaultyCustomerTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .Returns(() => throw new Exception("TestError"));

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CustomerViewModel>(viewResult.Model);
        Assert.Null(model.CustomerId);
        Assert.Null(model.CustomerName);
        Assert.Null(model.Description);
        Assert.False(model.ForceEdit);
        Assert.Equal("TestError", model.ErrorMessage);
    }

    [Fact]
    public async Task GetIndexWithSuccessfulReloadCustomerTest()
    {
        var attempt = 0;
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .Returns(() =>
            {
                if (attempt++ == 0)
                    throw new Exception("TestError");
                return Task.FromResult(_testCustomer);
            });

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CustomerViewModel>(viewResult.Model);
        Assert.Equal(_testCustomer.Id, model.CustomerId);
        Assert.Equal(_testCustomer.CustomerName, model.CustomerName);
        Assert.Equal(_testCustomer.Description, model.Description);
        Assert.False(model.ForceEdit);
        Assert.Null(model.ErrorMessage);
    }

    [Fact]
    public async Task PostIndexWithValidModelTest()
    {
        _customerManagerMock.Setup(m => m.GetCustomerInformationAsync(_testCustomer.Id))
            .ReturnsAsync(_testCustomer);

        var result = await _controller.Index(new CustomerViewModel { CustomerId = _testCustomer.Id });

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
    }

    [Fact]
    public async Task PostIndexWithInvalidGuidTest()
    {
        var result = await _controller.Index(new CustomerViewModel { CustomerId = null });

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CustomerViewModel>(viewResult.Model);
        Assert.NotEmpty(model.ErrorMessage);
    }

    [Fact]
    public async Task PostIndexWithGetCustomerErrorTest()
    {
        _customerManagerMock.Setup(m => m.GetCustomerInformationAsync(It.IsAny<Guid>()))
            .Returns(() => throw new Exception("TestError"));

        var result = await _controller.Index(new CustomerViewModel { CustomerId = Guid.Empty });

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CustomerViewModel>(viewResult.Model);
        Assert.Equal("TestError", model.ErrorMessage);
    }

    [Fact]
    public async Task GetEditTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        var result = await _controller.Edit();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CustomerViewModel>(viewResult.Model);
        Assert.True(model.ForceEdit);
    }
}