using System.Collections.Generic;
using System.Reflection;

using IAG.Infrastructure.Startup.Extensions.Swagger;
using IAG.Infrastructure.Swagger;

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;

using Moq;

using Swashbuckle.AspNetCore.SwaggerGen;

using Xunit;

namespace IAG.Infrastructure.Test.Startup.Extensions.Swagger;

public class ODataQueryParameterOperationFilterTest
{
    [Fact]
    public void NoParametersAddedForNoODataQueryEndpoint()
    {
        var testControllerType = typeof(TestController);
        var noODataGetMethod = testControllerType.GetMethod(nameof(TestController.NoODataGet));

        var operation = new OpenApiOperation()
        {
            Parameters = new List<OpenApiParameter>()
            {
                new() {Name = "ExistingParameter"}
            }
        };
        var context = CreateContext(noODataGetMethod);
        var opFilter = new ODataQueryParameterOperationFilter();

        opFilter.Apply(operation, context);

        Assert.Equal("ExistingParameter", Assert.Single(operation.Parameters)?.Name);
    }

    [Fact]
    public void ODataQueryParametersAddedForODataQueryEndpoint()
    {
        var testControllerType = typeof(TestController);
        var oDataGetMethod = testControllerType.GetMethod(nameof(TestController.ODataGet));

        var operation = new OpenApiOperation()
        {
            Parameters = new List<OpenApiParameter>()
            {
                new() {Name = "ExistingParameter"}
            }
        };
        var context = CreateContext(oDataGetMethod);
        var opFilter = new ODataQueryParameterOperationFilter();

        opFilter.Apply(operation, context);

        Assert.Equal(6, operation.Parameters.Count);
        Assert.Equal("ExistingParameter", operation.Parameters[0].Name);
        Assert.Contains(operation.Parameters, p => p.Name == "$filter");
        Assert.Contains(operation.Parameters, p => p.Name == "$skip");
        Assert.Contains(operation.Parameters, p => p.Name == "$top");
        Assert.Contains(operation.Parameters, p => p.Name == "$select");
        Assert.Contains(operation.Parameters, p => p.Name == "$orderby");
    }

    [Fact]
    public void NoParametersAddedForNotGetMethods()
    {
        var operation = new OpenApiOperation()
        {
            Parameters = new List<OpenApiParameter>()
            {
                new() {Name = "ExistingParameter"}
            }
        };
        var apiDescription = new ApiDescription() { HttpMethod = "POST" };
        var context = new OperationFilterContext(apiDescription, new Mock<ISchemaGenerator>().Object, new SchemaRepository(), null);
        var opFilter = new ODataQueryParameterOperationFilter();

        opFilter.Apply(operation, context);

        Assert.Equal("ExistingParameter", Assert.Single(operation.Parameters)?.Name);
    }

    [Fact]
    public void NoParametersAddedForNotControllerMethods()
    {
        var operation = new OpenApiOperation()
        {
            Parameters = new List<OpenApiParameter>()
            {
                new() {Name = "ExistingParameter"}
            }
        };
        var apiDescription = new ApiDescription() { HttpMethod = "GET" };
        var context = new OperationFilterContext(apiDescription, new Mock<ISchemaGenerator>().Object, new SchemaRepository(), null);
        var opFilter = new ODataQueryParameterOperationFilter();

        opFilter.Apply(operation, context);

        Assert.Equal("ExistingParameter", Assert.Single(operation.Parameters)?.Name);
    }


    private OperationFilterContext CreateContext(MethodInfo methodInfo)
    {
        var apiDescription = new ApiDescription()
        {
            HttpMethod = "GET",
            ActionDescriptor = new ControllerActionDescriptor()
            {
                MethodInfo = methodInfo
            }
        };
        return new OperationFilterContext(apiDescription, new Mock<ISchemaGenerator>().Object, new SchemaRepository(), methodInfo);
    }

    private class TestController
    {
        public void NoODataGet()
        {
        }

        [ODataQueryEndpoint]
        public void ODataGet()
        {
        }
    }
}