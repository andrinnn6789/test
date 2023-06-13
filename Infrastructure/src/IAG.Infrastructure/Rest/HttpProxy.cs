using System;
using System.Net;

namespace IAG.Infrastructure.Rest;

public class HttpProxy : IWebProxy
{
    public HttpProxy(string proxyUrl)
    {
        ProxUrl = new Uri(proxyUrl);
    }

    public ICredentials Credentials { get; set; }

    public Uri ProxUrl { get; }

    public Uri GetProxy(Uri destination)
    {
        return ProxUrl;
    }

    public bool IsBypassed(Uri host)
    {
        return false;
    }
}