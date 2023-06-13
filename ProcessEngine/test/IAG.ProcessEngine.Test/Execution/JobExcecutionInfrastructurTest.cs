using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.ProcessEngine.JobData;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Plugin.TestJob.SimplestJob;
using IAG.ProcessEngine.Execution;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Store;

using Moq;

using Xunit;

namespace IAG.ProcessEngine.Test.Execution;

public class JobExcecutionInfrastructurTest
{
    private readonly Dictionary<Guid, IJobState> _jobStateStore = new();
    private readonly Dictionary<Guid, IJobData> _jobDataStore = new();
    private readonly List<IJobInstance> _enqueuedJobs = new();

    [Fact]
    public void CheckPropertiesTest()
    {
        var jobInfrastructure = CreateJobExecutionInfrastructure();
        var task = new Task(() => { });
        jobInfrastructure.JobTask = task;

        Assert.NotNull(jobInfrastructure.JobInstance);
        Assert.NotNull(jobInfrastructure.JobTask);
        Assert.NotEqual(default, jobInfrastructure.JobCancellationToken);
        Assert.Same(task, jobInfrastructure.JobTask);
    }

    [Fact]
    public async Task HeartbeatTest()
    {
        var startTimestamp = DateTime.UtcNow;
        var jobInfrastructure = CreateJobExecutionInfrastructure();
        jobInfrastructure.Heartbeat();
        var heartbeat1 = jobInfrastructure.JobInstance.State.LastHeartbeat;
        await Task.Delay(10);
        jobInfrastructure.Heartbeat();
        var heartbeat2 = jobInfrastructure.JobInstance.State.LastHeartbeat;
        await Task.Delay(10);

        Assert.True(startTimestamp <= heartbeat1);
        Assert.True(heartbeat1 <= heartbeat2);
        Assert.True(heartbeat2 <= DateTime.UtcNow);
    }

    [Fact]
    public void CheckJobCancellationTest()
    {
        var jobInfrastructure = CreateJobExecutionInfrastructure();
        jobInfrastructure.HeartbeatAndCheckJobCancellation();
        var cancellationRequestedBeforeCancel = jobInfrastructure.JobCancellationToken.IsCancellationRequested;
        jobInfrastructure.CancelJob();
        jobInfrastructure.CancelJob();  // Check if it can be called twice without exception

        Assert.False(cancellationRequestedBeforeCancel);
        Assert.Throws<OperationCanceledException>(() => jobInfrastructure.HeartbeatAndCheckJobCancellation());
        Assert.True(jobInfrastructure.JobCancellationToken.IsCancellationRequested);
    }

    [Fact]
    public void ProgressTest()
    {
        var jobInfrastructure = CreateJobExecutionInfrastructure();
        jobInfrastructure.HeartbeatAndReportProgress(-1);
        var progressNegative = jobInfrastructure.JobInstance.State.Progress;
        jobInfrastructure.HeartbeatAndReportProgress(0.25);
        var progress25 = jobInfrastructure.JobInstance.State.Progress;
        jobInfrastructure.HeartbeatAndReportProgress(2);
        var progressOverflow = jobInfrastructure.JobInstance.State.Progress;

        Assert.Equal(0, progressNegative);
        Assert.Equal(0.25, progress25);
        Assert.Equal(1, progressOverflow);
    }

    [Fact]
    public void MessageTest()
    {
        var jobInfrastructure = CreateJobExecutionInfrastructure();
        var messageCountEmpty = jobInfrastructure.JobInstance.State.Messages.Count;
        jobInfrastructure.AddMessage(new MessageStructure(MessageTypeEnum.Information, "Test"));

        Assert.Equal(0, messageCountEmpty);
        Assert.NotEmpty(jobInfrastructure.JobInstance.State.Messages);
        Assert.NotNull(jobInfrastructure.JobInstance.State.Messages.FirstOrDefault());
        Assert.Equal("Test", jobInfrastructure.JobInstance.State.Messages.FirstOrDefault()?.ResourceId);
    }

    [Fact]
    public void AddMessageThreadSafetyWithTasksTest()
    {
        var jobInfrastructure = CreateJobExecutionInfrastructure();
        jobInfrastructure.JobInstance.Job.Config.LogActivity = true;
        var tasks = Enumerable.Repeat(1, 100).Select(_ => Task.Factory.StartNew(() => jobInfrastructure.AddMessage(new MessageStructure(MessageTypeEnum.Information, "Test")))).ToArray();
            
        Task.WaitAll(tasks);

        Assert.Equal(100, jobInfrastructure.JobInstance.State.Messages.Count);
    }

    [Fact]
    public void AddMessageThreadSafetyWithThreadsTest()
    {
        var jobInfrastructure = CreateJobExecutionInfrastructure();
        jobInfrastructure.JobInstance.Job.Config.LogActivity = true;

        var threads = Enumerable.Repeat(1, 100).Select(_ => new Thread(() => jobInfrastructure.AddMessage(new MessageStructure(MessageTypeEnum.Information, "Test")))).ToArray();
        Array.ForEach(threads, t => t.Start());
        Array.ForEach(threads, t => t.Join());

        Assert.Equal(100, jobInfrastructure.JobInstance.State.Messages.Count);
    }

    [Fact]
    public void PersistStateTest()
    {
        var jobInfrastructure = CreateJobExecutionInfrastructure();
        jobInfrastructure.JobInstance.Job.Config.LogActivity = true;
            
        jobInfrastructure.PersistState();
            
        Assert.NotEmpty(_jobStateStore);
    }

