using System.Collections.Generic;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace IAG.Infrastructure.Startup.Extensions;

public static class JsonConverterHelper
{
    private static void AddConverters(IList<JsonConverter> converters)
    {
        converters.Add(new JsonStringEnumConverter());
    }

    public static JsonSerializerOptions GetDefaulOption()
    {
        var opt = new JsonSerializerOptions();
        AddConverters(opt.Converters);
        return opt;
    }
}