using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.ProcessEngine.Execution;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Plugin.TestJob.SimplestJob;
using IAG.ProcessEngine.Store;
using IAG.ProcessEngine.Store.Model;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.Test.Execution;

public class SchedulerTest
{
    private readonly Mock<IJobService> _mockJobService = new();
    private readonly Mock<IJobBuilder> _mockJobBuilder = new();
    private readonly List<IJobConfig> _mockConfigs = new();
        
    private IScheduler _scheduler;

    [Fact]
    public async Task StartStopTest()
    {
        BuildCatalogue(new Dictionary<Guid, string>());
        var stopwatch = new Stopwatch();

        _scheduler.Start();
        _scheduler.Start();
        await Task.Delay(200);
        Assert.True(_scheduler.IsRunning);
        stopwatch.Start();
        _scheduler.Stop();
        _scheduler.Stop();
        stopwatch.Stop();

        Assert.False(_scheduler.IsRunning);
        Assert.True(stopwatch.ElapsedMilliseconds < 50, "Scheduler had too long to stop");
    }

    [Fact]
    public async Task ReStartTest()
    {
        BuildCatalogue(new Dictionary<Guid, string>());
        var stopwatch1 = new Stopwatch();
        var stopwatch2 = new Stopwatch();

        _scheduler.Start();
        await Task.Delay(200);
        Assert.True(_scheduler.IsRunning);
        stopwatch1.Start();
        _scheduler.Stop();
        stopwatch1.Stop();
        await Task.Delay(200);
        _scheduler.Start();
        await Task.Delay(200);
        Assert.True(_scheduler.IsRunning);
        stopwatch2.Start();
        _scheduler.Stop();
        stopwatch2.Stop();

        Assert.False(_scheduler.IsRunning);
        Assert.True(stopwatch1.ElapsedMilliseconds < 50, "Scheduler had too long to stop");
        Assert.True(stopwatch2.ElapsedMilliseconds < 50, "Scheduler had too long to stop");
    }

    [Fact]
    public async Task ExecuteJobTest()
    {
        BuildCatalogue(new Dictionary<Guid, string>
        {
            { Guid.NewGuid(), "* * * * * ? *" },
            { Guid.NewGuid(), "* * * * * ? *" }, // every second
            { Guid.NewGuid(), "0 0 0 * 12 0" } // irrelevant
        });
        var callbackCounter = 0;
        var stopwatch = new Stopwatch();
        _scheduler.OnBeforeEnqueueJob += _ => { callbackCounter++; };

        Thread.Sleep(1050 - DateTime.UtcNow.Millisecond); // wait for "fresh" second
        _scheduler.Start();
        Thread.Sleep(2500);
        var curCallbackCounter = callbackCounter;
        stopwatch.Start();
        _scheduler.Stop();
        stopwatch.Stop();
        await Task.Delay(2500);

        Assert.Equal(4, curCallbackCounter);
        Assert.Equal(4, callbackCounter);
        Assert.True(stopwatch.ElapsedMilliseconds < 50, "Scheduler had too long to stop");
    }

    [Fact]
    public void ExecuteUtcJobTest()
    {
        BuildCatalogue(new Dictionary<Guid, string>
        {
            { Guid.NewGuid(), $"* * {DateTime.UtcNow.Hour} * * ? *" }
        });
        var callbackCounter = 0;
        _scheduler.OnBeforeEnqueueJob += _ => { callbackCounter++; };

        Thread.Sleep(1050 - DateTime.UtcNow.Millisecond); // wait for "fresh" second
        _scheduler.Start();
        Thread.Sleep(1500);
        _scheduler.Stop();

        Assert.Equal(0, callbackCounter);
    }

    [Fact]
    public void ExecuteLocalTimeJobTest()
    {
        BuildCatalogue(new Dictionary<Guid, string>
        {
            { Guid.NewGuid(), $"* * {DateTime.Now.Hour} * * ? *" }
        });
        var callbackCounter = 0;
        _scheduler.OnBeforeEnqueueJob += _ => { callbackCounter++; };

        Thread.Sleep(1050 - DateTime.UtcNow.Millisecond); // wait for "fresh" second
        _scheduler.Start();
        Thread.Sleep(1500);
        _scheduler.Stop();

        Assert.Equal(1, callbackCounter);
    }

