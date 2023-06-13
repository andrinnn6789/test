using System.Collections.Generic;

namespace IAG.Infrastructure.Swagger;

public interface ISwaggerEndpointProvider
{
    IEnumerable<SwaggerEndpointDefinition> EndpointDefinitions { get; }
}