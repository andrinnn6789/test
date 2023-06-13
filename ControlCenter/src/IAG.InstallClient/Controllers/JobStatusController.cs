using IAG.Infrastructure.Globalisation.Localizer;
using IAG.ProcessEngine.Enum;
using IAG.ProcessEngine.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

namespace IAG.InstallClient.Controllers;

public class JobStatusController : Controller
{
    private readonly IJobService _jobService;
    private readonly IStringLocalizer<InstallationController> _stringLocalizer;

    public JobStatusController(IJobService jobService, IStringLocalizer<InstallationController> stringLocalizer)
    {
        _jobService = jobService;
        _stringLocalizer = stringLocalizer;
    }

    [HttpGet]
    [ClaimAuthorization(ScopeNamesInfrastructure.InstallClient, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
    public IActionResult GetJobStatus(Guid jobId)
    {
        var jobState = _jobService.GetJobInstanceState(jobId);
        var messageLocalizer = new MessageLocalizer(_stringLocalizer);
        return Ok(new
        {
            Messages = jobState.Messages.Select(messageLocalizer.Localize).ToList(),
            Finished = jobState.ExecutionState >= JobExecutionStateEnum.Success,
            Success = jobState.ExecutionState == JobExecutionStateEnum.Success,
            jobState.Progress
        });
    }
}