using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;

namespace IAG.Infrastructure.Http;

public static class HttpContextHelper
{
    public static bool IsLocalRequest(HttpContext context)
    {
        if (context.Request.Headers.Any(header => header.Key.ToLower().Equals("x-forwarded-for")))
        {
            return false;
        }
        var remoteIpAddress = context.Connection.RemoteIpAddress;
        var localIpAddress = context.Connection.LocalIpAddress;
        if (remoteIpAddress == null && localIpAddress == null)
        {
            return true;
        }

        return remoteIpAddress != null && (remoteIpAddress.Equals(localIpAddress) || IPAddress.IsLoopback(remoteIpAddress));
    }
}