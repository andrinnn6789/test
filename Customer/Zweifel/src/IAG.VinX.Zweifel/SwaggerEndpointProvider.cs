using System.Collections.Generic;

using IAG.Infrastructure.Swagger;

using JetBrains.Annotations;

namespace IAG.VinX.Zweifel;

[UsedImplicitly]
public class SwaggerEndpointProvider : ISwaggerEndpointProvider
{
    public const string ApiEndpoint = "Swiss1Mobile";
    
    public IEnumerable<SwaggerEndpointDefinition> EndpointDefinitions =>
        new List<SwaggerEndpointDefinition>
        {
            new()
            {
                Name = ApiEndpoint,
                Title = "Swiss1Mobile API",
                Version = "1.0",
                Description = "I-AG API for Swiss1Mobile"
            }
        };
}