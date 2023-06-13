using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.TestHelper.xUnit;
using IAG.ProcessEngine.Execution;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Store;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.Test.Execution;

public class MonitorTest
{
    private readonly List<IJobState> _jobStore = new();
    private readonly List<IJobInstance> _jobsExecuted = new();
    private readonly IMonitor _monitor;

    public MonitorTest()
    {
        var mockJobStore = new Mock<IJobStore>();
        mockJobStore.Setup(m => m.Insert(It.IsAny<IJobState>())).Callback<IJobState>(state => _jobStore.Add(state));
        mockJobStore.Setup(m => m.GetJobs()).Returns(() => _jobStore.AsQueryable());
        mockJobStore.Setup(m => m.GetJobCount(null)).Returns(() => _jobStore.Count);


        var mockJobService = new Mock<IJobService>();
        var mockServiceScope = SchedulerBaseTest.CreateServiceProvider(mockJobService.Object, mockJobStore.Object, null);

        mockJobService.Setup(m => m.CreateJobInstance(It.IsAny<IJobState>())).Returns<IJobState>((state) => new JobInstance(mockServiceScope) { State = state });
        mockJobService.Setup(m => m.EnqueueJob(It.IsAny<IJobInstance>())).Callback<IJobInstance>((inst) => _jobsExecuted.Add(inst));

        _monitor = new Monitor(mockServiceScope.ServiceProvider, new MockILogger<Monitor>());
    }

    [Fact]
    public async Task StartStopTest()
    {
        var stopwatch = new Stopwatch();

        _monitor.Start();
        _monitor.Start();
        await Task.Delay(200);
        Assert.True(_monitor.IsRunning);
        stopwatch.Start();
        _monitor.Stop();
        _monitor.Stop();
        stopwatch.Stop();

        Assert.False(_monitor.IsRunning);
        Assert.True(stopwatch.ElapsedMilliseconds < 50, "Monitor had too long to stop");
    }

    [Fact]
    public async Task ReStartTest()
    {
        var stopwatch1 = new Stopwatch();
        var stopwatch2 = new Stopwatch();

        _monitor.Start();
        await Task.Delay(200);
        Assert.True(_monitor.IsRunning);
        stopwatch1.Start();
        _monitor.Stop();
        stopwatch1.Stop();
        await Task.Delay(200);
        _monitor.Start();
        await Task.Delay(200);
        Assert.True(_monitor.IsRunning);
        stopwatch2.Start();
        _monitor.Stop();
        stopwatch2.Stop();

        Assert.False(_monitor.IsRunning);
        Assert.True(stopwatch1.ElapsedMilliseconds < 50, "Monitor had too long to stop");
        Assert.True(stopwatch2.ElapsedMilliseconds < 50, "Monitor had too long to stop");
    }

    [Fact]
    public async Task ExecuteJobTest()
    {
        var stopwatch = new Stopwatch();
        var dueDate = DateTime.UtcNow.AddMilliseconds(500);
        var jobState = new JobState() { DateDue = dueDate, TemplateId = Guid.NewGuid() };
        _jobStore.Add(jobState);

        _monitor.Start();
        await Task.Delay(1000);
        stopwatch.Start();
        _monitor.Stop();
        stopwatch.Stop();

        Assert.Single(_jobsExecuted);
        Assert.Equal(jobState, _jobsExecuted[0].State);
        Assert.True(stopwatch.ElapsedMilliseconds < 50, "Monitor had too long to stop");
    }

    [Fact]
    public async Task ExecuteConcurrentJobsTest()
    {
        var stopwatch = new Stopwatch();
        var dueDate = DateTime.UtcNow.AddMilliseconds(500);
        var jobState1 = new JobState() { DateDue = dueDate, TemplateId = Guid.NewGuid() };
        var jobState2 = new JobState() { DateDue = dueDate, TemplateId = Guid.NewGuid() };
        _jobStore.Add(jobState1);
        _jobStore.Add(jobState2);

        _monitor.Start();
        await Task.Delay(1000);
        stopwatch.Start();
        _monitor.Stop();
        stopwatch.Stop();

        Assert.Equal(2, _jobsExecuted.Count);
        Assert.Equal(jobState1, _jobsExecuted[0].State);
        Assert.Equal(jobState2, _jobsExecuted[1].State);
        Assert.True(stopwatch.ElapsedMilliseconds < 50, "Monitor had too long to stop");
    }

    [Fact]
    public async Task PreAbortTest()
    {
        var stopwatch = new Stopwatch();
        var dueDate = DateTime.UtcNow.AddMilliseconds(10000);
        var jobState = new JobState() { DateDue = dueDate, TemplateId = Guid.NewGuid() };
        _jobStore.Add(jobState);

        _monitor.Start();
        await Task.Delay(500);
        stopwatch.Start();
        _monitor.Stop();
        stopwatch.Stop();

        Assert.Empty(_jobsExecuted);
        Assert.True(stopwatch.ElapsedMilliseconds < 50, "Monitor had too long to stop");
    }

    [Fact]
    public async Task NoJobTest()
    {
        var stopwatch = new Stopwatch();

        _monitor.Start();
        await Task.Delay(500);
        stopwatch.Start();
        _monitor.Stop();
        stopwatch.Stop();

        Assert.Empty(_jobsExecuted);
        Assert.True(stopwatch.ElapsedMilliseconds < 50, "Monitor had too long to stop");
    }

    [Fact]
    public async Task OnlyIrrelevantTest()
    {
        var jobId = Guid.NewGuid();
        var stopwatch = new Stopwatch();
        _jobStore.Add(new JobState { TemplateId = jobId });
        _jobStore.Add(new JobState { DateDue = DateTime.UtcNow.AddDays(1), TemplateId = jobId });

        _monitor.Start();
        await Task.Delay(500);
        stopwatch.Start();
        _monitor.Stop();
        stopwatch.Stop();

        Assert.Empty(_jobsExecuted);
        Assert.True(stopwatch.ElapsedMilliseconds < 50, "Monitor had too long to stop");
    }
}