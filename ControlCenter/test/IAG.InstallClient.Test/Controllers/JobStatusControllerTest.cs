using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.TestHelper.Globalization.Mocks;
using IAG.Infrastructure.TestHelper.Session;
using IAG.InstallClient.Controllers;
using IAG.ProcessEngine.Execution;
using IAG.ProcessEngine.Execution.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using Xunit;

namespace IAG.InstallClient.Test.Controllers;

public class JobStatusControllerTest
{
    private readonly JobStatusController _controller;
    private readonly IJobInstance _jobInstance;

    public JobStatusControllerTest()
    {
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

        _controller = new JobStatusController(jobServiceMock.Object, localizer)
        {
            ControllerContext = new ControllerContext(new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor()))
        };
    }

    [Fact]
    public void GetJobStatusTest()
    {
        var result = _controller.GetJobStatus(_jobInstance.Id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult);
        Assert.NotNull(okResult.Value);
    }
}