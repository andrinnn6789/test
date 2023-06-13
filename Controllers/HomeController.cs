using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.Models;
using IAG.InstallClient.ProcessEngineJob.SelfUpdate;
using IAG.ProcessEngine.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

namespace IAG.InstallClient.Controllers;

public class HomeController : BaseController
{
    private readonly IInstallationManager _installationManager;
    private readonly IJobService _jobService;

    public HomeController(IInstallationManager installationManager, ICustomerManager customerManager, IJobService jobService, IStringLocalizer<HomeController> stringLocalizer) : base(customerManager, stringLocalizer)
    {
        _installationManager = installationManager;
        _jobService = jobService;
    }

    [ClaimAuthorization(ScopeNamesInfrastructure.InstallClient, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
    public IActionResult Index()
    {
        if (Customer == null)
        {
            return ViewCustomerChecked(null);
        }

        var model = new HomeViewModel
        {
            CurrentReleaseVersion = _installationManager.CurrentSelfVersion
        };

        return View(model);
    }

    [HttpPost]
    [ClaimAuthorization(ScopeNamesInfrastructure.InstallClient, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Execute)]
    public IActionResult StartSelfUpdate()
    {
        var model = new HomeViewModel
        {
            CurrentReleaseVersion = _installationManager.CurrentSelfVersion
        };

        try
        {
            var jobInstance = _jobService.CreateJobInstance(new Guid(SelfUpdateJob.JobId));
            var _ = _jobService.EnqueueJob(jobInstance);

            model.SelfUpdateJobId = jobInstance.Id;
        }
        catch (Exception ex)
        {
            model.ErrorMessage = MessageLocalizer.LocalizeException(ex);
        }

        return View("Index", model);
    }
}