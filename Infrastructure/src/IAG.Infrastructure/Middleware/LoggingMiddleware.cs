using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.Logging;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IO;

namespace IAG.Infrastructure.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

    public LoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<LoggingMiddleware>();
        _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
    }

    public async Task Invoke(HttpContext context)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
            await LogRequest(context);
        if (_logger.IsEnabled(LogLevel.Debug))
            await LogResponse(context);
        else
            await _next(context);
    }

    private async Task LogRequest(HttpContext context)
    {
        var message = new StringBuilder();
        var request = context.Request;
        var requestMethod = !string.IsNullOrEmpty(request.Method)
            ? request.Method.ToUpper(CultureInfo.InvariantCulture) + " "
            : string.Empty;
        message.Append("Received ").Append(requestMethod).Append("request '").Append(context.TraceIdentifier)
            .Append("' to '").Append(request.GetDisplayUrl())
            .Append($" with {request.ContentLength ?? 0} bytes of data of type {request.ContentType}");
        foreach (var (key, value) in request.Headers)
        {
            message.AppendLine(key + ": " + value);
        }

        if (RequestResponseLogger.IsContentText(request.ContentType))
        {
            request.EnableBuffering();
            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await request.Body.CopyToAsync(requestStream);
            message.AppendLine(ReadStreamInChunks(requestStream));
            request.Body.Position = 0;
        }

        _logger.LogDebug(message.ToString());
    }

    private async Task LogResponse(HttpContext context)
    {
        var response = context.Response;
        var originalBodyStream = response.Body;

        await using var responseBody = _recyclableMemoryStreamManager.GetStream();
        response.Body = responseBody;

        await _next(context);

        var message = new StringBuilder();
        message.Append($"TraceIdentifier:{context.Request.Scheme} {context.TraceIdentifier}");

        if (Enum.IsDefined(typeof(HttpStatusCode), response.StatusCode))
        {
            var statusCode = (HttpStatusCode)response.StatusCode;
            message.AppendFormat("'{0:g}' ", statusCode);
        }

        message.AppendFormat("({0:d})", response.StatusCode);
        string bodyMsg;
        if (RequestResponseLogger.IsContentText(response.ContentType))
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            bodyMsg = await new StreamReader(response.Body).ReadToEndAsync();
        }
        else
        {
            bodyMsg = $"{response.ContentType} with {response.Body.Length} bytes of data";
        }
        response.Body.Seek(0, SeekOrigin.Begin);

        message.AppendLine($"{Environment.NewLine}Response Body: {bodyMsg}");
        _logger.LogDebug(message.ToString());

        await responseBody.CopyToAsync(originalBodyStream);
    }

    private static string ReadStreamInChunks(Stream stream)
    {
        const int readChunkBufferLength = 4096;

        stream.Seek(0, SeekOrigin.Begin);

        using var textWriter = new StringWriter();
        using var reader = new StreamReader(stream);

        var readChunk = new char[readChunkBufferLength];
        int readChunkLength;

        do
        {
            readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
            textWriter.Write(readChunk, 0, readChunkLength);
        } while (readChunkLength > 0);

        return textWriter.ToString();
    }
}