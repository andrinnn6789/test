using System;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.TestHelper.Globalization.Mocks;
using IAG.Infrastructure.TestHelper.Session;
using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;

using Moq;

using Xunit;

namespace IAG.InstallClient.Test.Controllers;

public class BaseControllerTest
{
    private readonly CustomerInfo _testCustomer;
    private readonly Mock<ICustomerManager> _customerManagerMock;
    private readonly IStringLocalizer _localizer;

    public BaseControllerTest()
    {
        _testCustomer = new CustomerInfo
        {
            Id = Guid.NewGuid(),
            CustomerName = "TestCustomer",
            Description = "TestDescription"
        };

        _customerManagerMock = new Mock<ICustomerManager>();
        _localizer = new MockLocalizer<TestController>();
    }

    [Fact]
    public void CustomerHandlingTest()
    {
        var getCurrentCustomerInfoCounter = 0;
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(() =>
            {
                getCurrentCustomerInfoCounter++;
                return _testCustomer;
            });

        var httpContext = new DefaultHttpContext
        {
            Session = new MockSession()
        };
        var controller = new TestController(httpContext, _customerManagerMock.Object, _localizer);

        var customerInit = controller.CustomerWrapper;
        var _ = controller.CustomerWrapper; // just another access

        _testCustomer.Description = "Updated Description";
        controller.CustomerWrapper = _testCustomer;
        var customerUpdated = controller.CustomerWrapper;

        var newController = new TestController(httpContext, _customerManagerMock.Object, _localizer);
        var customerNewController = newController.CustomerWrapper;

        Assert.NotNull(customerInit);
        Assert.NotNull(customerUpdated);
        Assert.NotNull(customerNewController);
        Assert.Equal(1, getCurrentCustomerInfoCounter); // only one call, after that from local field or session!
        Assert.Equal(_testCustomer.Id, customerInit.Id);
        Assert.Equal(_testCustomer.CustomerName, customerInit.CustomerName);
        Assert.Equal("Updated Description", customerUpdated.Description);
        Assert.Equal(_testCustomer.Id, customerNewController.Id);
        Assert.Equal(_testCustomer.CustomerName, customerNewController.CustomerName);
        Assert.Equal("Updated Description", customerNewController.Description);
        Assert.NotNull(controller.MessageLocalizerWrapper);
    }

    [Fact]
    public void GetCustomerExceptionTest()
    {
        var getCurrentCustomerInfoCounter = 0;
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .Returns(() =>
            {
                getCurrentCustomerInfoCounter++;
                throw new Exception();
            });

        var httpContext = new DefaultHttpContext
        {
            Session = new MockSession()
        };
        var controller = new TestController(httpContext, _customerManagerMock.Object, _localizer);

        var customerInit = controller.CustomerWrapper;
        var customerSecondAccess = controller.CustomerWrapper;

        Assert.Null(customerInit);
        Assert.Null(customerSecondAccess);
        Assert.Equal(1, getCurrentCustomerInfoCounter); // only one call, after that from local field or session!
    }

    [Fact]
    public void ForwardingTest()
    {
        var httpContext = new DefaultHttpContext
        {
            Session = new MockSession()
        };
        var controller = new TestController(httpContext, _customerManagerMock.Object, _localizer);

        controller.CustomerWrapper = null;
        var withoutCustomerResult = controller.ViewCustomerCheckedWrapper();

        controller.CustomerWrapper = _testCustomer;
        var withCustomerResult = controller.ViewCustomerCheckedWrapper();

        var redirectToCustomerResult = Assert.IsType<RedirectToActionResult>(withoutCustomerResult);
        var viewResult = Assert.IsType<ViewResult>(withCustomerResult);

        Assert.Equal("Customer", redirectToCustomerResult.ControllerName);
        Assert.Equal(nameof(CustomerController.Index), redirectToCustomerResult.ActionName);
        Assert.NotNull(viewResult);
    }


    private class TestController : BaseController
    {
        public CustomerInfo CustomerWrapper
        {
            get => Customer;
            set => Customer = value;
        }

        public IActionResult ViewCustomerCheckedWrapper() => ViewCustomerChecked(null);

        public IMessageLocalizer MessageLocalizerWrapper => MessageLocalizer;

        public TestController(HttpContext httpContext, ICustomerManager customerManager, IStringLocalizer stringLocalizer) : base(customerManager, stringLocalizer)
        {
            ControllerContext = new ControllerContext(new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor()));
        }
    }
}