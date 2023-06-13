using System;
using System.Collections.Generic;
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

public class LinkListControllerTest
{
    private readonly LinkListController _controller;
    private readonly CustomerInfo _testCustomer;
    private readonly Mock<ICustomerManager> _customerManagerMock;
    private readonly Mock<ILinkListManager> _linkListManagerMock;


    public LinkListControllerTest()
    {
        _testCustomer = new CustomerInfo
        {
            Id = Guid.NewGuid(),
            CustomerName = "TestCustomer",
            Description = "TestDescription"
        };

        _linkListManagerMock = new Mock<ILinkListManager>();
        _customerManagerMock = new Mock<ICustomerManager>();
           
        var localizer = new MockLocalizer<LinkListController>();
        var httpContext = new DefaultHttpContext
        {
            Session = new MockSession()
        };
        httpContext.Session.Set("UpdateCheck.Done", new byte[] { 1 });

        _controller = new LinkListController(_linkListManagerMock.Object, _customerManagerMock.Object, localizer)
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
        var testLink = new LinkInfo
        {
            Name = "TestLink",
            Url = "https://test.link",
            Description = "TestDescription"
        };
        _linkListManagerMock.Setup(m => m.GetLinksAsync(_testCustomer.Id))
            .ReturnsAsync(new List<LinkInfo> {testLink});

        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var overviewModel = Assert.IsType<LinkListViewModel>(viewResult.Model);
        var singleLinkModel = Assert.Single(overviewModel.Links);
        Assert.NotNull(singleLinkModel);
        Assert.Equal(testLink.Name, singleLinkModel.Name);
        Assert.Equal(testLink.Url, singleLinkModel.Url);
        Assert.Equal(testLink.Description, singleLinkModel.Description);
        Assert.Null(overviewModel.ErrorMessage);
    }

    [Fact]
    public async Task GetIndexWithExceptionTest()
    {
        _customerManagerMock.Setup(m => m.GetCurrentCustomerInformationAsync())
            .ReturnsAsync(_testCustomer);

        _linkListManagerMock.Setup(m => m.GetLinksAsync(_testCustomer.Id))
            .ReturnsAsync(() => throw new Exception("TestError"));

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var overviewModel = Assert.IsType<LinkListViewModel>(viewResult.Model);
        Assert.Empty(overviewModel.Links);
        Assert.Equal("TestError", overviewModel.ErrorMessage);
    }
}