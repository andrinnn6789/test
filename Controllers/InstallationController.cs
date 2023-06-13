using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.BusinessLogic.Model;

using Microsoft.AspNetCore.Mvc;
using IAG.InstallClient.Models;
using IAG.InstallClient.Models.Mapper;
using IAG.InstallClient.ProcessEngineJob.Installation;
using IAG.InstallClient.ProcessEngineJob.Transfer;
using IAG.InstallClient.Resource;
using IAG.ProcessEngine.Execution;

using Microsoft.Extensions.Localization;

namespace IAG.InstallClient.Controllers;

public class InstallationController : BaseController
{
    public enum InstallationAction
    {
        Undefined,
        StartService,
        StopService,
        InstallService,
        Delete
    }

    private readonly IInstallationManager _installationManager;
    private readonly IReleaseManager _releaseManager;
    private readonly IServiceManager _serviceManager;
    private readonly IInventoryManager _inventoryManager;
    private readonly IJobService _jobService;

    public InstallationController(IInstallationManager installationManager, IReleaseManager releaseManager, IServiceManager serviceManager, ICustomerManager customerManager, IInventoryManager inventoryManager, IJobService jobService, IStringLocalizer<InstallationController> stringLocalizer)
        :base(customerManager, stringLocalizer)
    {
        _installationManager = installationManager;
        _releaseManager = releaseManager;
        _serviceManager = serviceManager;
        _inventoryManager = inventoryManager;
        _jobService = jobService;
    }

