using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.Rest;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

using Newtonsoft.Json.Linq;

namespace IAG.Infrastructure.Formatter;

public class CsvOutputFormatter : TextOutputFormatter
{
    public CsvOutputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ContentTypes.TextCsv));

        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    [ExcludeFromCodeCoverage]
    public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        if (context.Object == null)
        {
            return Task.CompletedTask;
        }

        var csv = ConvertToCsv(context.Object);
        return context.HttpContext.Response.WriteAsync(csv);
    }

    public string ConvertToCsv(object response)
    {
        IEnumerable<JObject> jsonList = response as IEnumerable<JObject>;
        if (jsonList == null)
        {
            jsonList = (response as IEnumerable<object>)?.Select(JObject.FromObject);
        }

        var csvConverter = new JsonToCsv();
        return csvConverter.ConvertToCsv(jsonList?.ToList());
    }

    [ExcludeFromCodeCoverage]
    protected override bool CanWriteType(Type type)
    {
        if (typeof(IEnumerable<object>).IsAssignableFrom(type))
        {
            return base.CanWriteType(type);
        }

        return false;
    }
}