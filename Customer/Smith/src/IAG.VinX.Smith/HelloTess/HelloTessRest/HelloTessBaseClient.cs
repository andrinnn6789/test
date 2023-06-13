using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.Rest;

using Newtonsoft.Json;

namespace IAG.VinX.Smith.HelloTess.HelloTessRest;

class HelloTessBaseClient<T> : RestClient, IRestClient<T>
{
    private readonly JsonSerializerSettings _jsonSerializerSetting = new()
    {
        NullValueHandling = NullValueHandling.Include,
        DefaultValueHandling = DefaultValueHandling.Include,
        ContractResolver = new JsonPropertyAnnotationContractResolver()
    };

    public HelloTessBaseClient(IHttpConfig config, string resourcePath, IRequestResponseLogger logger)
        : base(config, logger)
    {
        ResourcePath = resourcePath;
    }

    protected string ResourcePath { get; }

    public async Task<IEnumerable<T>> Get()
    {
        var request = new JsonRestRequest(HttpMethod.Get, ResourcePath);
        var response = await ExecuteAsync(request);

        await response.CheckResponse();
        return await response.GetData<IEnumerable<T>>();
    }

    public async Task Post(T body)
    {
        var request = new JsonRestRequest(HttpMethod.Post, ResourcePath);
        request.SetJsonBody(body, _jsonSerializerSetting);

        var response = await ExecuteAsync(request);
        await response.CheckResponse();
    }
}