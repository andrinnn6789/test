using System.Collections.Generic;

using IAG.Infrastructure.Swagger;

using JetBrains.Annotations;

namespace IAG.ControlCenter;

[UsedImplicitly]
public class SwaggerEndpointProvider : ISwaggerEndpointProvider
{
    public const string ApiControlCenter = "ControlCenter";

    public IEnumerable<SwaggerEndpointDefinition> EndpointDefinitions =>
        new List<SwaggerEndpointDefinition>
        {
            new()
            {
                Name = ApiControlCenter,
                Title = "I-AG Control Center Api",
                Version = "1.0",
                Description = "I-AG Control Center Api"
            }
        };
}