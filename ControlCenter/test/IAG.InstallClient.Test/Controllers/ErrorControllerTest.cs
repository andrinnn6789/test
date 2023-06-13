using IAG.InstallClient.Controllers;
using IAG.InstallClient.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

using Xunit;

namespace IAG.InstallClient.Test.Controllers;

public class ErrorControllerTest
{
    [Fact]
    public void GetIndexTest()
    {
        var controller = new ErrorController
        {
            ControllerContext = new ControllerContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ControllerActionDescriptor()))
        };

        var result = controller.Error();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
        Assert.NotNull(model.RequestId);
        Assert.NotEmpty(model.RequestId);
    }
}