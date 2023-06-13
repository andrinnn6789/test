using System;
using System.Collections.Generic;
using System.Net;

namespace IAG.Infrastructure.Rest;

public class RestException: ApplicationException
{ 
    public RestException(HttpStatusCode statusCode, string content)
    {
        StatusCode = statusCode;
        Content = content;
        AdditionalInfo = new List<string>();
    }

    public HttpStatusCode StatusCode { get; }

    public string Content { get; }

    public List<string> AdditionalInfo { get; }
}