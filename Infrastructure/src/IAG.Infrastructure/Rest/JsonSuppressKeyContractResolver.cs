using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IAG.Infrastructure.Rest;

public class SuppressKeySerializeContractResolver : JsonPropertyAnnotationContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        if (property.PropertyName?.ToUpper() == "ID")
        {
            property.ShouldSerialize =
                _ => false;
        }

        return property;
    }
}