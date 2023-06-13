using System;
using System.Data;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using IAG.Infrastructure.Cron;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.Globalisation.Localizer;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace IAG.Infrastructure.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    [UsedImplicitly]
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    [UsedImplicitly]
    public async Task Invoke(HttpContext context, ILogger<ExceptionMiddleware> logger, IStringLocalizer<ExceptionMiddleware> localizer)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (System.Exception ex)
        {
            var msg = new MessageLocalizer(localizer).LocalizeException(ex);

            logger.LogError("{msg}", msg);
            logger.LogDebug("Stack: {stack}", ex.StackTrace);

            var statusCode = 500;
            switch (ex)
            {
                case ArgumentException _:
                case NoNullAllowedException _:
                case BadRequestException _:
                case CronParsingException _:
                case JsonSerializationException _:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case AuthenticationFailedException _:
                    statusCode = (int)HttpStatusCode.Unauthorized;  // sic! status has wrong name, should be "Unauthenticated"
                    // Korrekterweise müsste hier der HTTP-Header "WWW-Authenticate" mit den möglichen Authentifizierungsmethoden zurückgegeben werden.
                    // Aber diese kennt nur der Identity-Server...
                    break;
                case AuthorizationFailedException _:
                    statusCode = (int)HttpStatusCode.Forbidden;
                    break;
                case NotFoundException _:
                case FileNotFoundException _:
                    statusCode = (int)HttpStatusCode.NotFound;
                    break;
                case DuplicateKeyException _:
                    statusCode = (int)HttpStatusCode.Conflict;
                    break;
            }

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(
                new ProblemDetails
                {
                    Type = context.Request.GetDisplayUrl(),
                    Instance = context.Request.Path,
                    Title = msg,
                    Status = statusCode
                }));
        }
    }
}