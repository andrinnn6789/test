using System.Collections.Generic;

using IAG.Infrastructure.Swagger;

using JetBrains.Annotations;

namespace IAG.PerformX.ibW;

[UsedImplicitly]
public class SwaggerEndpointProvider : ISwaggerEndpointProvider
{
    public const string ApiEndpoint = "ibW";
    public const string ApiEndpointAzure = ApiEndpoint + "-Azure";
    public const string ApiEndpointWeb = ApiEndpoint + "-Web";

    public IEnumerable<SwaggerEndpointDefinition> EndpointDefinitions =>
        new List<SwaggerEndpointDefinition>
        {
            GetEndpointDefinition(ApiEndpointAzure),
            GetEndpointDefinition(ApiEndpointWeb)
        };

    private SwaggerEndpointDefinition GetEndpointDefinition(string endpoint)
    {
        return new()
        {
            Name = endpoint,
            Title = $"I-AG {endpoint} API",
            Version = "1.0",
            Description = $"I-AG API for {endpoint}"
        };
    }
}