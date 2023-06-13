using Newtonsoft.Json.Serialization;

namespace IAG.Infrastructure.Rest;

public class JsonLowercasePropertyContractResolver : DefaultContractResolver
{
    protected override string ResolvePropertyName(string propertyName)
    {
        return propertyName.ToLowerInvariant();
    }
}