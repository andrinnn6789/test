using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Resource;

using Newtonsoft.Json;

namespace IAG.Infrastructure.Rest;

public class JsonRestResponse : IRestResponse
{
    private string _httpContent;

    public JsonRestResponse(HttpResponseMessage httpResponse)
    {
        HttpResponse = httpResponse;
    }

    protected JsonConverter DeserializeConverter { get; set; }

    public HttpStatusCode StatusCode => HttpResponse.StatusCode;

    public HttpResponseHeaders Headers => HttpResponse.Headers;

    public bool IsSuccessStatusCode => HttpResponse.IsSuccessStatusCode;

    protected HttpResponseMessage HttpResponse { get; }

    public async Task<string> GetContent()
    {
        return _httpContent ??= await HttpResponse.Content.ReadAsStringAsync();
    }

    public virtual async Task CheckResponse()
    {
        if (!HttpResponse.IsSuccessStatusCode)
        {
            string msg = await GetErrorMessage();
            var ex = new RestException(StatusCode, msg);
            var request = HttpResponse.RequestMessage;
            if (request != null)
            {
                if (request.Content != null)
                    ex.AdditionalInfo.Add(request.Content.ReadAsStringAsync().Result);
                if (request.RequestUri != null)
                    ex.AdditionalInfo.Add(request.RequestUri.ToString());
            }

            throw ex;
        }
    }

    public async Task<T> GetData<T>()
    {
        string content = await GetContent();
        try
        {
            return DeserializeResponse<T>(content);
        }
        catch (System.Exception ex)
        {
            throw new LocalizableException(ResourceIds.JsonRestResponseDeserializeError, ex);
        }
    }

    public virtual async Task<string> GetErrorMessage()
    {
        return await GetContent();
    }

    public virtual T DeserializeResponse<T>(string content)
    {
        return DeserializeConverter == null ? 
            JsonConvert.DeserializeObject<T>(content) : 
            JsonConvert.DeserializeObject<T>(content, DeserializeConverter);
    }
}