    [HttpGet]
    [ClaimAuthorization(ScopeNamesInfrastructure.InstallClient, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
    public async Task<IActionResult> Index()
    {
        return ViewCustomerChecked(await GetInstallationsAsync());
    }

    [HttpPost]
    [ClaimAuthorization(ScopeNamesInfrastructure.InstallClient, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Execute)]
    public async Task<IActionResult> Index(InstallationViewModel installation, InstallationAction installationAction)
    {
        return installationAction switch
        {
            InstallationAction.StartService => await ChangeService(() => _serviceManager.StartService(installation.ServiceName)),
            InstallationAction.StopService => await ChangeService(() => _serviceManager.StopService(installation.ServiceName)),
            InstallationAction.InstallService => await ChangeService(() =>
                _serviceManager.InstallService(installation.ProductName, installation.InstanceName)),
            InstallationAction.Delete => await ChangeService(() =>
            {
                if (!string.IsNullOrEmpty(installation.ServiceName))
                {
                    _serviceManager.UninstallService(installation.ServiceName);
                }

                _installationManager.DeleteInstance(installation.InstanceName);
                _installationManager.DeleteInstanceDirectory(installation.InstanceName);
                _inventoryManager.DeRegisterInstallationAsync(Customer.Id, installation.InstanceName);
            }),
            _ => View(await GetInstallationsAsync())
        };
    }

    [HttpGet]
    [ClaimAuthorization(ScopeNamesInfrastructure.InstallClient, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Create)]
    public async Task<IActionResult> New()
    {
        var model = new NewInstallationViewModel();
        try
        {
            model.ExistingInstances = await GetExistingInstancesAsync();
            model.AvailableReleases = await GetAvailableReleasesAsync();
        }
        catch (Exception ex)
        {
            model.ErrorMessage = MessageLocalizer.LocalizeException(ex);
        }

        return View("NewInstallation", model);
    }

    [HttpGet]
    [ClaimAuthorization(ScopeNamesInfrastructure.InstallClient, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Update)]
    public async Task<IActionResult> Update(string instanceName, string productName)
    {
        var model = new UpdateInstallationViewModel();
        try
        {
            var instances = await _installationManager.GetInstallationsAsync();
            var instanceToUpdate = instances.FirstOrDefault(i => i.InstanceName == instanceName && i.ProductName == productName);
            if (instanceToUpdate != null)
            {
                model.ServiceName =  _serviceManager.GetServiceName(instanceToUpdate.InstanceName);
                model.SelectedInstance = instanceName;
            }
            model.AvailableReleases = await GetAvailableReleasesAsync();
        }
        catch (Exception ex)
        {
            model.ErrorMessage = MessageLocalizer.LocalizeException(ex);
        }

        return View("UpdateInstallation", model);
    }

    [HttpGet]
    [ClaimAuthorization(ScopeNamesInfrastructure.InstallClient, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
    public async Task<IActionResult> Transfer(string instanceName, string productName)
    {
        var mapper = new InstallationViewModelMapper();
        var model = new TransferInstallationViewModel()
        {
            ProductName = productName
        };

        try
        {
            var instances = (await _installationManager.GetInstallationsAsync())
                .Where(i => i.ProductName == productName)
                .ToList();
            var targetInstance = instances.FirstOrDefault(i => i.InstanceName == instanceName);
            if (targetInstance != null)
            {
                model.TargetServiceName = _serviceManager.GetServiceName(targetInstance.InstanceName);
                model.TargetInstanceName = targetInstance.InstanceName;
            }

            model.AvailableSourceInstances = instances
                .Where(i => i.InstanceName != instanceName)
                .Select(i => mapper.NewDestination(i));
        }
        catch (Exception ex)
        {
            model.ErrorMessage = MessageLocalizer.LocalizeException(ex);
        }

        return View("TransferInstallation", model);
    }

    [HttpPost]
    [ClaimAuthorization(ScopeNamesInfrastructure.InstallClient, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Create)]
    public async Task<IActionResult> NewInstallation(NewInstallationViewModel model)
    {
        try
        {
            var availableReleases = await GetAvailableReleasesAsync();
            model.AvailableReleases = availableReleases;
            if (ModelState.IsValid && model.SelectedRelease.HasValue && !string.IsNullOrEmpty(model.SelectedInstance))
            {
                var release = availableReleases.FirstOrDefault(r => r.ReleaseId == model.SelectedRelease.Value);
                if (release == null)
                {
                    throw new LocalizableException(ResourceIds.UnknownReleaseError);
                }

                var customerExtension = availableReleases.FirstOrDefault(r => r.ProductType == ProductType.CustomerExtension
                                                                              && r.DependsOnProductId == release.ProductId && r.ReleaseVersion == release.ReleaseVersion);
                var configuration = availableReleases.FirstOrDefault(r => r.ReleaseId == model.SelectedConfiguration
                                                                          && r.DependsOnProductId == release.ProductId);

                var jobInstance = _jobService.CreateJobInstance(new Guid(InstallJob.JobId));
                jobInstance.Job.Parameter = new InstallJobParameter()
                {
                    Setup = new InstallationSetup
                    {
                        InstanceName = model.SelectedInstance,
                        CustomerId = Customer.Id,
                        ProductId = release.ProductId,
                        ReleaseId = release.ReleaseId,
                        CustomerExtensionReleaseId = customerExtension?.ReleaseId,
                        ConfigurationProductId = configuration?.ProductId,
                        ConfigurationReleaseId = configuration?.ReleaseId,
                    }
                };
                var _ = _jobService.EnqueueJob(jobInstance);

                model.InstallationJobId = jobInstance.Id;
            }
        }
        catch (Exception ex)
        {
            model.ErrorMessage = MessageLocalizer.LocalizeException(ex);
        }

        model.ExistingInstances = await GetExistingInstancesAsync();

        return View("NewInstallation", model);
    }

    [HttpPost]
    [ClaimAuthorization(ScopeNamesInfrastructure.InstallClient, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Update)]
    public async Task<IActionResult> UpdateInstallation(UpdateInstallationViewModel model)
    {
        try
        {
            var availableReleases = await GetAvailableReleasesAsync();
            model.AvailableReleases = availableReleases;
            if (ModelState.IsValid && model.SelectedRelease.HasValue &&
                !string.IsNullOrEmpty(model.SelectedInstance))
            {

                var release = availableReleases.FirstOrDefault(r => r.ReleaseId == model.SelectedRelease.Value);
                if (release == null)
                {
                    throw new LocalizableException(ResourceIds.UnknownReleaseError);
                }

                var serviceIsStarted = false;
                var serviceName = _serviceManager.GetServiceName(model.SelectedInstance);
                if (serviceName != null)
                {
                    serviceIsStarted = _serviceManager.GetServiceState(serviceName) == ServiceStatus.Running;
                    if (serviceIsStarted)
                        _serviceManager.StopService(serviceName);
                }

                var customerExtension = availableReleases.FirstOrDefault(r =>
                    r.ProductType == ProductType.CustomerExtension
                    && r.DependsOnProductId == release.ProductId && r.ReleaseVersion == release.ReleaseVersion);

                var jobInstance = _jobService.CreateJobInstance(new Guid(InstallJob.JobId));
                jobInstance.Job.Parameter = new InstallJobParameter()
                {
                    Setup = new InstallationSetup
                    {
                        InstanceName = model.SelectedInstance,
                        CustomerId = Customer.Id,
                        ProductId = release.ProductId,
                        ReleaseId = release.ReleaseId,
                        CustomerExtensionReleaseId = customerExtension?.ReleaseId
                    },
                    ServiceToStart = serviceIsStarted ? serviceName : null
                };
                var _ = _jobService.EnqueueJob(jobInstance);

                model.InstallationJobId = jobInstance.Id;
            }
        }
        catch (Exception ex)
        {
            model.ErrorMessage = MessageLocalizer.LocalizeException(ex);
        }

        return View("UpdateInstallation", model);
    }

    [HttpPost]
    [ClaimAuthorization(ScopeNamesInfrastructure.InstallClient, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Create)]
    public async Task<IActionResult> TransferInstallation(TransferInstallationViewModel model)
    {
        var mapper = new InstallationViewModelMapper();
        try
        {
            var instances = (await _installationManager.GetInstallationsAsync())
                .Where(i => i.ProductName == model.ProductName)
                .ToList();
            model.AvailableSourceInstances = instances
                .Where(i => i.InstanceName != model.TargetInstanceName)
                .Select(i => mapper.NewDestination(i));

            if (ModelState.IsValid && !string.IsNullOrEmpty(model.SourceInstanceName))
            {
                var targetInstallation = instances
                    .FirstOrDefault(i => i.InstanceName == model.TargetInstanceName);

                if (targetInstallation == null)
                {
                    throw new LocalizableException(ResourceIds.UnknownTargetInstanceError);
                }

                var startService = false;
                if (model.TargetServiceName != null)
                {
                    startService = _serviceManager.GetServiceState(model.TargetServiceName) == ServiceStatus.Running;
                    if (startService)
                        _serviceManager.StopService(model.TargetServiceName);
                }

                var jobInstance = _jobService.CreateJobInstance(new Guid(TransferJob.JobId));
                jobInstance.Job.Parameter = new TransferJobParameter()
                {
                    CustomerId = Customer.Id,
                    SourceInstanceName = model.SourceInstanceName,
                    TargetInstanceName = targetInstallation.InstanceName,
                    TargetVersion = targetInstallation.Version,
                    ServiceToStart = startService ? model.TargetServiceName : null
                };
                var _ = _jobService.EnqueueJob(jobInstance);

                model.TransferJobId = jobInstance.Id;
            }
        }
        catch (Exception ex)
        {
            model.ErrorMessage = MessageLocalizer.LocalizeException(ex);
        }

        return View("TransferInstallation", model);
    }

    private async Task<InstallationOverviewViewModel> GetInstallationsAsync()
    {
        var mapper = new InstallationViewModelMapper();
        var model = new InstallationOverviewViewModel();
        try
        {
            var installations = await _installationManager.GetInstallationsAsync();
            model.Installations = installations.Select(i => mapper.NewDestination(i)).ToList();
            foreach (var installation in model.Installations)
            {
                var serviceName = _serviceManager.GetServiceName(installation.InstanceName);
                if (!string.IsNullOrEmpty(serviceName))
                {
                    installation.ServiceName = serviceName;
                    installation.ServiceStatus = _serviceManager.GetServiceState(serviceName);
                }
            }
        }
        catch (Exception ex)
        {
            model.Installations ??= new List<InstallationViewModel>();
            model.ErrorMessage = MessageLocalizer.LocalizeException(ex);
        }

        return model;
    }

    private async Task<IEnumerable<string>> GetExistingInstancesAsync()
    {
        return (await _installationManager.GetInstallationsAsync()).Select(i => i.InstanceName);
    }

    private async Task<List<ReleaseViewModel>> GetAvailableReleasesAsync()
    {
        var releaseModels = new List<ReleaseViewModel>();
        var products = (await _releaseManager.GetProductsAsync(Customer.Id))
            .Where(p => p.ProductType != ProductType.Updater)
            .ToList();
        foreach (var product in products)
        {
            if (product.DependsOnProductId.HasValue && products.All(p => p.Id != product.DependsOnProductId.Value))
            {
                // depending product of updater (e.g. configuration template)
                continue;
            }

            var releases = await _releaseManager.GetReleasesAsync(Customer.Id, product.Id);
            releaseModels.AddRange(releases.OrderByDescending(r => r.ReleaseVersion).Select(release => new ReleaseViewModel
            {
                ProductId = product.Id,
                ReleaseId = release.Id,
                Plattform = release.Platform,
                ProductName = product.ProductName,
                ReleaseVersion = release.ReleaseVersion,
                ReleaseDate = release.ReleaseDate ?? DateTime.Now,
                ProductType = product.ProductType,
                DependsOnProductId = product.DependsOnProductId
            }));
        }

        return releaseModels;
    }

    private async Task<IActionResult> ChangeService(Action changeFunction)
    {
        string errorMessage = null;
        try
        {
            changeFunction();
        }
        catch (Exception ex)
        {
            errorMessage = MessageLocalizer.LocalizeException(ex);
        }

        var model = await GetInstallationsAsync();
        if (!string.IsNullOrEmpty(errorMessage))
        {
            if (!string.IsNullOrEmpty(model.ErrorMessage))
            {
                errorMessage += Environment.NewLine + model.ErrorMessage;
            }
            model.ErrorMessage = errorMessage;
        }

        return View("Index", model);
    }
}