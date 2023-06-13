using System.Collections.Generic;

using IAG.Infrastructure.Swagger;

using JetBrains.Annotations;

namespace IAG.PerformX.CampusSursee;

[UsedImplicitly]
public class SwaggerEndpointProvider : ISwaggerEndpointProvider
{
    public const string ApiEndpoint = "CampusSursee";

    public IEnumerable<SwaggerEndpointDefinition> EndpointDefinitions =>
        new List<SwaggerEndpointDefinition>
        {
            new()
            {
                Name = ApiEndpoint,
                Title = "I-AG Campus Sursee API",
                Version = "1.0",
                Description = "I-AG API for Campus Sursee"
            }
        };
}