using Microsoft.AspNetCore.Http;

using System;

namespace IAG.InstallClient.Authorization;

public static class BearerTokenCookieHandler
{
    private const string BearerTokenCookieName = "Bearer-Token";

    public static string GetBearerToken(HttpContext context)
    {
        return context.Request.Cookies[BearerTokenCookieName];
    }

    public static void SetBearerToken(HttpContext context, string token)
    {
        context.Response.Cookies.Append(BearerTokenCookieName, token, new CookieOptions()
        {
            Expires = DateTime.UtcNow.AddDays(1)
        });
    }

    public static void ClearBearerToken(HttpContext context)
    {
        context.Response.Cookies.Append(BearerTokenCookieName, string.Empty, new CookieOptions()
        {
            Expires = DateTime.UtcNow.AddDays(-1)
        });
    }
}