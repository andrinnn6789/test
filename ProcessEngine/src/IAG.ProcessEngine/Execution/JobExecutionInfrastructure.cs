using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.ProcessEngine.JobData;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Resource;
using IAG.ProcessEngine.Store;

using Microsoft.Extensions.DependencyInjection;

namespace IAG.ProcessEngine.Execution;

public sealed class JobExecutionInfrastructure : IJobInfrastructure, IDisposable
{
    private readonly IJobService _jobService;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public JobExecutionInfrastructure(IJobInstance jobInstance, IJobService service)
    {
        _jobService = service;
        JobInstance = jobInstance;
        _cancellationTokenSource = new CancellationTokenSource(jobInstance.Job.Config.HeartbeatTimeout);
    }

    public IJobInstance JobInstance { get; }

    public Task JobTask { get; set; }

    public CancellationToken JobCancellationToken => _cancellationTokenSource.Token;

    private IJobDataStore GetJobDataStore() => JobInstance.ServiceProvider.GetRequiredService<IJobDataStore>();

    internal IJobStore GetJobStore() => JobInstance.ServiceProvider.GetRequiredService<IJobStore>();

    internal IJobConfigStore GetJobConfigStore() => JobInstance.ServiceProvider.GetRequiredService<IJobConfigStore>();


    public void Heartbeat()
    {
        JobInstance.State.LastHeartbeat = DateTime.UtcNow;
        _cancellationTokenSource.CancelAfter(JobInstance.Job.Config.HeartbeatTimeout);
    }

    public void HeartbeatAndCheckJobCancellation()
    {
        Heartbeat();
        if (!Debugger.IsAttached)
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
    }

    public void HeartbeatAndReportProgress(double progress)
    {
        Heartbeat();

        if (progress < 0)
            progress = 0;
        else if (progress > 1)
            progress = 1;

        JobInstance.State.Progress = progress;
        PersistState();
    }

    public void AddMessage(MessageStructure message)
    {
        JobInstance.State.Messages.Add(message);
        PersistState();
    }

    public void PersistState()
    {
        if (JobInstance.Job.Config?.LogActivity ?? false)
        {
            GetJobStore().Upsert(JobInstance.State);
        }
    }

    public T GetJobData<T>() where T : IJobData, new()
    {
        if (JobInstance.Job.Config.AllowConcurrentInstances)
        {
            throw new LocalizableException(ResourceIds.JobDataStoreNotForConcurrentJobs);
        }

        return GetJobDataStore().Get<T>(JobInstance.Job.TemplateId);
    }

    public void SetJobData<T>(T data) where T : IJobData
    {
        if (JobInstance.Job.Config.AllowConcurrentInstances)
        {
            throw new LocalizableException(ResourceIds.JobDataStoreNotForConcurrentJobs);
        }

        data.Id = JobInstance.Job.TemplateId;

        GetJobDataStore().Set<T>(data);
    }

    public void RemoveJobData()
    {
        if (JobInstance.Job.Config.AllowConcurrentInstances)
        {
            throw new LocalizableException(ResourceIds.JobDataStoreNotForConcurrentJobs);
        }

        GetJobDataStore().Remove(JobInstance.Job.TemplateId);
    }


    public void EnqueueFollowUpJob(Guid jobConfigId, IJobParameter parameter = null)
    {
        if (parameter == null || parameter.TimeToRunUtc <= DateTime.UtcNow)
        {
            var jobInstance = _jobService.CreateJobInstance(jobConfigId, JobInstance.State);
            jobInstance.Job.Parameter = parameter;
            _jobService.EnqueueJob(jobInstance);
        }
        else
        {
            GetJobStore().Insert(new JobState
            {
                DateDue = parameter.TimeToRunUtc, 
                JobConfigId = jobConfigId,
                ParentJob = JobInstance.State
            });
        }
    }
    public void CancelJob()
    {
        _cancellationTokenSource.Cancel();
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();
    }
}