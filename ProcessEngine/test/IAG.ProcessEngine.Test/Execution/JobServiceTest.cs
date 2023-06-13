using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Plugin.TestJob.HelloWorldJob;
using IAG.ProcessEngine.Enum;
using IAG.ProcessEngine.Execution;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.InternalJob.Cleanup;
using IAG.ProcessEngine.Store;
using IAG.ProcessEngine.Store.Model;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.Test.Execution;

public class JobServiceTestBase
{
    protected const int LoopDelay = 200;
    protected readonly IJobService Service;
    protected readonly IJobStore JobStore;

    protected JobServiceTestBase()
    {
        var mockJobCatalog = new Mock<IJobCatalogue>();
        var mockJobBuilder = new Mock<IJobBuilder>();
        var mockJobEngine = new Mock<IJobExecuter>();
        var mockJobInstance = new Mock<IJobInstance>();
        var mockJobState = new Mock<IJobState>();
        var testJob = new HelloJob();
        var testMeta = new JobMetadata()
        {
            TemplateId = testJob.TemplateId, PluginName = testJob.Name, JobType = typeof(HelloJob),
            ConfigType = typeof(HelloConfig), ParameterType = typeof(HelloParameter)
        };

        var cancellationToken = new CancellationTokenSource();
        var isRunning = false;

        mockJobCatalog.Setup(m => m.Catalogue).Returns(new List<IJobMetadata> { testMeta });

        mockJobState.SetupAllProperties();
        mockJobState.Object.Result = testJob.Result;
        mockJobState.Object.Parameter = testJob.Parameter;

        mockJobInstance.Setup(m => m.Id).Returns(Guid.NewGuid());
        mockJobInstance.Setup(m => m.State).Returns(mockJobState.Object);
        mockJobInstance.Setup(m => m.Job).Returns(testJob);

        mockJobBuilder.Setup(m => m.BuildInstance(It.IsAny<Guid>(), It.IsAny<IJobState>())).Returns(mockJobInstance.Object);
        mockJobBuilder.Setup(m => m.ReBuildInstance(It.IsAny<IJobState>())).Returns(mockJobInstance.Object);

        var jobStore = new List<IJobState>();
        mockJobEngine.Setup(m => m.EnqueueJob(It.IsAny<IJobInstance>(), It.IsAny<IJobService>()))
            .Returns<IJobInstance, IJobService>((instance, _) => Task.Run(() =>
            {
                instance.State.ExecutionState = JobExecutionStateEnum.Running;
                try
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Thread.Sleep(LoopDelay);
                        cancellationToken.Token.ThrowIfCancellationRequested();
                    }

                    instance.State.ExecutionState = JobExecutionStateEnum.Success;
                }
                catch (OperationCanceledException)
                {
                    instance.State.ExecutionState = JobExecutionStateEnum.Aborted;
                }

                if (instance.Job.Config.LogActivity)
                    jobStore.Add(instance.State);
            }));

        mockJobEngine.Setup(m => m.CancelJob(It.IsAny<Guid>())).Returns(() =>
        {
            cancellationToken.Cancel();
            return true;
        });

        mockJobEngine.Setup(m => m.GetJobState(It.IsAny<Guid>())).Returns(() =>
            mockJobInstance.Object.State.ExecutionState == JobExecutionStateEnum.Running ? mockJobInstance.Object.State : null);
        mockJobEngine.Setup(m => m.Start()).Callback(() => isRunning = true);
        mockJobEngine.Setup(m => m.Stop()).Callback(() => isRunning = false);
        mockJobEngine.Setup(m => m.Running).Returns(() => isRunning);
        mockJobEngine.Setup(m => m.RunningJobs).Returns(() => new List<IJobInstance>());

        var mockJobStore = new Mock<IJobStore>();
        mockJobStore.Setup(m => m.Insert(It.IsAny<IJobState>())).Callback<IJobState>((state) => jobStore.Add(state));
        mockJobStore.Setup(m => m.GetJobs()).Returns(() => jobStore.AsQueryable());
        mockJobStore.Setup(m => m.Get(It.IsAny<Guid>())).Returns<Guid>((id) => jobStore.Find(state => state.Id == id));
        mockJobStore.Setup(m => m.GetJobCount(null)).Returns(() => jobStore.Count);
        JobStore = mockJobStore.Object;

        Service = new JobService(mockJobCatalog.Object, mockJobBuilder.Object, mockJobEngine.Object, mockJobStore.Object);
        Service.Start();
    }
}

