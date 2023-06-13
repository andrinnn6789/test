using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Rest.Authentication;

namespace IAG.Infrastructure.Rest;

[ExcludeFromCodeCoverage]
public class HttpConfig : IHttpConfig
{
    public string BaseUrl { get; set; }

    public IAuthentication Authentication { get; set; }

    public IDictionary<string, string> HttpHeaders { get; set; }
}