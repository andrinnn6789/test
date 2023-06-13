using System;
using System.Linq;

using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Store;

using Microsoft.Extensions.DependencyInjection;

namespace IAG.ProcessEngine.Execution;

public class JobBuilder : IJobBuilder
{
    private readonly IJobCatalogue _catalogue;
    private readonly IJobConfigStore _jobConfigStore;
    private readonly IServiceProvider _serviceProvider;

    public JobBuilder(IServiceProvider serviceProvider, IJobCatalogue catalogue, IJobConfigStore jobConfigStore)
    {
        _jobConfigStore = jobConfigStore;
        _catalogue = catalogue;
        _serviceProvider = serviceProvider;
    }

    public IJobInstance BuildInstance(Guid jobConfigId, IJobState jobStateParent)
    {
        var jobInstance = CreateJob(jobConfigId);
        var userContext = jobInstance.ServiceProvider.GetRequiredService<IUserContext>();

        var jobState = new JobState
        {
            TemplateId = jobInstance.Job.Config.TemplateId,
            JobConfigId = jobConfigId,
            Result = jobInstance.Job.Result,
            Parameter = jobInstance.Job.Parameter,
            ParentJob = jobStateParent,
            Owner = userContext.UserName,
            ContextTenant = userContext.TenantId
        };
        jobInstance.State = jobState;

        return jobInstance;
    }

    public IJobInstance ReBuildInstance(IJobState jobState)
    {
        var jobInstance = CreateJob(jobState.JobConfigId);
        jobInstance.State = jobState;
        jobState.Result ??= jobInstance.Job.Result;    // ensure result in state

        return jobInstance;
    }

    private JobInstance CreateJob(Guid jobConfigId)
    {
        var jobConfig =_jobConfigStore.Read(jobConfigId);
        var jobMeta = _catalogue.Catalogue.FirstOrDefault(c => c.TemplateId == jobConfig.TemplateId);
        if (jobMeta == null)
        {
            throw new NotFoundException(jobConfig.TemplateId.ToString());
        }

        var serviceScope = _serviceProvider.CreateScope();

        var job = (IJob)serviceScope.ServiceProvider.GetRequiredService(jobMeta.JobType);
        job.Config = jobConfig;

        return new JobInstance(serviceScope)
        {
            Job = job
        };
    }
}