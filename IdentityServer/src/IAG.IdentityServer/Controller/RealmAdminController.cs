using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using IAG.IdentityServer.Configuration;
using IAG.IdentityServer.Configuration.Model.Realm;
using IAG.IdentityServer.SeedImportExport;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Rest;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using Newtonsoft.Json;

namespace IAG.IdentityServer.Controller;

[Route(InfrastructureEndpoints.Auth + "Admin/[controller]")]
public class RealmAdminController : ControllerBase
{
    private readonly IRealmCatalogue _realmCatalogue;
    private readonly IPluginCatalogue _pluginCatalogue;
    private readonly IStringLocalizer _localizer;


    public RealmAdminController(IRealmCatalogue realmCatalogue, IPluginCatalogue pluginCatalogue, IStringLocalizer<RealmAdminController> localizer)
    {
        _realmCatalogue = realmCatalogue;
        _pluginCatalogue = pluginCatalogue;
        _localizer = localizer;
    }

    [HttpGet]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
    public ActionResult<IEnumerable<IRealmConfig>> GetAll()
    {
        return _realmCatalogue.Realms;
    }

    [HttpPost("Reload")]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
    public IActionResult Reload()
    {
        _realmCatalogue.Reload();

        return NoContent();
    }

    [HttpPost("Import")]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Create)]
    public IActionResult Import([FromBody]RealmImportExport importData, [FromServices]IRealmSeedImporterExporter importer)
    {
        if (string.IsNullOrEmpty(importData?.RealmConfig?.Realm))
        {
            return BadRequest();
        }

        importer.ImportRealm(importData);

        return NoContent();
    }

    [HttpGet("{id}")]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
    public ActionResult<IRealmConfig> GetById(string id)
    {
        var realmConfig = _realmCatalogue.GetRealm(id);
        if (realmConfig == null)
        {
            return NotFound();
        }

        return new ActionResult<IRealmConfig>(realmConfig);
    }

    [HttpGet("{id}/Export")]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
    public IActionResult ExportById(string id, [FromServices]IRealmSeedImporterExporter exporter)
    {
        try
        {
            var realmExport = exporter.Export(id, out string seedFileName);

            return File(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(realmExport)),
                ContentTypes.ApplicationJson, seedFileName);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new MessageLocalizer(_localizer).LocalizeException(ex));
        }
    }

    [HttpPost]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Create | PermissionKind.Update)]
    public IActionResult Insert([FromBody]RealmConfig config)
    {
        if (config == null)
        {
            return BadRequest("parameter 'config' is null");
        }

        if (string.IsNullOrEmpty(config.Realm))
        {
            return BadRequest("parameter 'config.Realm' is null");
        }

        return SaveRealmConfig(config);
    }

    [HttpPut("{id}")]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Update)]
    public IActionResult Update(string id, [FromBody]RealmConfig config)
    {
        if (string.IsNullOrEmpty(id) || (config == null))
        {
            return BadRequest();
        }

        if (_realmCatalogue.GetRealm(id) == null)
        {
            return NotFound(id);
        }

        config.Realm = id;

        return SaveRealmConfig(config);
    }

    [HttpDelete("{id}")]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Delete)]
    public IActionResult Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        try
        {
            _realmCatalogue.Delete(id);
        }
        catch (NotFoundException)
        {
            return NotFound(id);
        }

        return new NoContentResult();
    }

    [HttpPost("PublishClaims")]
    [ClaimAuthorization(ScopeNamesInfrastructure.SystemScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Create)]
    public async Task<IActionResult> PublishClaims([FromBody]IEnumerable<ClaimDefinition> claimDefinitions, [FromServices]IClaimPublisher publisher)
    {
        await publisher.PublishClaimsAsync(claimDefinitions);

        return NoContent();
    }

    private IActionResult SaveRealmConfig(RealmConfig realmConfig)
    {
        try
        {
            realmConfig.SetPluginConfig(_pluginCatalogue);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new MessageLocalizer(_localizer).LocalizeException(ex));
        }

        _realmCatalogue.Save(realmConfig);

        return new NoContentResult();
    }
}