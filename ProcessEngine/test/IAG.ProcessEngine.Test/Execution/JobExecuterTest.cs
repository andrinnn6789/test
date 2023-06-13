using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;
using IAG.ProcessEngine.Plugin.TestJob.SimplestJob;
using IAG.ProcessEngine.Enum;
using IAG.ProcessEngine.Exception;
using IAG.ProcessEngine.Execution;
using IAG.ProcessEngine.Execution.Condition;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Store;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.Test.Execution;

public class JobExecuterTest : IDisposable
{
    private readonly JobExecuter _executer;
    private readonly List<IJobState> _jobStore;
    private readonly Dictionary<Guid, List<IFollowUpJob>> _jobFollowUps;
    private readonly Mock<IJobService> _mockJobService;
    private readonly Mock<IServiceScope> _mockServiceScope;
    private Mock<IJobStore> _mockJobStore;

    public JobExecuterTest()
    {
        _jobStore = new List<IJobState>();
        _jobFollowUps = new Dictionary<Guid, List<IFollowUpJob>>();

        _mockJobStore = new Mock<IJobStore>();
        _mockJobStore.Setup(m => m.Insert(It.IsAny<IJobState>()))
            .Callback<IJobState>(state => _jobStore.Add(state));
        _mockJobStore.Setup(m => m.GetJobs()).Returns(() => _jobStore.AsQueryable());
        _mockJobStore.Setup(m => m.Get(It.IsAny<Guid>()))
            .Returns<Guid>(id => _jobStore.Find(state => state.Id == id));
        _mockJobStore.Setup(m => m.GetJobCount(null)).Returns(() => _jobStore.Count);

        var mockConfigStore = new Mock<IJobConfigStore>();
        mockConfigStore.Setup(m => m.GetFollowUpJobs(It.IsAny<Guid>())).Returns<Guid>(id =>
            _jobFollowUps.ContainsKey(id) ? _jobFollowUps[id] : new List<IFollowUpJob>());

        var mockConditionChecker = new Mock<IConditionChecker>();
        mockConditionChecker.Setup(m => m.CheckCondition(It.IsAny<IJobInstance>(), It.IsAny<string>()))
            .Returns<IJobInstance, string>((job, cond) =>
                string.IsNullOrEmpty(cond) || job.State.Result.Result.ToString() == cond);

        var mockServiceProvider = new Mock<IServiceProvider>();
        _mockServiceScope = new Mock<IServiceScope>();
        var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        _mockServiceScope.Setup(m => m.ServiceProvider).Returns(mockServiceProvider.Object);
        mockServiceScopeFactory.Setup(m => m.CreateScope()).Returns(_mockServiceScope.Object);
        mockServiceProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IJobStore)))).Returns(_mockJobStore.Object);
        mockServiceProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IJobConfigStore)))).Returns(mockConfigStore.Object);
        mockServiceProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IServiceScopeFactory)))).Returns(mockServiceScopeFactory.Object);

        _mockJobService = new Mock<IJobService>();
        _executer = new JobExecuter(mockConditionChecker.Object, new MockILogger<JobExecuter>());
        _executer.Start();
    }

    public void Dispose()
    {
        _executer.Dispose();
    }

    [Fact]
    public async Task NotRunningTestAsync()
    {
        _executer.Stop();
        var job = new SimplestJob {Config = {LogActivity = true}};
        using var jobInstance = new JobInstance(_mockServiceScope.Object) { State = new JobState(), Job = job};

        await Assert.ThrowsAsync<JobEngineNotRunningException>(() =>
            _executer.EnqueueJob(jobInstance, _mockJobService.Object));
    }

    [Fact]
    public async Task EnqueueJobTestAsync()
    {
        var job = new SimplestJob {Config = {LogActivity = true}};
        using var jobInstance = new JobInstance(_mockServiceScope.Object) {State = new JobState(), Job = job};

        var task = _executer.EnqueueJob(jobInstance, _mockJobService.Object);
        var runningJobs = _executer.RunningJobs;
        Assert.True(runningJobs.Count() == 1, "1 job running");
        await task;
        Assert.True(!_executer.RunningJobs.Any(), "no more jobs running");
    }

    [Fact]
    public async Task EnqueueJobPersistFailTestAsync()
    {
        var job = new SimplestJob {Config = {LogActivity = true}};
        _mockJobStore.Setup(m => m.Upsert(It.IsAny<IJobState>())).Throws(new System.Exception());
        using var jobInstance = new JobInstance(_mockServiceScope.Object) {State = new JobState(), Job = job};

        var task = _executer.EnqueueJob(jobInstance, _mockJobService.Object);
        var runningJobs = _executer.RunningJobs;
        Assert.True(runningJobs.Count() == 1, "1 job running");
        await task;
        Assert.True(!_executer.RunningJobs.Any(), "no more jobs running");
        Assert.Equal(JobExecutionStateEnum.Failed, jobInstance.State.ExecutionState);
    }

    [Fact]
    public async Task EnqueueExceptionJobTestAsync()
    {
        var mockJob = new Mock<IJob>();
        mockJob.Setup(m => m.TemplateId).Returns(Guid.NewGuid());
        var config = new Mock<IJobConfig>();
        config.Setup(c => c.Active).Returns(true);
        mockJob.Setup(m => m.Config).Returns(config.Object);
        mockJob.Setup(m => m.Result).Returns(new Mock<IJobResult>().Object);
        mockJob.Setup(m => m.Parameter).Returns(new Mock<IJobParameter>().Object);
        mockJob.Setup(m => m.Execute(It.IsAny<IJobInfrastructure>()))
            .Callback(() => throw new System.Exception("Test"));

        using var jobInstance = new JobInstance(_mockServiceScope.Object)
        {
            Job = mockJob.Object,
            State = new JobState()
        };

        var task = _executer.EnqueueJob(jobInstance, _mockJobService.Object);
        await task;
        Assert.Equal(JobExecutionStateEnum.Failed, jobInstance.State.ExecutionState);
    }

    [Fact]
    public async Task EnqueueNonActiveJobTest()
    {
        var mockJob = new Mock<IJob>();
        mockJob.Setup(m => m.TemplateId).Returns(Guid.NewGuid());
        mockJob.Setup(m => m.Config).Returns(new Mock<IJobConfig>().Object);
        mockJob.Setup(m => m.Result).Returns(new Mock<IJobResult>().Object);
        mockJob.Setup(m => m.Parameter).Returns(new Mock<IJobParameter>().Object);

        using var jobInstance = new JobInstance(_mockServiceScope.Object)
        {
            Job = mockJob.Object,
            State = new JobState()
        };

        await Assert.ThrowsAnyAsync<JobNotActiveException>(() =>
            _executer.EnqueueJob(jobInstance, _mockJobService.Object));
    }

    [Fact]
    public async Task GetJobStateTestAsync()
    {
        var job = new HelloJob {Config = {Delay = 100, NbOfOutputs = 10, HeartbeatTimeout = TimeSpan.FromMilliseconds(150)}};
        using var jobInstance = new JobInstance(_mockServiceScope.Object) {State = new JobState(), Job = job};

        var task = _executer.EnqueueJob(jobInstance, _mockJobService.Object);
        await Task.Delay(500);  // Heartbeat should prevent cancellation
        var jobStateRunning = _executer.GetJobState(jobInstance.Id)?.ExecutionState;
        await task;
            
        var jobStateAfterFinishing = _executer.GetJobState(jobInstance.Id);
        Assert.NotNull(jobStateRunning);
        Assert.Equal(JobExecutionStateEnum.Running, jobStateRunning);
        Assert.Null(jobStateAfterFinishing);
    }

    [Fact]
    public async Task EnqueueJobWithCancelTestAsync()
    {
        var job = new HelloJob {Config = {Delay = 5000}};
        using var jobInstance = new JobInstance(_mockServiceScope.Object) { State = new JobState(), Job = job};

        var task = _executer.EnqueueJob(jobInstance, _mockJobService.Object);
        var runningJob = _executer.RunningJobs.SingleOrDefault();
        await Task.Delay(500);  // job should be in delay now...
        var cancellationStopwatch = new Stopwatch();
        cancellationStopwatch.Start();
        _executer.CancelJob(runningJob?.Id ?? Guid.Empty);
        await task;
        cancellationStopwatch.Stop();

        Assert.NotNull(runningJob);
        Assert.Equal(JobExecutionStateEnum.Aborted, runningJob.State.ExecutionState);
        Assert.False(_executer.RunningJobs.Any(), "no more jobs running");
        Assert.True(cancellationStopwatch.ElapsedMilliseconds < 500);
    }

    [Fact]
    public async Task EnqueueJobWithMissingHeartbeatTestAsync()
    {
        var job = new HelloJob { Config = { Delay = 1000, HeartbeatTimeout = TimeSpan.FromMilliseconds(200) } };
        using var jobInstance = new JobInstance(_mockServiceScope.Object) { State = new JobState(), Job = job };

        var task = _executer.EnqueueJob(jobInstance, _mockJobService.Object);
        var runningJob = _executer.RunningJobs.SingleOrDefault();
        await Task.Delay(500);  // job should already be cancelled because of missing heartbeat
        var cancellationStopwatch = new Stopwatch();
        cancellationStopwatch.Start();
        await task;
        cancellationStopwatch.Stop();

        Assert.NotNull(runningJob);
        Assert.Equal(JobExecutionStateEnum.Aborted, runningJob.State.ExecutionState);
        Assert.False(_executer.RunningJobs.Any(), "no more jobs running");
        Assert.True(cancellationStopwatch.ElapsedMilliseconds < 10);
    }

    [Fact]
    public void EnqueueJobWithStopTest()
    {
        var job = new HelloJob {Config = {Delay = 1000}};
        using var jobInstance = new JobInstance(_mockServiceScope.Object) { State = new JobState(), Job = job };

        _executer.EnqueueJob(jobInstance, _mockJobService.Object);
        var runningJob = _executer.RunningJobs.SingleOrDefault();
        _executer.Stop();

        Assert.NotNull(runningJob);
        Assert.True(!_executer.RunningJobs.Any(), "no more jobs running");
    }

    [Fact]
    public void IgnoreCancelTest()
    {
        var job = new HelloJob
        {
            Config = {Delay = 500, NbOfOutputs = 4},
            Parameter = {IgnoreJobCancel = true}
        };
        using var jobInstance = new JobInstance(_mockServiceScope.Object) { State = new JobState(), Job = job };

        _executer.EnqueueJob(jobInstance, _mockJobService.Object);
        _executer.Config.JobShutdownDelay = 500;
        var runningJob = _executer.RunningJobs.SingleOrDefault();
        _executer.Stop();
        var hasRunningJobs = _executer.RunningJobs.Any();
        _executer.Dispose();

        Assert.NotNull(runningJob);
        // no way to stop a job that is not responding :-(
        Assert.True(hasRunningJobs, "no more jobs running");
    }

    [Fact]
    public void ExceptionTest()
    {
        var job = new HelloJob
        {
            Config = {Delay = 50, NbOfOutputs = 3},
            Parameter = {IgnoreJobCancel = true, ThrowException = true}
        };
        using var jobInstance = new JobInstance(_mockServiceScope.Object)
        {
            State = new JobState(),
            Job = job
        };

        _executer.EnqueueJob(jobInstance, _mockJobService.Object);
        _executer.Config.JobShutdownDelay = 1000;
        var runningJob = _executer.RunningJobs.SingleOrDefault();
        Assert.NotNull(runningJob);
        _executer.Stop();
        Assert.True(!_executer.RunningJobs.Any(), "no more jobs running");
    }

    [Fact]
    public void LocalizableExceptionTest()
    {
        var job = new HelloJob
        {
            Config = {Delay = 50, NbOfOutputs = 3},
            Parameter = {IgnoreJobCancel = true, ThrowLocalizableException = true}
        };
        using var jobInstance = new JobInstance(_mockServiceScope.Object)
        {
            State = new JobState(),
            Job = job
        };

        _executer.EnqueueJob(jobInstance, _mockJobService.Object);
        _executer.Config.JobShutdownDelay = 1000;
        var runningJob = _executer.RunningJobs.SingleOrDefault();
        Assert.NotNull(runningJob);
        _executer.Stop();
        Assert.True(!_executer.RunningJobs.Any(), "no more jobs running");
    }

    [Fact]
    public async Task RetryTest()
    {
        var job = new HelloJob
        {
            Config = {Delay = 0, NbOfOutputs = 0, RetryIntervals = new[] {new TimeSpan(0, 0, 0, 5)}},
            Parameter = {IgnoreJobCancel = false, ThrowException = true}
        };
        using var jobInstance = new JobInstance(_mockServiceScope.Object)
        {
            State = new JobState {Parameter = job.Parameter},
            Job = job
        };

        await _executer.EnqueueJob(jobInstance, _mockJobService.Object);
        await Task.Delay(500);

        Assert.Single(_jobStore);
        Assert.Equal(jobInstance.State.Id, _jobStore[0].ParentJob.Id);
        Assert.Equal(jobInstance.State.TemplateId, _jobStore[0].TemplateId);
        Assert.Equal(JobExecutionStateEnum.New, _jobStore[0].ExecutionState);
        Assert.Equal(5, Math.Round((_jobStore[0].DateDue - _jobStore[0].DateCreation).TotalSeconds));
    }

    [Fact]
    public void NonExistingCancelTest()
    {
        Assert.False(_executer.CancelJob(Guid.NewGuid()), "job to cancel not found");
    }

    [Fact]
    public void NonExistingGetJobStateTest()
    {
        Assert.Null(_executer.GetJobState(Guid.NewGuid()));
    }

    [Fact]
    public async Task DoubleJobEnqueueTestAsync()
    {
        var job = new HelloJob {Config = {Delay = 1000, AllowConcurrentInstances = true}};
        using var jobInstance = new JobInstance(_mockServiceScope.Object) {State = new JobState(), Job = job};

        var task = _executer.EnqueueJob(jobInstance, _mockJobService.Object);
        await Assert.ThrowsAsync<DuplicateKeyException>(() =>
            _executer.EnqueueJob(jobInstance, _mockJobService.Object));
        _executer.Stop();
        await task;
    }

    [Fact]
    public async Task ConcurrentJobEnqueueTestAsync()
    {
        var job1 = new HelloJob {Config = {Delay = 1000, AllowConcurrentInstances = false}};
        var job2 = new HelloJob {Config = {Delay = 1000, AllowConcurrentInstances = false}};
        using var jobInstance1 = new JobInstance(_mockServiceScope.Object) {State = new JobState(), Job = job1};
        using var jobInstance2 = new JobInstance(_mockServiceScope.Object) {State = new JobState(), Job = job2};

        var task = _executer.EnqueueJob(jobInstance1, _mockJobService.Object);
        var exception =
            await Assert.ThrowsAsync<LocalizableException>(() =>
                _executer.EnqueueJob(jobInstance2, _mockJobService.Object));
        Assert.NotNull(exception.LocalizableParameter);
        Assert.Contains(job1.TemplateId, exception.LocalizableParameter.Params);
        _executer.Stop();
        await task;
    }

    [Fact]
    public async Task EnqueueJobWithFollowUpsTestAsync()
    {
        var job = new SimplestJob();
        using var jobInstance = new JobInstance(_mockServiceScope.Object) {State = new JobState() {Result = job.Result}, Job = job};
        _jobFollowUps[job.Config.Id] = new List<IFollowUpJob>() {new FollowUpJob(), new FollowUpJob()};

        _mockJobService.Setup(m => m.CreateJobInstance(It.IsAny<Guid>(), It.IsAny<IJobState>()))
            .Returns(() => new JobInstance(_mockServiceScope.Object) { State = new JobState(), Job = new HelloJob() });

        await _executer.EnqueueJob(jobInstance, _mockJobService.Object);

        Assert.Equal(2, _executer.RunningJobs.Count());
    }

    [Fact]
    public async Task EnqueueJobWithConditionedFollowUpsTestAsync()
    {
        var job = new SimplestJob();
        using var jobInstance = new JobInstance(_mockServiceScope.Object)
        {
            State = new JobState
            {
                Result = job.Result
            },
            Job = job
        };
        var followUp1 = new HelloJob();
        var followUp2 = new SimplestJob();
        _jobFollowUps[job.Config.Id] = new List<IFollowUpJob>
        {
            new FollowUpJob
                {FollowUpJobConfigId = followUp1.Config.Id, ExecutionCondition = JobResultEnum.Success.ToString()},
            new FollowUpJob
                {FollowUpJobConfigId = followUp2.Config.Id, ExecutionCondition = JobResultEnum.Failed.ToString()}
        };

        var followJob = new HelloJob
        {
            Config = new HelloConfig
            {
                Delay = 0,
                NbOfOutputs = 6
            }
        };

        _mockJobService.Setup(m => m.CreateJobInstance(It.IsAny<Guid>(), It.IsAny<IJobState>()))
            .Returns(() => new JobInstance(_mockServiceScope.Object)
            {
                State = new JobState(),
                Job = followJob
            });

        await _executer.EnqueueJob(jobInstance, _mockJobService.Object);
        await Task.Delay(50);

        Assert.Equal(6, followJob.Result.NbExecutions);
    }
}