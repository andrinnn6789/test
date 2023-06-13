using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.TestHelper.Globalization.Mocks;
using IAG.ProcessEngine.Controller;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Store;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.Test.Controller;

public class JobStoreControllerTest
{
    private readonly JobStoreController _controller;
    private readonly IJobState _jobState;

    public JobStoreControllerTest()
    {
        _jobState = new JobState
        {
            Id = Guid.NewGuid()
        };
        var jobStore = new List<IJobState> { _jobState };
        var mockJobStore = new Mock<IJobStore>();
        mockJobStore.Setup(m => m.GetJobs()).Returns(() => jobStore.AsQueryable());
        mockJobStore.Setup(m => m.Get(It.IsAny<Guid>())).Returns<Guid>(id => jobStore.Find(state => state.Id == id));
        mockJobStore.Setup(m => m.GetJobCount(null)).Returns(() => jobStore.Count);

        _controller = new JobStoreController(mockJobStore.Object, new MockLocalizer<JobStoreController>())
        {
            ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()}
        };
    }

    [Fact]
    public void DeleteOldJobs()
    {
        var result = _controller.DeleteOldJobs(1, 1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void DeleteScheduledJobs()
    {
        var result = _controller.DeleteScheduledJobs();

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void GetJobs()
    {
        var result = _controller.GetJobs();

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value);
    }

    [Fact]
    public void GetJobLogs()
    {
        var result = _controller.GetJobLogs();

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value);
    }

    [Fact]
    public void GetJobFail()
    {
        Assert.Throws<NotFoundException>(() => _controller.GetJobs(Guid.NewGuid()));
    }

    [Fact]
    public void GetJob()
    {
        _controller.GetJobs(_jobState.Id);
    }

    [Fact]
    public void GetJobFailLog()
    {
        Assert.Throws<NotFoundException>(() => _controller.GetJobLog(Guid.NewGuid()));
    }

    [Fact]
    public void GetJobLog()
    {
        _controller.GetJobLog(_jobState.Id);
    }

    [Fact]
    public void GetJobCount()
    {
        var result = _controller.GetJobCount();

        Assert.Equal(1, result.Value);
    }
}