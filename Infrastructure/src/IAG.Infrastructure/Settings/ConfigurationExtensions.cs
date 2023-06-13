using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json.Linq;

namespace IAG.Infrastructure.Settings;

public static class ConfigurationExtensions
{
    public static IEnumerable<KeyValuePair<string, string>> GetAllConfigValues(this IConfigurationRoot configurationRoot)
    {
        foreach (var configurationProvider in configurationRoot.Providers)
        {
            foreach (var key in GetAllConfigValues(configurationProvider))
            {
                yield return key;
            }
        }
    }

    public static IEnumerable<KeyValuePair<string,string>> GetAllConfigValues(this IConfigurationProvider configurationProvider, string parent = null)
    {
        foreach (var key in configurationProvider.GetChildKeys(Enumerable.Empty<string>(), parent))
        {
            var fullKey = parent != null ? $"{parent}:{key}" : key;
            if (!configurationProvider.TryGet(fullKey, out var value))
            {
                foreach (var configEntry in GetAllConfigValues(configurationProvider, fullKey))
                {
                    yield return configEntry;
                }
            }
            else
            {
                yield return new KeyValuePair<string, string>(fullKey, value);
            }
        }
    }

    public static JToken GetConfigAsJson(this IConfiguration config)
    {
        JObject jObject = new JObject();
        foreach (var child in config.GetChildren())
        {
            if (child.Path.EndsWith(":0"))
            {
                var arr = new JArray();
                foreach (var arrayChild in config.GetChildren())
                {
                    arr.Add(GetConfigAsJson(arrayChild));
                }

                return arr;
            }

            jObject.Add(child.Key, GetConfigAsJson(child));
        }

        if (!jObject.HasValues && config is IConfigurationSection section)
        {
            if (bool.TryParse(section.Value, out bool boolean))
            {
                return new JValue(boolean);
            }

            if (long.TryParse(section.Value, out long cardinal))
            {
                return new JValue(cardinal);
            }

            if (decimal.TryParse(section.Value, NumberStyles.Float, new NumberFormatInfo() {NumberDecimalSeparator = "."}, out decimal real))
            {
                return new JValue(real);
            }

            return new JValue(section.Value);
        }

        return jObject;
    }
}