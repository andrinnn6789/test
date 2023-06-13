using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IAG.Infrastructure.Rest;

public interface IRestResponse
{
    HttpStatusCode StatusCode { get; }

    bool IsSuccessStatusCode { get; }

    HttpResponseHeaders Headers { get; }

    Task<string> GetContent();

    Task CheckResponse();

    Task<T> GetData<T>();
}