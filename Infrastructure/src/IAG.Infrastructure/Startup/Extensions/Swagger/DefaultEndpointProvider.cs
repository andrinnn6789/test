using System.Collections.Generic;

using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.Swagger;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Startup.Extensions.Swagger;

[UsedImplicitly]
public class DefaultEndpointProvider: ISwaggerEndpointProvider
{
    public IEnumerable<SwaggerEndpointDefinition> EndpointDefinitions =>
        new List<SwaggerEndpointDefinition>
        {
            new()
            {
                Name = ApiExplorerDefaults.DefaultGroup,
                Title = "I-AG Base API v1.0",
                Version = "1.0",
                Description = "I-AG API base functionality"
            },
            new()
            {
                Name = ApiExplorerDefaults.AppGroup,
                Title = "I-AG Mobile API v1.0",
                Version = "1.0",
                Description = "I-AG API for the App",
            },
            new()
            {
                Name = ApiExplorerDefaults.AppGroupV20,
                Title = "I-AG Mobile API v2.0",
                Version = "2.0",
                Description = "I-AG API for the App",
            }
        };
}