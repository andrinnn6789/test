using System;
using System.Reflection;

using Newtonsoft.Json.Linq;

namespace IAG.Infrastructure.ObjectMapper;

public static class ObjectPatcher
{
    public static T Patch<T>(T obj, JObject patchObject)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        if (patchObject == null)
        {
            return obj;
        }

        var targetType = obj.GetType();
        foreach (var jProperty in patchObject.Properties())
        {
            var propertyInfo = targetType.GetProperty(jProperty.Name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
            if (propertyInfo == null)
            {
                // unknown properties are just ignored
                continue;
            }

            var value = jProperty.Value.ToObject(propertyInfo.PropertyType);
            propertyInfo.SetValue(obj, value);
        }

        return obj;
    }
}