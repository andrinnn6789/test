using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.DI;

using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.IdentityServer.Authorization.Model;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.ObjectMapper;
using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.Resource;
using IAG.Infrastructure.Swagger;
using IAG.ProcessEngine.Execution.Condition;
using IAG.ProcessEngine.Store;
using IAG.ProcessEngine.Store.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.Localization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IAG.ProcessEngine.Controller;

[ApiExplorerSettings(GroupName = ApiExplorerDefaults.DefaultGroup)]
[Route(InfrastructureEndpoints.Process + "[controller]")]
[ClaimAuthorization(ScopeNamesInfrastructure.ProcessEngine, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.Read)]
[ApiController]
public class JobConfigStoreController : ODataController
{
    private readonly IJobConfigStore _store;
    private readonly IJobCatalogue _jobCatalogue;
    
    public JobConfigStoreController(IJobConfigStore store, IJobCatalogue jobCatalogue)
    {
        _store = store;
        _jobCatalogue = jobCatalogue;
    }

    [ODataQueryEndpoint]
    [EnableQuery]
    [HttpGet]
    public ActionResult<IEnumerable<IJobConfig>> GetAllUnprocessed()
    {
        return new(_store.GetAllUnprocessed());
    }

    [HttpGet("{id}")]
    public ActionResult<IJobConfig> GetByIdUnprocessed(Guid id)
    {
        return new(_store.ReadUnprocessed(id));
    }

    [ODataQueryEndpoint]
    [EnableQuery]
    [ClaimAuthorization(ScopeNamesInfrastructure.ProcessEngine, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.SuperUser)]
    [HttpGet("Processed")]
    public ActionResult<IEnumerable<IJobConfig>> GetAll()
    {
        return new(_store.GetAll());
    }

    [ClaimAuthorization(ScopeNamesInfrastructure.ProcessEngine, ClaimNamesInfrastructure.GeneralClaim, PermissionKind.SuperUser)]
    [HttpGet("Processed/{id}")]
    public ActionResult<IJobConfig> GetById(Guid id)
    {
        return new(_store.Read(id));
    }

    [HttpGet("ForTemplate/{templateId}")]
    public ActionResult<IJobConfig> GetForTemplate(Guid templateId)
    {
        var meta = _jobCatalogue.Catalogue.FirstOrDefault(m => m.TemplateId == templateId);
        return meta == null ? NotFound() : new ActionResult<IJobConfig>(Activator.CreateInstance(meta.ConfigType) as IJobConfig);
    }

    [HttpPost]
    public IActionResult Create([FromBody] JObject config, [FromServices] IStringLocalizer<JobConfigStoreController> localizer, [FromServices] IConditionChecker conditionChecker)
    {
        var jobConfig = GetJobConfig(config, localizer, conditionChecker);
        _store.Insert(jobConfig);

        return new CreatedResult(nameof(GetById), jobConfig);
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, [FromBody] JObject config, [FromServices] IStringLocalizer<JobConfigStoreController> localizer, [FromServices] IConditionChecker conditionChecker)
    {
        var jobConfig = GetJobConfig(config, localizer, conditionChecker);
        jobConfig.Id = id;
        _store.Update(jobConfig);

        return new NoContentResult();
    }

    [HttpPatch("{id}")]
    public IActionResult Patch(Guid id, [FromBody] JObject patch)
    {
        var jobConfig = _store.ReadUnprocessed(id);
        jobConfig = ObjectPatcher.Patch(jobConfig, patch);
        _store.Update(jobConfig);

        return new NoContentResult();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        _store.Delete(_store.Read(id));

        return new NoContentResult();
    }

    [HttpPost("InitConfigs")]
    public IActionResult InitConfigs()
    {
        foreach (var meta in _jobCatalogue.Catalogue)
        {
            if (_store.GetAll().Any(c => c.TemplateId == meta.TemplateId))
            {
                continue;
            }

            var config = ActivatorWithCheck.CreateInstance<IJobConfig>(meta.ConfigType);
            config.Id = meta.TemplateId;
            _store.Insert(config);
        }

        return Ok();
    }

    [HttpPost("CleanUpConfigs")]
    public IActionResult CleanUpConfigs()
    {
        foreach (var config in _store.GetAll().ToList())    // ToList() necessary as we're deleting configs!
        {
            if (_jobCatalogue.Catalogue.Any(m => m.TemplateId == config.TemplateId))
            {
                continue;
            }

            _store.Delete(config);
        }

        return Ok();
    }

    private IJobConfig GetJobConfig(JObject config, IStringLocalizer<JobConfigStoreController> localizer, IConditionChecker conditionChecker)
    {
        if (config == null || !config.HasValues ||
            !config.TryGetValue(nameof(IJobConfig.TemplateId), StringComparison.InvariantCultureIgnoreCase, out JToken templateIdToken) ||
            !Guid.TryParse(templateIdToken.ToString(), out Guid templateId))
        {
            throw new BadRequestException(ResourceIds.ValueIsEmpty, "JobConfig");
        }

        var meta = GetJobMetadata(templateId);
        var jobConfig = (IJobConfig)JsonConvert.DeserializeObject(config.ToString(Formatting.None), meta.ConfigType);
        
        try
        {
            foreach (var followUpJob in jobConfig?.FollowUpJobs ?? Array.Empty<FollowUpJob>().ToList())
            {
                conditionChecker.CheckConditionValidity(followUpJob.ExecutionCondition);
            }
        }
        catch (ParseException ex)
        {
            throw new BadRequestException(new MessageLocalizer(localizer).LocalizeException(ex));
        }

        return jobConfig;
    }

    private IJobMetadata GetJobMetadata(Guid templateId)
    {
        var configType = _jobCatalogue.Catalogue.FirstOrDefault(m => m.TemplateId == templateId);
        if (configType == null)
        {
            throw new NotFoundException(templateId.ToString());
        }

        return configType;
    }
}