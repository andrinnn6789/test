using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;
using IAG.ProcessEngine.Controller;
using IAG.ProcessEngine.Execution;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Store;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

using Newtonsoft.Json.Linq;

using Xunit;

namespace IAG.ProcessEngine.Test.Controller;

public class JobServiceControllerTest
{
    private const string JobName = "TestJob";

    private readonly JobServiceController _controller;
    private readonly Guid _jobId;
    private readonly Guid _jobInstanceId = Guid.NewGuid();
    private readonly List<IJobConfig> _configStore;

    public JobServiceControllerTest()
    {
        var mockJobService = new Mock<IJobService>();
        var mockJobInstance = new Mock<IJobInstance>();
        var mockJobState = new Mock<IJobState>();
        var localizer = new DbStringLocalizer<JobServiceController>(new SortedList<string, string>());
        var job = new HelloJob();
        var jobConfig = new HelloConfig {Name = JobName};
        _configStore = new List<IJobConfig> {jobConfig};

        _jobId = jobConfig.Id;
        mockJobState.SetupAllProperties();
        mockJobState.Object.Result = new JobResult();
        mockJobState.Object.Parameter = new JobParameter();
        mockJobState.Setup(m => m.Messages).Returns(new List<MessageStructure>());
        mockJobInstance.Setup(m => m.Id).Returns(_jobInstanceId);
        mockJobInstance.Setup(m => m.State).Returns(mockJobState.Object);
        mockJobInstance.Setup(m => m.Job).Returns(job);
        mockJobService.Setup(m => m.CreateJobInstance(It.Is<Guid>(id => id == _jobId), null)).Returns(mockJobInstance.Object);
        mockJobService.Setup(m => m.CreateJobInstance(It.Is<Guid>(id => id != _jobId), null))
            .Returns<Guid, IJobState>((x, _) => throw new NotFoundException(x.ToString()));
        mockJobService.Setup(m => m.GetJobParameter(It.Is<Guid>(id => id == _jobId))).Returns(job.Parameter);
        mockJobService.Setup(m => m.GetJobParameter(It.Is<Guid>(id => id != _jobId)))
            .Returns<Guid>(x => throw new NotFoundException(x.ToString()));
        mockJobService.Setup(m => m.GetJobInstanceState(It.Is<Guid>(id => id == _jobInstanceId))).Returns(mockJobState.Object);
        mockJobService.Setup(m => m.GetJobInstanceState(It.Is<Guid>(id => id != _jobInstanceId)))
            .Returns<Guid>(x => throw new NotFoundException(x.ToString()));
        var mockInstance = new Mock<IJobInstance>();
        mockInstance.Setup(m => m.State).Returns(new JobState());
        mockJobService.Setup(m => m.RunningJobs).Returns(() => new List<IJobInstance>
        {
            mockInstance.Object

        });
        mockJobService.Setup(m => m.AbortJob(It.IsAny<Guid>())).Returns<Guid>(id => id == _jobInstanceId);
        var mockConfigStore = new Mock<IJobConfigStore>();
        mockConfigStore.Setup(m => m.GetAll()).Returns(_configStore);
        mockConfigStore.Setup(m => m.Insert(It.IsAny<IJobConfig>())).Callback<IJobConfig>(c => _configStore.Add(c));
        mockConfigStore.Setup(m => m.GetOrCreateJobConfig(It.Is<string>(c => c == JobName))).Returns(jobConfig);
        mockConfigStore.Setup(m => m.GetOrCreateJobConfig(It.Is<string>(c => c == "UnknownJobName"))).Throws(new NotFoundException("xx"));

        _controller = new JobServiceController(mockJobService.Object, mockConfigStore.Object, localizer)
        {
            ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()}
        };
    }

    [Fact]
    public void Running()
    {
        var result = _controller.Running();

        Assert.NotNull(result);
        Assert.False(result.Value);
    }

    [Fact]
    public void RunningJobs()
    {
        var result = _controller.RunningJobs();

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value);
    }

    [Fact]
    public void GetJobParameter()
    {
        var result = _controller.GetJobParameter(_jobId);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<HelloParameter>(result.Value);
    }

    [Fact]
    public void GetJobParameterNotFound()
    {
        Assert.Throws<NotFoundException>(() => _controller.GetJobParameter(Guid.NewGuid()));
    }

    [Fact]
    public void GetJobInstanceState()
    {
        var result = _controller.GetJobInstanceState(_jobInstanceId);

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value.LocalizedMessages);
    }

    [Fact]
    public void GetJobInstanceStateNotFound()
    {
        Assert.Throws<NotFoundException>(() => _controller.GetJobInstanceState(Guid.NewGuid()));
    }

    [Fact]
    public async Task ExecuteWithoutParam()
    {
        var result = await _controller.Execute(_jobId, null);

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value.LocalizedMessages);
    }

    [Fact]
    public async Task ExecuteWithParam()
    {
        var jobParam = new HelloParameter { GreetingsFrom = "Controller Test" };
        var result = await _controller.Execute(_jobId, JObject.FromObject(jobParam));

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value.LocalizedMessages);
    }

    [Fact]
    public async Task ExecuteNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.Execute(Guid.NewGuid(), null));
    }

    [Fact]
    [Obsolete]
    public async Task ExecuteJobWithoutParam()
    {
        var result = await _controller.ExecuteJob(JobName, null);

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value.LocalizedMessages);
    }

    [Fact]
    public async Task ExecuteByNameWithoutParam()
    {
        var result = await _controller.ExecuteByName(JobName, null);

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value.LocalizedMessages);
    }

    [Fact]
    public async Task ExecuteByNameWithoutConfig()
    {
        _configStore.Clear();
        var result = await _controller.ExecuteByName(JobName, null);

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value.LocalizedMessages);
    }

    [Fact]
    public void EnqueueWithoutParam()
    {
        var result = _controller.Enqueue(_jobId, null);

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public void EnqueueWithParam()
    {
        var jobParam = new HelloParameter { GreetingsFrom = "Controller Test" };
        var result = _controller.Enqueue(_jobId, JObject.FromObject(jobParam));

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public void EnqueueNotFound()
    {
        Assert.Throws<NotFoundException>(() => _controller.Enqueue(Guid.NewGuid(), null));
    }

    [Fact]
    public void AbortJob()
    {
        var result = _controller.AbortJob(_jobInstanceId);

        Assert.NotNull(result);
        Assert.True(result.Value);
    }

    [Fact]
    public void AbortJobNotFound()
    {
        var result = _controller.AbortJob(Guid.NewGuid());

        Assert.NotNull(result);
        Assert.False(result.Value);
    }

    [Fact]
    public void Start()
    {
        var result = _controller.Start();

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void Stop()
    {
        var result = _controller.Stop();

        Assert.IsType<NoContentResult>(result);
    }
}