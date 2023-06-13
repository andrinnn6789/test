using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using IAG.VinX.Schüwo.SV.Dto.Interface;

using JetBrains.Annotations;

namespace IAG.VinX.Schüwo.SV.BusinessLogic;

public class DataFormatter
{
    private const char DefaultDelimiter = ';';

    private char Delimiter { get; }

    public DataFormatter(char? delimiter = null)
    {
        Delimiter = delimiter ?? DefaultDelimiter;
    }

    public void FormatDataListToCsv<T>(TextWriter writer, IEnumerable<T> dataList, string dataName)
    {
        WriteDataHeader(writer, dataName);
        WriteDataList(writer, dataList);
    }

    public void FormatOrderListToCsv(TextWriter writer, IEnumerable<IOrder<IOrderPos>> orderList, string dataName)
    {
        WriteDataHeader(writer, dataName);
        foreach (var order in orderList)
        {
            WriteOrderHeader(writer, order);
            WriteDataList(writer, order.PosData);
        }
    }

    public static void WriteLineCountFooter(MemoryStream stream, TextWriter writer)
    {
        var fullString = Encoding.UTF8.GetString(stream.ToArray(), 0, (int) stream.Length);
        var lineCount = Regex.Matches(fullString, "(\r\n?|\n)").Count + 1;

        writer.WriteLine(lineCount);
        writer.Flush();
    }

    private void WriteDataHeader(TextWriter writer, string dataName)
    {
        writer.WriteLine($"{dataName}{Delimiter}-1");
        writer.Flush();
    }

    private void WriteDataList<T>(TextWriter writer, IEnumerable<T> dataList)
    {
        var propertyList = dataList.GetType().GetGenericArguments().First().GetProperties().ToList();
        WriteColumnNames(writer, propertyList);
        foreach (var dataElement in dataList)
        {
            var dataAsStrings = propertyList.Select(p => GetStringFromValue(p.GetValue(dataElement)));
            writer.WriteLine(string.Join(Delimiter, dataAsStrings));
            writer.Flush();
        }
    }

    private void WriteColumnNames(TextWriter writer, IEnumerable<PropertyInfo> propertyList)
    {
        var propertyNames = propertyList.Select(p => p.Name.ToLower());
        writer.WriteLine(string.Join(Delimiter, propertyNames));
        writer.Flush();
    }

    private void WriteOrderHeader(TextWriter writer, IOrder<IOrderPos> order)
    {
        var propertyList = order.GetType().GetProperties().ToList();
        foreach (var property in propertyList)
        {
            if (property.Name == nameof(order.PosData))
                continue;

            var dataAsString = GetStringFromValue(property.GetValue(order));
            writer.WriteLine($"{property.Name.ToLower()}{Delimiter}{dataAsString}");
            writer.Flush();
        }
    }

    private string GetStringFromValue([CanBeNull] object value)
    {
        return value switch
        {
            bool b => b ? "1" : "0",
            decimal d => d.ToString("G20", CultureInfo.InvariantCulture),
            double d => d.ToString("G20", CultureInfo.InvariantCulture),
            _ => value == null ? string.Empty : HandleEscaping(value.ToString())
        };
    }

    private string HandleEscaping(string value)
    {
        if (value.IndexOfAny(new[] {Delimiter, '"' }) > 0)
        {
            value = $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}