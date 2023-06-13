using System;
using System.Collections.Generic;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.CoreServer.Configuration;
using IAG.Infrastructure.CoreServer.Plugin;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IAG.Infrastructure.CoreServer.Controller;

[Route(InfrastructureEndpoints.CoreAdmin + "[controller]")]
[ClaimAuthorization(ScopeNamesInfrastructure.AdminScope, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.All)]
public class PluginController : ControllerBase
{
    private readonly IPluginCatalogue _catalogue;
    private readonly IPluginConfigStore _configStore;
        
    public PluginController(IPluginCatalogue catalogue, IPluginConfigStore configStore)
    {
        _catalogue = catalogue;
        _configStore = configStore;
    }

    [HttpGet]
    public ActionResult<IEnumerable<IPluginMetadata>> GetAll()
    {
        return _catalogue.Plugins;
    }

    [HttpPost("Reload")]
    public IActionResult Reload()
    {
        _catalogue.Reload();

        return NoContent();
    }

    [HttpGet("{id}")]
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
    public ActionResult<IPluginConfig> GetConfigById(Guid id)
    {
        var plugin = _catalogue.GetPluginMeta(id);
        if (plugin == null)
        {
            return new NotFoundResult();
        }

        var config = _configStore.Get(id, plugin.PluginConfigType);
        return new ActionResult<IPluginConfig>(config);
    }

    [HttpPut("{id}/config")]
    [HttpPost("{id}/config")]
    public IActionResult SetConfigById(Guid id, [FromBody] JObject config)
    {
        if (config == null)
        {
            return BadRequest();
        }

        var plugin = _catalogue.GetPluginMeta(id);
        if (plugin == null)
        {
            return new NotFoundResult();
        }

        string configString = JsonConvert.SerializeObject(config);

        var pluginConfig = (IPluginConfig)JsonConvert.DeserializeObject(configString, plugin.PluginConfigType);
        _configStore.Write(pluginConfig, plugin.PluginConfigType);

        return new NoContentResult();
    }
}