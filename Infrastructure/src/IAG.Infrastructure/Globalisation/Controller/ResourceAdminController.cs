using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.Globalisation.Admin;
using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Globalisation.ResourceProvider;
using IAG.Infrastructure.Globalisation.TranslationExchanger;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

using Microsoft.AspNetCore.Mvc;

namespace IAG.Infrastructure.Globalisation.Controller;

[Route(InfrastructureEndpoints.Resource + "[controller]")]
public class ResourceAdminController : ControllerBase
{
    private readonly ResourceContext _context;
    private readonly IStringLocalizerFactoryReloadable _factory;

    public ResourceAdminController(ResourceContext context, IStringLocalizerFactoryReloadable factory)
    {
        _context = context;
        _factory = factory;
    }

    [HttpPost("Update")]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Update)]
    public IActionResult Update([FromBody] TranslationSync request)
    {
        new ResourceAdmin(_context).SyncSystems(request);

        return new OkResult();
    }

    [HttpGet("GetFlat")]
    [ClaimAuthorization(
        ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read,
        ScopeNamesInfrastructure.ReaderScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read
    )]
    public ActionResult<IEnumerable<TranslationFlat>> GetFlat(string culture)
    {
        return new ResourceAdmin(_context).GetFlat(culture);
    }

    [HttpGet("DownloadResources")]
    public IActionResult DownloadResources()
    {
        return new ResourceExcelSyncer(_context).DownloadResources();
    }

    [HttpPost("UploadResources")]
    public async Task<IActionResult> UploadResources()
    {
        await using var data = new MemoryStream();
        await Request.Body.CopyToAsync(data);
        new ResourceExcelSyncer(_context).UploadResources(data.ToArray());
        return Accepted();
    }

    [HttpPost("Reload")]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Update)]
    public ActionResult Reload()
    {
        _factory.Reload();
        return new NoContentResult();
    }

    [HttpPost("Collect")]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Update)]
    public IActionResult Collect([FromServices] IResourceCollector collector)
    {
        collector.CollectAndUpdate();
        return Reload();
    }
}