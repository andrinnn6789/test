using System;
using System.Linq;
using System.Threading.Tasks;

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

public class LoginControllerTest
{
    private readonly LoginController _controller;

    public LoginControllerTest()
    {
        var loginManagerMock = new Mock<ILoginManager>();
        loginManagerMock.Setup(m => m.DoLoginAsync(It.IsAny<string>(), "WrongPassword"))
            .ThrowsAsync(new Exception("WrongPasswordException"));
        loginManagerMock.Setup(m => m.DoLoginAsync(It.IsAny<string>(), "CorrectPassword"))
            .ReturnsAsync("ValidBearerToken");

        var customerManagerMock = new Mock<ICustomerManager>();
        var localizer = new MockLocalizer<HomeController>();
        var httpContext = new DefaultHttpContext
        {
            Session = new MockSession()
        };

        _controller = new LoginController(customerManagerMock.Object, loginManagerMock.Object, localizer)
        {
            ControllerContext = new ControllerContext(new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor()))
        };
           
    }

    [Fact]
    public void GetIndexTest()
    {
        var result = _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<LoginViewModel>(viewResult.Model);
        Assert.NotNull(model);
        Assert.Null(model.Username);
        Assert.Null(model.Password);
        Assert.Null(model.ErrorMessage);
    }

    [Fact]
    public async Task DoLoginWithCorrectPasswordSucceedTest()
    {
        var result = await _controller.DoLogin(new LoginViewModel() { Password = "CorrectPassword"});
        var tokenCookie = _controller.HttpContext.Response.GetTypedHeaders().SetCookie.FirstOrDefault();
            
        var actionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.NotNull(actionResult);
        Assert.Equal("Index", actionResult.ActionName);
        Assert.Equal("Home", actionResult.ControllerName);
        Assert.NotNull(tokenCookie);
        Assert.Equal("ValidBearerToken", tokenCookie.Value);
    }

    [Fact]
    public async Task DoLoginWithWrongPasswordFailsTest()
    {
        var result = await _controller.DoLogin(new LoginViewModel() { Username = "TestUser", Password = "WrongPassword" });
        var tokenCookie = _controller.HttpContext.Response.GetTypedHeaders().SetCookie.FirstOrDefault();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<LoginViewModel>(viewResult.Model);
        Assert.NotNull(model);
        Assert.Equal("TestUser", model.Username);
        Assert.Null(model.Password);
        Assert.Contains("WrongPasswordException", model.ErrorMessage);
        Assert.Null(tokenCookie);
    }


    [Fact]
    public void DoLogoutTest()
    {
        var result = _controller.Logout();
        var tokenCookie = _controller.HttpContext.Response.GetTypedHeaders().SetCookie.FirstOrDefault();

        var actionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.NotNull(actionResult);
        Assert.Equal("Index", actionResult.ActionName);
        Assert.Null(actionResult.ControllerName);
        Assert.NotNull(tokenCookie);
        Assert.True(DateTime.UtcNow > tokenCookie.Expires);
        Assert.Equal(0, tokenCookie.Value.Length);
    }
}