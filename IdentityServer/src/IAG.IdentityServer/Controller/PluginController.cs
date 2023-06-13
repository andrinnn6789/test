using System;
using System.Collections.Generic;

using IAG.IdentityServer.Configuration;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.IdentityServer.Plugin;

using Microsoft.AspNetCore.Mvc;

namespace IAG.IdentityServer.Controller;

[Route(InfrastructureEndpoints.Auth + "Admin/[controller]")]
public class PluginController : ControllerBase
{
    private readonly IPluginCatalogue _catalogue;
        
    public PluginController(IPluginCatalogue catalogue)
    {
        _catalogue = catalogue;
    }

    [HttpGet]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
    public ActionResult<IEnumerable<IPluginMetadata>> GetAll()
    {
        return _catalogue.Plugins;
    }

    [HttpPost("Reload")]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
    public IActionResult Reload()
    {
        _catalogue.Reload();

        return NoContent();
    }

    [HttpGet("{id}")]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
    public ActionResult<IPluginMetadata> GetById(Guid id)
    {
        var plugin = _catalogue.GetPluginMeta(id);
        if (plugin == null)
        {
            return new NotFoundResult();
        }

        return new ActionResult<IPluginMetadata>(plugin);
    }

    [HttpGet("{id}/config")]
    [ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
    public ActionResult<IAuthenticationPluginConfig> GetConfigById(Guid id)
    {
        var plugin = _catalogue.GetPluginMeta(id);
        if (plugin == null)
        {
            return new NotFoundResult();
        }

        var config = Activator.CreateInstance(plugin.PluginConfigType) as IAuthenticationPluginConfig;
        return new ActionResult<IAuthenticationPluginConfig>(config);
    }
}