    [Fact]
    public async Task PreAbortTest()
    {
        BuildCatalogue(new Dictionary<Guid, string>
        {
            { Guid.NewGuid(), $"{DateTime.UtcNow.AddSeconds(2).Second}/2 * * * * ? *" }    // every 2 seconds, start in 2 seconds
        });
        int callbackCounter = 0;
        var stopwatch = new Stopwatch();
        _scheduler.OnBeforeEnqueueJob += _ => { callbackCounter++; };
            
        _scheduler.Start();
        Thread.Sleep(500);
        var curCallbackCounter = callbackCounter;
        stopwatch.Start();
        _scheduler.Stop();
        stopwatch.Stop();
        await Task.Delay(4500);

        Assert.Equal(0, curCallbackCounter);
        Assert.Equal(0, callbackCounter);
        Assert.True(stopwatch.ElapsedMilliseconds < 50, "Scheduler had too long to stop");
    }

    [Fact]
    public async Task NoJobTest()
    {
        BuildCatalogue(new Dictionary<Guid, string>());
        int callbackCounter = 0;
        var stopwatch = new Stopwatch();
        _scheduler.OnBeforeEnqueueJob += _ => { callbackCounter++; };

        _scheduler.Start();
        await Task.Delay(1000);
        var curCallbackCounter = callbackCounter;
        stopwatch.Start();
        _scheduler.Stop();
        stopwatch.Stop();
        await Task.Delay(1000);

        Assert.Equal(0, curCallbackCounter);
        Assert.Equal(0, callbackCounter);
        Assert.True(stopwatch.ElapsedMilliseconds < 50, "Scheduler had too long to stop");
    }

    [Fact]
    public async Task NoScheduledJobTest()
    {
        BuildCatalogue(new Dictionary<Guid, string>
        {
            { Guid.NewGuid(), string.Empty }
        });
        int callbackCounter = 0;
        var stopwatch = new Stopwatch();
        _scheduler.OnBeforeEnqueueJob += _ => { callbackCounter++; };

        _scheduler.Start();
        await Task.Delay(1000);
        var curCallbackCounter = callbackCounter;
        stopwatch.Start();
        _scheduler.Stop();
        stopwatch.Stop();
        await Task.Delay(1000);

        Assert.Equal(0, curCallbackCounter);
        Assert.Equal(0, callbackCounter);
        Assert.True(stopwatch.ElapsedMilliseconds < 50, "Scheduler had too long to stop");
    }

    private void BuildCatalogue(Dictionary<Guid, string> idCronSet)
    {
        foreach (var entry in idCronSet)
        {
            AddJob(entry.Key, entry.Value);
        }

        _mockJobService.Setup(m => m.CreateJobInstance(It.IsAny<Guid>(), null))
            .Returns<Guid, IJobState>((id, _) => _mockJobBuilder.Object.BuildInstance(id, null));

        var mockJobConfigStore = new Mock<IJobConfigStore>();
        mockJobConfigStore.Setup(m => m.GetAll()).Returns(_mockConfigs);

        var serviceScope = SchedulerBaseTest.CreateServiceProvider(_mockJobService.Object, new Mock<IJobStore>().Object, mockJobConfigStore.Object);

        _scheduler = new Scheduler(serviceScope.ServiceProvider, new MockILogger<Scheduler>());
    }

    private void AddJob(Guid templateId, string cronExpression)
    {
        var jobMeta = new Mock<IJobMetadata>();
        jobMeta.Setup(e => e.TemplateId).Returns(templateId);
        jobMeta.Setup(e => e.ConfigType).Returns(typeof(SimplestConfig));

        var mockConfig = new Mock<IJobConfig>();
        mockConfig.Setup(c => c.CronExpression).Returns(cronExpression);
        mockConfig.Setup(c => c.Id).Returns(templateId);
        mockConfig.Setup(c => c.Active).Returns(true);
        mockConfig.Setup(c => c.TemplateId).Returns(templateId);

        var mockJob = new Mock<IJob>();
        mockJob.Setup(j => j.Config).Returns(mockConfig.Object);

        var mockJobInstance = new Mock<IJobInstance>();
        mockJobInstance.Setup(m => m.Job).Returns(mockJob.Object);

        _mockConfigs.Add(mockConfig.Object);
        _mockJobBuilder.Setup(m => m.BuildInstance(templateId, null)).Returns(mockJobInstance.Object);
    }
}