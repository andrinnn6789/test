using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Store;

namespace IAG.ProcessEngine.Execution;

public class JobService : IJobService
{
    private readonly IJobCatalogue _jobCatalogue;
    private readonly IJobBuilder _jobBuilder;
    private readonly IJobExecuter _executer;
    private readonly IJobStore _jobStore;

    public JobService(IJobCatalogue jobCatalogue, IJobBuilder jobBuilder, IJobExecuter executer, IJobStore jobStore)
    {
        _jobStore = jobStore;
        _jobCatalogue = jobCatalogue;
        _jobBuilder = jobBuilder;
        _executer = executer;
    }

    public bool Running => _executer.Running;

    public IEnumerable<IJobInstance> RunningJobs => _executer.RunningJobs;

    public IJobParameter GetJobParameter(Guid templateId)
    {
        var jobMetadata = _jobCatalogue.Catalogue.FirstOrDefault(job => job.TemplateId == templateId);
        if (jobMetadata == null)
        {
            throw new NotFoundException(templateId.ToString());
        }

        return Activator.CreateInstance(jobMetadata.ParameterType) as IJobParameter;
    }

    public IJobInstance CreateJobInstance(Guid jobConfigId, IJobState jobStateParent = null)
    {
        return _jobBuilder.BuildInstance(jobConfigId, jobStateParent);
    }

    public IJobInstance CreateJobInstance(IJobState jobState)
    {
        return _jobBuilder.ReBuildInstance(jobState);
    }

    public Task EnqueueJob(IJobInstance jobInstance)
    {
        return _executer.EnqueueJob(jobInstance, this);
    }

    public IJobState GetJobInstanceState(Guid jobInstanceId)
    {
        var state  = _executer.GetJobState(jobInstanceId);
        return state ?? _jobStore.Get(jobInstanceId);
    }
        
    public bool AbortJob(Guid jobInstanceId)
    {
        return _executer.CancelJob(jobInstanceId);
    }

    public void Start()
    {
        _executer.Start();
    }

    public void Stop()
    {
        _executer.Stop();
    }
}