[Collection("JobServiceTestBase")]
public class JobServiceTestCancel : JobServiceTestBase
{
    [Fact]
    public async Task EnqueueJobWithCancelTest()
    {
        var jobInstance = Service.CreateJobInstance(Guid.NewGuid());

        await Service.EnqueueJob(jobInstance);
        await Task.Delay(LoopDelay);

        var abortResult = Service.AbortJob(jobInstance.Id);

        await Task.Delay(2*LoopDelay); // give time to abort job...

        var jobState = Service.GetJobInstanceState(jobInstance.Id);

        Assert.NotEqual(Guid.Empty, jobInstance.Id);
        Assert.True(abortResult, "Failed to abort job");
        Assert.Null(jobState);
    }
}

[Collection("JobServiceTestBase")]
public class JobServiceTest: JobServiceTestBase
{
    [Fact]
    public void CreateJobInstanceTest()
    {
        var jobInstance1 = Service.CreateJobInstance(Guid.NewGuid());
        var jobInstance2 = Service.CreateJobInstance(jobInstance1.State);
        var jobInstance3 = Service.CreateJobInstance(Guid.NewGuid(), jobInstance1.State);

        Assert.NotNull(jobInstance1);
        Assert.NotNull(jobInstance2);
        Assert.NotNull(jobInstance3);
    }

    [Fact]
    public void StartTest()
    {
        Assert.True(Service.Running);
    }

    [Fact]
    public void StopTest()
    {
        Service.Stop();
        Assert.False(Service.Running);
    }

    [Fact]
    public async Task EnqueueJobTest()
    {
        var jobInstance = Service.CreateJobInstance(Guid.NewGuid());
        var jobTask = Service.EnqueueJob(jobInstance);
        await Task.Delay(LoopDelay);

        var jobStateRunning = Service.GetJobInstanceState(jobInstance.Id).ExecutionState;
        await jobTask;

        var jobStateEnd = Service.GetJobInstanceState(jobInstance.Id);

        Assert.NotEqual(Guid.Empty, jobInstance.Id);
        Assert.Equal(JobExecutionStateEnum.Running, jobStateRunning);
        Assert.Null(jobStateEnd);
    }

    [Fact]
    public async Task EnqueueInternalJobTest()
    {
        var mockJobInstance = new Mock<IJobInstance>();
        var testJob = new CleanupJob(JobStore) { Config = new CleanupJobConfig() };
        mockJobInstance.Setup(m => m.Job).Returns(testJob);
        mockJobInstance.Setup(m => m.State).Returns(new JobState());

        await Service.EnqueueJob(mockJobInstance.Object);
    }

    [Fact]
    public void RunningJobsTest()
    {
        Assert.Empty(Service.RunningJobs);
    }

    [Fact]
    public async Task EnqueueJobTimingTest()
    {
        var start = DateTime.UtcNow;
        var jobInstance = Service.CreateJobInstance(Guid.NewGuid());
        await Service.EnqueueJob(jobInstance);
        var duration = (DateTime.UtcNow - start).TotalMilliseconds;

        Assert.NotNull(jobInstance.State.Result);
        Assert.True(duration >= 5 * LoopDelay, "Test to fast...");
    }

    [Fact]
    public void GetJobParameterTest()
    {
        var jobParam = Service.GetJobParameter(JobInfoAttribute.GetTemplateId(typeof(HelloJob)));

        Assert.IsType<HelloParameter>(jobParam);
    }

    [Fact]
    public void GetUnknownJobParameterTest()
    {
        Assert.Throws<NotFoundException>(() => Service.GetJobParameter(Guid.NewGuid()));
    }
}