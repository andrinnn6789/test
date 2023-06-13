using System.Collections.Generic;

using IAG.Infrastructure.Swagger;

using JetBrains.Annotations;

namespace IAG.VinX.Globalwine;

[UsedImplicitly]
public class SwaggerEndpointProvider : ISwaggerEndpointProvider
{
    public const string ApiEndpoint = "Globalwine";

    public IEnumerable<SwaggerEndpointDefinition> EndpointDefinitions =>
        new List<SwaggerEndpointDefinition>
        {
            new()
            {
                Name = ApiEndpoint,
                Title = "I-AG Globalwine API",
                Version = "1.0",
                Description = "I-AG API for Globalwine, Shop with Next AG"
            }
        };
}