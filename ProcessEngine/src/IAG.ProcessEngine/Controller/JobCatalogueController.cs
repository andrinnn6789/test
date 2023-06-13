using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;
using IAG.ProcessEngine.Store;
using IAG.ProcessEngine.Store.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.Localization;

namespace IAG.ProcessEngine.Controller;

[ApiExplorerSettings(GroupName = ApiExplorerDefaults.DefaultGroup)]
[Route(InfrastructureEndpoints.Process + "[controller]")]
[ClaimAuthorization(ScopeNamesInfrastructure.ProcessEngine, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
[ApiController]
public class JobCatalogueController : ODataController
{
    private readonly IJobCatalogue _catalogue;
    private readonly IStringLocalizer<JobCatalogueController> _localizer;

    public JobCatalogueController(IJobCatalogue catalogue, IStringLocalizer<JobCatalogueController> localizer)
    {
        _catalogue = catalogue;
        _localizer = localizer;
    }

    [HttpGet]
    [ODataQueryEndpoint]
    [EnableQuery]
    public ActionResult<IEnumerable<IJobMetadata>> GetAll()
    {
        var jobMetadatas = _catalogue.Catalogue;
        foreach (var jobMetadata in jobMetadatas)
        {
            jobMetadata.Description = _localizer.GetString(jobMetadata.PluginName);
        }
        return Ok(_catalogue.Catalogue);
    }

    [HttpGet("{templateId}")]
    public ActionResult<IJobMetadata> GetById(Guid templateId)
    {
        var meta = _catalogue.Catalogue.FirstOrDefault(m => m.TemplateId == templateId);
        if (meta == null)
            return NotFound();
        meta.Description = _localizer.GetString(meta.PluginName);
        return new ActionResult<IJobMetadata>(meta);
    }
}