    [Fact]
    public void JobDataTest()
    {
        var jobId = Guid.NewGuid();
        var jobInfrastructure = CreateJobExecutionInfrastructure();

        var dataIn = new Infrastructure.ProcessEngine.JobData.JobData {Id = jobId};
        jobInfrastructure.SetJobData(dataIn);

        var dataOut1 = jobInfrastructure.GetJobData<Infrastructure.ProcessEngine.JobData.JobData>();
        jobInfrastructure.RemoveJobData();
        var dataOut2 = jobInfrastructure.GetJobData<Infrastructure.ProcessEngine.JobData.JobData>();

        Assert.NotNull(dataOut1);
        Assert.NotNull(dataOut2);
        Assert.NotEqual(Guid.Empty, dataOut1.Id);
        Assert.NotEqual(jobId, dataOut1.Id);
    }

    [Fact]
    public void JobDataNotAllowedTest()
    {
        var jobInfrastructure = CreateJobExecutionInfrastructure();
        jobInfrastructure.JobInstance.Job.Config.AllowConcurrentInstances = true;

        Assert.Throws<LocalizableException>(() => jobInfrastructure.GetJobData<Infrastructure.ProcessEngine.JobData.JobData>());
        Assert.Throws<LocalizableException>(() => jobInfrastructure.SetJobData(new Infrastructure.ProcessEngine.JobData.JobData()));
        Assert.Throws<LocalizableException>(() => jobInfrastructure.RemoveJobData());
    }

    [Fact]
    public void EnqueueFollowUpJobTest()
    {
        var jobId = Guid.NewGuid();
        var jobInfrastructure = CreateJobExecutionInfrastructure();
        jobInfrastructure.JobInstance.Job.Config.LogActivity = true;

        jobInfrastructure.EnqueueFollowUpJob(jobId);
        var enqueuedJob = Assert.Single(_enqueuedJobs);

        var jobParam = new JobParameter();
        jobInfrastructure.EnqueueFollowUpJob(jobId, jobParam);

        Assert.NotNull(enqueuedJob);
        Assert.Equal(2, _enqueuedJobs.Count);
        Assert.NotNull(_enqueuedJobs[1]);
        Assert.Equal(jobParam, _enqueuedJobs[1].Job.Parameter);
    }

    private JobExecutionInfrastructure CreateJobExecutionInfrastructure()
    {
        var mockJobStore = new Mock<IJobStore>();
        var mockJobDataStore = new Mock<IJobDataStore>();
        var mockJobService = new Mock<IJobService>();
        var mockJobState = new Mock<IJobState>();
        var simpleJob = new SimplestJob();
        var mockJobInstance = new Mock<IJobInstance>();

        mockJobState.SetupAllProperties();
        mockJobState.Setup(m => m.Messages).Returns(new List<MessageStructure>());
        mockJobState.Setup(m => m.TemplateId).Returns(Guid.NewGuid());
            

        mockJobStore.Setup(m => m.Update(It.IsAny<IJobState>())).Callback<IJobState>(
            jobState =>
            {
                if (!_jobStateStore.ContainsKey(jobState.TemplateId))
                {
                    throw new NotFoundException(jobState.TemplateId.ToString());
                }
                        
                _jobStateStore[jobState.TemplateId] = jobState;
            });
        mockJobStore.Setup(m => m.Upsert(It.IsAny<IJobState>())).Callback<IJobState>(
            jobState =>
            {
                if (_jobStateStore.ContainsKey(jobState.TemplateId))
                    _jobStateStore[jobState.TemplateId] = jobState;
                else
                    _jobStateStore.Add(jobState.TemplateId, jobState);
            });
        mockJobStore.Setup(m => m.Insert(It.IsAny<IJobState>())).Callback<IJobState>(
            jobState =>
            {
                if (_jobStateStore.ContainsKey(jobState.TemplateId))
                {
                    throw new DuplicateKeyException(jobState.TemplateId.ToString());
                }

                _jobStateStore.Add(jobState.TemplateId, jobState);
            });

        mockJobDataStore.Setup(m => m.Set<IJobData>(It.IsAny<IJobData>())).Callback<IJobData>(
            data =>
            {
                _jobDataStore[data.Id] = data;
            });
        mockJobDataStore.Setup(m => m.Get<Infrastructure.ProcessEngine.JobData.JobData>(It.IsAny<Guid>())).Returns<Guid>(
            id =>
            {
                if (!_jobDataStore.ContainsKey(id))
                {
                    _jobDataStore[id] = new Infrastructure.ProcessEngine.JobData.JobData();
                }
                return _jobDataStore[id] as Infrastructure.ProcessEngine.JobData.JobData;
            });
        mockJobDataStore.Setup(m => m.Remove(It.IsAny<Guid>())).Callback<Guid>(
            id =>
            {
                _jobDataStore.Remove(id);
            });

        mockJobService.Setup(m => m.CreateJobInstance(It.IsAny<Guid>(), It.IsAny<IJobState>())).Returns(() => mockJobInstance.Object);
        mockJobService.Setup(m => m.EnqueueJob(It.IsAny<IJobInstance>())).Callback<IJobInstance>(job => _enqueuedJobs.Add(job));

        var mockConfigStore = new Mock<IJobConfigStore>();

        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IJobStore)))).Returns(mockJobStore.Object);
        mockServiceProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IJobDataStore)))).Returns(mockJobDataStore.Object);
        mockServiceProvider.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IJobConfigStore)))).Returns(mockConfigStore.Object);

        mockJobInstance.Setup(m => m.State).Returns(() => mockJobState.Object);
        mockJobInstance.Setup(m => m.Job).Returns(() => simpleJob);
        mockJobInstance.Setup(m => m.ServiceProvider).Returns(mockServiceProvider.Object);

        return new JobExecutionInfrastructure(mockJobInstance.Object, mockJobService.Object);
    }
}