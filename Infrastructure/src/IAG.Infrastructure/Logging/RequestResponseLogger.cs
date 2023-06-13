using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Resource;
using IAG.Infrastructure.Rest;

using Microsoft.Extensions.Logging;

namespace IAG.Infrastructure.Logging;

public class RequestResponseLogger : IRequestResponseLogger
{
    private readonly IMessageLogger _messageLogger;
    private readonly ILogger _logger;

    public RequestResponseLogger(ILogger logger)
    {
        _logger = logger;
    }

    public RequestResponseLogger(IMessageLogger messageLogger)
    {
        _messageLogger = messageLogger;
    }

    public RequestResponseLogger(ILogger logger, IMessageLogger messageLogger)
    {
        _logger = logger;
        _messageLogger = messageLogger;
    }

    public static bool IsContentText(string contentType)
    {
        switch (contentType?.Split(';').First().Replace("x-", string.Empty))
        {
            case ContentTypes.ApplicationAtomXml:
            case ContentTypes.ApplicationEcmascript:
            case ContentTypes.ApplicationEdiX12:
            case ContentTypes.ApplicationEdifact:
            case ContentTypes.ApplicationJson:
            case ContentTypes.ApplicationJsonPatch:
            case ContentTypes.ApplicationRdfXml:
            case ContentTypes.ApplicationSoapXml:
            case ContentTypes.ApplicationXml:
            case ContentTypes.TextCsv:
            case ContentTypes.TextHtml:
            case ContentTypes.TextPlain:
            case ContentTypes.TextXml:
                return true;
        }

        return false;
    }

    public void LogRequest(string root, HttpRequestMessage request)
    {
        var url = root == null ?
            request.RequestUri?.ToString() ?? string.Empty :
            new Uri(new Uri(root), request.RequestUri?.ToString()).ToString();

        var method = request.Method.Method;
        string content;
        if (IsContentText(request.Content?.Headers.ContentType?.MediaType))
            content = request.Content?.ReadAsStringAsync().Result ?? string.Empty;
        else
            content = $"{request.Content?.Headers.ContentLength} data of type {request.Content?.Headers.ContentType?.MediaType}";
        var headers = GetHeaders(request.Headers);

        var message = new StringBuilder();
        message.AppendLine("Send HttpRequest:");
        message.Append("\turl: ").AppendLine(url);
        message.Append("\tmethod: ").AppendLine(method);
        message.Append("\theader(s): ").AppendLine(headers);
        message.Append("\tcontent: ").Append(content);
        _logger?.LogDebug(message.ToString());
        _messageLogger?.AddMessage(new MessageStructure(MessageTypeEnum.Debug, ResourceIds.RequestMessage, message.ToString()));
    }

    public void LogResponse(HttpResponseMessage response)
    {
        var code = $"{response.StatusCode} ({(int)response.StatusCode})";
        string content;
        if (IsContentText(response.Content.Headers.ContentType?.MediaType))
            content = response.Content.ReadAsStringAsync().Result;
        else
            content = $"{response.Content.Headers.ContentLength} data of type {response.Content.Headers.ContentType?.MediaType}";
        var headers = GetHeaders(response.Headers);
        var message = new StringBuilder();
        message.AppendLine("Received HttpResponse:");
        message.Append("\tcode: ").AppendLine(code);
        message.Append("\theader(s): ").AppendLine(headers);
        message.Append("\tcontent: ").Append(content);
        _logger?.LogDebug(message.ToString());
        _messageLogger?.AddMessage(new MessageStructure(MessageTypeEnum.Debug, ResourceIds.ResponseMessage, message.ToString()));
    }

    private static string GetHeaders(HttpHeaders headers)
    {
        var headersBuilder = new StringBuilder();
        foreach (var header in headers)
        {
            headersBuilder.Append(header.Key).Append(":").Append(string.Join(',', header.Value)).Append(Environment.NewLine);
        }

        return headersBuilder.ToString().TrimEnd();
    }
}