using System.Linq;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IAG.Infrastructure.Rest;

public class JsonPropertyAnnotationContractResolver : CamelCasePropertyNamesContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var res = base.CreateProperty(member, memberSerialization);
        if (res.PropertyName != null
            && member.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault() is
                JsonPropertyAttribute attr)
        {
            res.PropertyName = attr.PropertyName;
        }

        return res;
    }
}