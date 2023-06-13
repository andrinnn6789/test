using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.Http;
using IAG.Infrastructure.IdentityServer;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.IdentityServer.Plugin;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IAG.Infrastructure.Swagger;

/// <summary>
/// Swagger-endpoint is protected with basic-auth if not local-client and auth is active
/// Authentication against all auth-plugins
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]  // only testable with full ID-server and auth-plugins
public class SwaggerAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;
    private List<IAuthenticationPlugin> _plugins;

    public SwaggerAuthMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    [UsedImplicitly]
    public async Task Invoke(HttpContext context, ILogger<SwaggerAuthMiddleware> logger)
    {
        if (context.Request.Path.StartsWithSegments("/swagger") && !HttpContextHelper.IsLocalRequest(context))
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':', 2)[0];
                var password = decodedUsernamePassword.Split(':', 2)[1];
                if (IsAuthorized(username, password))
                {
                    await _next.Invoke(context);
                    return;
                }

                logger.LogWarning($"Unauthorized access on Swagger from {context.Connection.RemoteIpAddress}/{username}");
            }

            context.Response.Headers["WWW-Authenticate"] = "Basic";
            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
        }
        else
        {
            await _next.Invoke(context);
        }
    }

    private bool IsAuthorized(string username, string password)
    {
        if (_plugins == null)
        {
            var realmHandler = _serviceProvider.GetService<IRealmHandler>();
            if (realmHandler == null)
                return true;
            _plugins = realmHandler.GetAuthPlugins();
        }

        foreach (var plugin in _plugins)
        {
            try
            {
                plugin.Authenticate(RequestTokenParameter.ForPasswordGrant(username, password));
                return true;
            }
            catch (AuthenticationFailedException)
            {
                // nok, check next plugin
            }
        }
        return false;
    }
}