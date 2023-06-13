using Microsoft.AspNetCore.Mvc;

using IAG.Infrastructure.Controllers;

using Xunit;

namespace IAG.Infrastructure.Test.Controllers;

public class DocsControllerTest
{
    private readonly DocsController _controller;

    public DocsControllerTest()
    {
        _controller = new DocsController();
    }
    
    [Fact]
    public void Index_WhenExecuted_ShouldReturnIndexPage()
    {
        // Arrange
        var result = _controller.Index();

        // Act & Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", viewResult.ViewName);
    }
}