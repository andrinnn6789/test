using System;

namespace IAG.Infrastructure.Swagger;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class ODataQueryEndpointAttribute : Attribute
{
}