using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.ProcessEngine.Enum;
using IAG.ProcessEngine.Exception;
using IAG.ProcessEngine.Execution.Condition;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Resource;
using IAG.ProcessEngine.Store;
using IAG.ProcessEngine.Store.Model;

using Microsoft.Extensions.Logging;

namespace IAG.ProcessEngine.Execution;

public class JobExecuter : IJobExecuter
{
    private readonly IConditionChecker _conditionChecker;
        
    private readonly ILogger<JobExecuter> _logger;
    private readonly HashSet<Guid> _runningSingletonJobIds;
    private readonly ConcurrentDictionary<Guid, JobExecutionInfrastructure> _runningJobs;

    public JobExecuter(IConditionChecker conditionChecker, ILogger<JobExecuter> logger)
    {
        _conditionChecker = conditionChecker;
        _logger = logger;
        _runningSingletonJobIds = new HashSet<Guid>();
        _runningJobs = new ConcurrentDictionary<Guid, JobExecutionInfrastructure>();
        Running = false;
        Config = new EngineConfig();    // Currently config is not persisted anywhere, do this when needed
    }

    public IEngineConfig Config { get; set; }

    public bool Running { get; private set; }
        
    public IEnumerable<IJobInstance> RunningJobs
    {
        get
        {
            return _runningJobs.Values.Select(r => r.JobInstance).ToList();
        }
    }

    public void Start()
    {
        Running = true;
    }

    public void Stop()
    {
        Running = false;
        List<Task> tasks = new List<Task>();
        foreach (var executionStructure in _runningJobs.Values)
        {
            executionStructure.CancelJob();
            if (executionStructure.JobTask != null)
                tasks.Add(executionStructure.JobTask);
        }
            
        Task.WaitAll(tasks.ToArray(), Config.JobShutdownDelay);
    }

    public Task EnqueueJob(IJobInstance jobInstance, IJobService jobService)
    {
        if (!Running)
            throw new JobEngineNotRunningException();

        if (!jobInstance.Job.Config.Active)
            throw new JobNotActiveException();

        CheckForConcurrentInstances(jobInstance);

        var executionInfrastructure = new JobExecutionInfrastructure(jobInstance, jobService);
        if (!_runningJobs.TryAdd(jobInstance.Id, executionInfrastructure))
        {
            executionInfrastructure.Dispose();
            FinalizeConcurrentInstances(jobInstance);

            throw new DuplicateKeyException(jobInstance.Id.ToString());
        }

        executionInfrastructure.JobTask = Task.Run(() =>
        {
            try
            {
                DoExecuteJob(jobInstance, executionInfrastructure);
                FinalizeFinishedJob(jobInstance);
                CheckRetryJob(executionInfrastructure.JobInstance, executionInfrastructure.GetJobStore());
                EnqueueFollowUpJobs(executionInfrastructure.JobInstance, jobService, executionInfrastructure.GetJobConfigStore());
            }
            finally
            {
                executionInfrastructure.Dispose();
            }
        });
        return executionInfrastructure.JobTask;
    }

    public IJobState GetJobState(Guid jobInstanceId)
    {
        if (!_runningJobs.ContainsKey(jobInstanceId))
            return null;

        return _runningJobs[jobInstanceId].JobInstance.State;
    }

    public bool CancelJob(Guid jobInstanceId)
    {
        if (!_runningJobs.ContainsKey(jobInstanceId))
        {
            return false;
        }

        _runningJobs[jobInstanceId].CancelJob();
        return true;
    }

    public void Dispose()
    {
        foreach (var executionStructure in _runningJobs.Values)
        {
            executionStructure.Dispose();
        }
    }

    private void CheckForConcurrentInstances(IJobInstance jobInstance)
    {
        if (jobInstance.Job.Config.AllowConcurrentInstances)
            return;

        lock (_runningSingletonJobIds)
        {
            if (!_runningSingletonJobIds.Add(jobInstance.Job.TemplateId))   // will return false if the id is already in the set
                throw new LocalizableException(ResourceIds.JobExecuterSingletonJobAlreadyRunning, jobInstance.Job.TemplateId);
        }
    }

    private void FinalizeConcurrentInstances(IJobInstance jobInstance)
    {
        if (jobInstance.Job.Config.AllowConcurrentInstances)
            return;

        lock (_runningSingletonJobIds)
        {
            _runningSingletonJobIds.Remove(jobInstance.Job.TemplateId);
        }
    }

