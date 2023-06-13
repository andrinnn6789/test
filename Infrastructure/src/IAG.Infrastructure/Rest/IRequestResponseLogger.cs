using System.Net.Http;

namespace IAG.Infrastructure.Rest;

public interface IRequestResponseLogger
{
    void LogRequest(string root, HttpRequestMessage request);
    void LogResponse(HttpResponseMessage httpResponse);
}