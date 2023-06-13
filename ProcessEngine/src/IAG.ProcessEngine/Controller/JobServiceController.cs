using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.ProcessEngine.JobModel;
using IAG.ProcessEngine.Execution;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Store;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IAG.ProcessEngine.Controller;

[Route(InfrastructureEndpoints.Process + "[controller]")]
[ClaimAuthorization(ScopeNamesInfrastructure.ProcessEngine, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
public class JobServiceController : ControllerBase
{
    private readonly IJobService _service;
    private readonly IJobConfigStore _jobConfigStore;

    private readonly IMessageLocalizer _localizer;
        
    public JobServiceController(IJobService service, IJobConfigStore jobConfigStore, IStringLocalizer<JobServiceController> localizer)
    {
        _service = service;
        _jobConfigStore = jobConfigStore;
        _localizer = new MessageLocalizer(localizer);
    }

    [HttpGet("Running")]
    public ActionResult<bool> Running()
    {
        return _service.Running;
    }

    [HttpGet("GetJobParameter")]
    public ActionResult<IJobParameter> GetJobParameter(Guid jobTemplateId)
    {
        return new(_service.GetJobParameter(jobTemplateId));
    }

    [HttpGet("RunningJobs")]
    public ActionResult<IEnumerable<IJobState>> RunningJobs()
    {
        return new(_service.RunningJobs.Select(j => j.State));
    }

    [HttpGet("GetJobInstanceState")]
    public ActionResult<IJobStateLocalized> GetJobInstanceState(Guid jobInstanceId)
    {
        var state = _service.GetJobInstanceState(jobInstanceId);
        var localizedState = new JobStateLocalized(state, _localizer);

        return localizedState;
    }

    // legacy VinX < 20.1
    [LegacySerializer]
    [Obsolete("Use ExecuteByName instead.")]
    [HttpPost("ExecuteJob")]
    public async Task<ActionResult<IJobStateLocalized>> ExecuteJob(string jobConfigName, [FromBody] JObject jobParam)
    {
        var jobConfig = _jobConfigStore.GetOrCreateJobConfig(jobConfigName);
        return await Execute(jobConfig.Id, jobParam);
    }

    [HttpPost("ExecuteByName")]
    public async Task<ActionResult<IJobStateLocalized>> ExecuteByName(string jobConfigName, [FromBody] JObject jobParam)
    {
        var jobConfig = _jobConfigStore.GetOrCreateJobConfig(jobConfigName);
        return await Execute(jobConfig.Id, jobParam);
    }

    [HttpPost("Execute")]
    public async Task<ActionResult<IJobStateLocalized>> Execute(Guid jobConfigId, [FromBody] JObject jobParam)
    {
        using var jobInstance = CreateJobInstance(jobConfigId, jobParam);
        await _service.EnqueueJob(jobInstance);
        return new JobStateLocalized(jobInstance.State, _localizer);
    }

    [HttpPost("Enqueue")]
    public ActionResult<IJobState> Enqueue(Guid jobConfigId, [FromBody] JObject jobParam)
    {
        var jobInstance = CreateJobInstance(jobConfigId, jobParam);
        _service.EnqueueJob(jobInstance).ContinueWith(_ => jobInstance.Dispose());

        return new ActionResult<IJobState>(_service.GetJobInstanceState(jobInstance.Id));
    }

    [HttpPost("AbortJob")]
    public ActionResult<bool> AbortJob(Guid jobInstanceId)
    {
        return _service.AbortJob(jobInstanceId);
    }

    [HttpPost("Start")]
    [ClaimAuthorization(ScopeNamesInfrastructure.ProcessEngine, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.SuperUser)]
    public IActionResult Start()
    {
        _service.Start();

        return new NoContentResult();
    }

    [HttpPost("Stop")]
    [ClaimAuthorization(ScopeNamesInfrastructure.ProcessEngine, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.SuperUser)]
    public IActionResult Stop()
    {
        _service.Stop();

        return new NoContentResult();
    }

    private IJobInstance CreateJobInstance(Guid jobConfigId, JObject param)
    {
        var jobInstance = _service.CreateJobInstance(jobConfigId);

        if (param != null)
        {
            Type paramType = jobInstance.Job.Parameter.GetType();
            var paramString = JsonConvert.SerializeObject(param);
            jobInstance.Job.Parameter = (IJobParameter)JsonConvert.DeserializeObject(paramString, paramType);
        }

        return jobInstance;
    }
}