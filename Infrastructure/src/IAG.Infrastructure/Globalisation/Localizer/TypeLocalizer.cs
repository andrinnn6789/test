using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.Localization;

namespace IAG.Infrastructure.Globalisation.Localizer;

public class TypeLocalizer : ITypeLocalizer
{
    private readonly IStringLocalizer _localizer;

    public TypeLocalizer(IStringLocalizer localizer)
    {
        _localizer = localizer;
    }

    public string Localize(string prefix, object obj)
    {
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(obj,
            new JsonSerializerOptions
            {
                Converters = {new JsonStringEnumConverter() }
            }));
        var localized = new StringBuilder();
        AddElementData(prefix, localized, json.RootElement);
        return localized.ToString();
    }

    private void AddElementData(string prefix, StringBuilder sb, JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var sub in element.EnumerateObject())
                {
                    sb.Append(_localizer.GetString($"{prefix}{sub.Name}")).Append(": ");
                    AddElementData(prefix, sb, sub.Value);
                    sb.Append(Environment.NewLine);
                }

                break;
            case JsonValueKind.Array:
                foreach (var sub in element.EnumerateArray())
                {
                    AddElementData(prefix, sb, sub);
                }

                break;
            case JsonValueKind.String:
                sb.Append(_localizer.GetString($"{prefix}{element}"));
                break;
            case JsonValueKind.Number:
            case JsonValueKind.True:
                sb.Append(element.ToString());
                break;
            case JsonValueKind.Null:
                sb.Append('-');
                break;
        }
    }
}