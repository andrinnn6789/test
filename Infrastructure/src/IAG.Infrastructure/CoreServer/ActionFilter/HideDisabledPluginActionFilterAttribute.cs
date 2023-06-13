using System;
using IAG.Infrastructure.CoreServer.Plugin;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IAG.Infrastructure.CoreServer.ActionFilter;

public class HideDisabledPluginActionFilterAttribute : ActionFilterAttribute
{
    private readonly Type _pluginConfigType;

    public HideDisabledPluginActionFilterAttribute(Type pluginConfigType)
    {
        if (!typeof(IPluginConfig).IsAssignableFrom(pluginConfigType))
        {
            throw new ArgumentException("pluginConfigType has to implement IPluginConfig", nameof(pluginConfigType));
        }

        _pluginConfigType = pluginConfigType;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var config = context.HttpContext.RequestServices.GetService(_pluginConfigType) as IPluginConfig;
        if (config?.Active != true)
        {
            context.Result = new NotFoundObjectResult("Plugin has no active configuration");
        }
        else
        {
            base.OnActionExecuting(context);
        }
    }
}