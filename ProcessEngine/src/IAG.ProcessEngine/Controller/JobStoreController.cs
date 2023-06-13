using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;
using IAG.ProcessEngine.Enum;
using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Store;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.Localization;

namespace IAG.ProcessEngine.Controller;

[ApiExplorerSettings(GroupName = ApiExplorerDefaults.DefaultGroup)]
[Route(InfrastructureEndpoints.Process + "[controller]")]
[ClaimAuthorization(ScopeNamesInfrastructure.ProcessEngine, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
[ApiController]
public class JobStoreController : ODataController
{
    private readonly IJobStore _store;
    private readonly IStringLocalizer<JobStoreController> _localizer;

    public JobStoreController(IJobStore store, IStringLocalizer<JobStoreController> localizer)
    {
        _store = store;
        _localizer = localizer;
    }

    [HttpPost("DeleteOldJobs")]
    [ClaimAuthorization(ScopeNamesInfrastructure.ProcessEngine, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Delete)]
    public IActionResult DeleteOldJobs(int archiveDays, int errorDays)
    {
        _store.DeleteOldJobs(archiveDays, errorDays);

        return new NoContentResult();
    }

    [HttpPost("DeleteScheduledJobs")]
    [ClaimAuthorization(ScopeNamesInfrastructure.ProcessEngine, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Delete)]
    public IActionResult DeleteScheduledJobs()
    {
        _store.DeleteScheduledJobs();

        return new NoContentResult();
    }

    [ODataQueryEndpoint]
    [EnableQuery]
    [HttpGet]
    public ActionResult<IEnumerable<IJobState>> GetJobs()
    {
        return _store.GetJobs().ToList();
    }

    [ODataQueryEndpoint]
    [EnableQuery]
    [HttpGet("{id}")]
    public ActionResult<IJobState> GetJobs(Guid id)
    {
        var log = _store.GetJobs().FirstOrDefault(j => j.Id == id);
        return log == null 
            ? throw new NotFoundException(id.ToString())
            : new ActionResult<IJobState>(log);
    }

    [ODataQueryEndpoint]
    [EnableQuery]
    [HttpGet("Log")]
    public ActionResult<IEnumerable<JobLogLocalized>> GetJobLogs()
    {
        var jobs = _store.GetJobs();
        return new ActionResult<IEnumerable<JobLogLocalized>>(jobs.Select(j => new JobLogLocalized(j, _localizer)));
    }

    [ODataQueryEndpoint]
    [EnableQuery]
    [HttpGet("Log/{id}")]
    public ActionResult<JobLogLocalized> GetJobLog(Guid id)
    {
        var log = _store.GetJobs().FirstOrDefault(j => j.Id == id);
        return log == null 
            ? throw new NotFoundException(id.ToString()) 
            : new ActionResult<JobLogLocalized>(new JobLogLocalized(log, _localizer));
    }

    [HttpGet("GetJobCount")]
    public ActionResult<int> GetJobCount(JobExecutionStateEnum[] executionStateFilter = null)
    {
        return _store.GetJobCount(executionStateFilter);
    }
}