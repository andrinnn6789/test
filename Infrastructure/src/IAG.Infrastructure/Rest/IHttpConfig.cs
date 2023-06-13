using System.Collections.Generic;

using IAG.Infrastructure.Rest.Authentication;

namespace IAG.Infrastructure.Rest;

public interface IHttpConfig
{
    string BaseUrl { get; set; }

    IAuthentication Authentication { get; set; }

    IDictionary<string, string> HttpHeaders { get; set; }
}