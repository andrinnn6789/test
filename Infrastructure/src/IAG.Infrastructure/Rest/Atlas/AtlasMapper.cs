using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Resource;
using IAG.Infrastructure.Rest.Atlas.Dto;

using Newtonsoft.Json;

namespace IAG.Infrastructure.Rest.Atlas;

public class AtlasMapper<T>
    where T : new()
{
    public AtlasMapper(List<Property> properties)
    {
        Properties = properties;
    }

    protected List<Property> Properties { get; }

    public T GetObject(List<object> data)
    {
        if (Properties.Count != data.Count)
            throw new ArgumentException("Length of properties description not equal to data");

        T result = new T();
        Type type = result.GetType();
        var annotedProperties = type.GetProperties()
            .Select(
                prop =>
                {
                    var jsonProp = prop.GetCustomAttribute(typeof(JsonPropertyAttribute)) as JsonPropertyAttribute;
                    return new
                    {
                        JsonPropertyName = jsonProp?.PropertyName,
                        Property = prop
                    };
                })
            .Where(prop => prop.JsonPropertyName != null)
            .ToDictionary(prop => prop.JsonPropertyName, prop => prop.Property);
        for (int i = 0; i < Properties.Count; i++)
        {
            var propInfo = annotedProperties.ContainsKey(Properties[i].Name) 
                ? annotedProperties[Properties[i].Name] 
                : type.GetProperty(Properties[i].Name, BindingFlags.Public | BindingFlags.Instance);

            if (propInfo == null || !propInfo.CanWrite)
                continue;

            var propType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
            var propValue = (data[i] == null) ? null : Convert.ChangeType(data[i], propType);
            propInfo.SetValue(result, propValue);
        }

        return result;
    }

    public List<object> GetData(T obj)
    {
        var result = new List<object>();
        Type type = typeof(T);

        foreach (var property in Properties)
        {
            var propInfo = type.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);
            if (propInfo == null || !propInfo.CanRead)
                throw new LocalizableException(ResourceIds.AtlasMapperMissingProperty, property.Name);

            result.Add(propInfo.GetValue(obj));
        }

        return result;
    }
}