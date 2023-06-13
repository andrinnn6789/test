using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Swagger;

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace IAG.Infrastructure.Startup.Extensions.Swagger;

public class ODataQueryParameterOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.HttpMethod?.Equals(HttpMethod.Get.ToString(),
                StringComparison.InvariantCultureIgnoreCase) != true)
        {
            return;
        }

        if (!(context.ApiDescription.ActionDescriptor is ControllerActionDescriptor controllerDescriptor))
        {
            return;
        }

        if (!controllerDescriptor.MethodInfo.GetCustomAttributes(typeof(ODataQueryEndpointAttribute), true).Any())
        {
            return;
        }

        operation.Parameters ??= new List<OpenApiParameter>();
        operation.Parameters.Add(CreateODataParameter("$filter"));
        operation.Parameters.Add(CreateODataParameter("$skip", true));
        operation.Parameters.Add(CreateODataParameter("$top", true));
        operation.Parameters.Add(CreateODataParameter("$select"));
        operation.Parameters.Add(CreateODataParameter("$orderby"));
    }

    private OpenApiParameter CreateODataParameter(string name, bool integer = false)
    {
        return new()
        {
            Name = name,
            Description = $"Parameter '{name}' for OData query",
            AllowEmptyValue = true,
            In = ParameterLocation.Query,
            Schema = new OpenApiSchema()
            {
                Type = integer ? "integer" : "string"
            }
        };
    }
}