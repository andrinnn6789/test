using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace IAG.Infrastructure.Formatter;

public class JsonToCsv
{
    public const char DefaultDelimiter = ';';

    public JsonToCsv(char? delimiter = null)
    {
        Delimiter = delimiter ?? DefaultDelimiter;
    }

    public char Delimiter { get; set; }

    public string ConvertToCsv(List<JObject> jsonList)
    {
        if (!jsonList.Any())
        {
            return string.Empty;
        }

        StringBuilder csvString = new StringBuilder();

        var properties = jsonList.First().Properties().Select(column => column.Name).ToList();
        csvString.AppendLine(string.Join(Delimiter, properties));

        foreach (var item in jsonList)
        {
            var values = new List<string>();
            foreach (string property in properties)
            {
                var value = item.Properties().FirstOrDefault(p => p.Name == property)?.Value.ToString() ?? string.Empty;
                value = HandleEscaping(value);

                values.Add(value);
            }

            csvString.AppendLine(string.Join(Delimiter, values));
        }

        return csvString.ToString();
    }

    private string HandleEscaping(string value)
    {
        if (value.IndexOfAny(new[] { Delimiter, '"' }) > 0)
        {
            value = $"\"{value.Replace("\"", "\\\"")}\"";
        }

        return value;
    }
}