    private void DoExecuteJob(IJobInstance jobInstance, JobExecutionInfrastructure executionInfrastructure)
    {
        _logger.LogInformation($"Starting {jobInstance.Job.TemplateId} {jobInstance.Job.Name} {jobInstance.Id}..");
        try
        {
            jobInstance.State.Parameter = jobInstance.Job.Parameter;    // ensure latest parametrization
            jobInstance.State.DateRunStart = DateTime.UtcNow;
            jobInstance.State.ExecutionState = JobExecutionStateEnum.Running;
            if (jobInstance.Job.Execute(executionInfrastructure))
                jobInstance.State.ExecutionState = jobInstance.Job.Result.Result == JobResultEnum.Success ? JobExecutionStateEnum.Success : JobExecutionStateEnum.Warning;
            else 
                jobInstance.State.ExecutionState = JobExecutionStateEnum.Failed;
            jobInstance.State.DateRunEnd = DateTime.UtcNow;
        }
        catch (OperationCanceledException)
        {
            jobInstance.State.ExecutionState = JobExecutionStateEnum.Aborted;
            jobInstance.State.Messages.Add(new MessageStructure(MessageTypeEnum.Error, ResourceIds.JobExecuterJobCanceled));
        }
        catch (System.Exception ex)
        {
            jobInstance.State.ExecutionState = JobExecutionStateEnum.Failed;
            jobInstance.State.Messages.Add(new MessageStructure(MessageTypeEnum.Error, ResourceIds.JobExecuterUnhandledException, LocalizableException.GetExceptionMessage(ex)));
            jobInstance.State.Messages.Add(new MessageStructure(ex));
        }

        try
        {
            executionInfrastructure.PersistState();
        }
        catch (System.Exception e)
        {
            jobInstance.State.ExecutionState = JobExecutionStateEnum.Failed;
            _logger.LogError(e, $"Job persistance failed for {jobInstance.Id} {jobInstance.Job.Name}: {e.Message}");
        }
    }

    private void FinalizeFinishedJob(IJobInstance jobInstance)
    {
        FinalizeConcurrentInstances(jobInstance);
        _runningJobs.TryRemove(jobInstance.Id, out _);
        _logger.LogInformation($"Finished job {jobInstance.Id} {jobInstance.Job.Name} with state {jobInstance.State.ExecutionState}");
    }

    private void CheckRetryJob(IJobInstance jobInstance, IJobStore jobStore)
    {
        if (jobInstance.State.ExecutionState != JobExecutionStateEnum.Failed 
            || jobInstance.Job.Config?.RetryIntervals == null
            || jobInstance.Job.Config.RetryIntervals.Length == 0
            || jobInstance.Job.Config.RetryIntervals.Length <= jobInstance.State.RetryCounter)
        {
            return; // job not failed or no (more) retry
        }
            
        IJobState failedJobState = jobInstance.State;
        IJobState retryJobState = new JobState
        {
            TemplateId = failedJobState.TemplateId,
            JobConfigId = failedJobState.JobConfigId,
            ParentJob = failedJobState,
            DateDue = DateTime.UtcNow + jobInstance.Job.Config.RetryIntervals[jobInstance.State.RetryCounter],
            IsBlocking = failedJobState.IsBlocking,
            MetadataId = failedJobState.MetadataId,
            Owner = failedJobState.Owner,
            ContextTenant = failedJobState.ContextTenant,
            RetryCounter = failedJobState.RetryCounter + 1,
            Parameter = failedJobState.Parameter
        };
                    
        jobStore.Insert(retryJobState);
    }

    private void EnqueueFollowUpJobs(IJobInstance jobInstance, IJobService jobService, IJobConfigStore jobConfigStore)
    {
        var followUpEntries = jobConfigStore.GetFollowUpJobs(jobInstance.Job.Config.Id);

        foreach (var followUpEntry in followUpEntries.Where(e => _conditionChecker.CheckCondition(jobInstance, e.ExecutionCondition)))
        {
            var followUpdJobInstance = jobService.CreateJobInstance(followUpEntry.FollowUpJobConfigId, jobInstance.State);
            EnqueueJob(followUpdJobInstance, jobService);
        }
    }
}