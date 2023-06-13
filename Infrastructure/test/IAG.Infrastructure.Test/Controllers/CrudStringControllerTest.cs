using IAG.Infrastructure.Exception.HttpException;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Xunit;

namespace IAG.Infrastructure.Test.Controllers;

public class CrudStringControllerTest
{
    private readonly CrudStringController _controller;

    public CrudStringControllerTest()
    {
        _controller = new CrudStringController {ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()}};
    }

    [Fact]
    public void CreateFailsTest()
    {
        var result = _controller.Create(null);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void DeleteFailsTest()
    {
        Assert.Throws<NotFoundException>(() => _controller.Delete("xxx"));
    }

    [Fact]
    public void GetFailsTest()
    {
        Assert.Throws<NotFoundException>(() => _controller.GetById("xxx"));
    }

    [Fact]
    public void UpdateFailsTest()
    {
        var insertedItem = new StringKey { Id = "123" };
        _controller.Create(insertedItem);
        var result = _controller.Update("xxx", null);
        insertedItem = new StringKey { Id = "xxx" };

        Assert.IsType<BadRequestResult>(result);
        Assert.Throws<NotFoundException>(() => _controller.Update("xxx", insertedItem));
    }

    [Fact]
    public void CrudTest()
    {
        var insertedItem = new StringKey { Id = "123" };

        // add
        var result = _controller.Create(insertedItem);
        var resultCreated = Assert.IsType<CreatedResult>(result);
        var jobState = Assert.IsAssignableFrom<StringKey>(resultCreated.Value);
        Assert.Equal(insertedItem.Name, jobState.Name);
        Assert.Throws< DuplicateKeyException>(() => _controller.Create(insertedItem));

        // update
        jobState.Name = "update";
        result = _controller.Update(jobState.Id, jobState);
        Assert.IsType<NoContentResult>(result);

        // read
        var resultGetById = _controller.GetById(jobState.Id);
        Assert.NotNull(resultGetById?.Value);
        Assert.Equal(jobState.Name, resultGetById?.Value.Name);

        // delete
        result = _controller.Delete(jobState.Id);
        Assert.IsType<NoContentResult>(result);
    }
}