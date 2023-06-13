using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.Rest;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.LGAVRest;

public abstract class BaseClient<TData, TResult> : RestClient
{
    protected BaseClient(IHttpConfig config, string resourcePath, IRequestResponseLogger logger) : base(config, logger)
    {
        ResourcePath = resourcePath;
    }

    protected string ResourcePath { get; }

    public async Task<TResult> Post(TData data)
    {
        var request = new JsonRestRequest(HttpMethod.Post, ResourcePath);
        var jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling =
                DefaultValueHandling.Include,
            ContractResolver = new DefaultContractResolver()
        };

        request.SetJsonBody(data, jsonSerializerSettings);

        var response = await ExecuteAsync(request);
        await response.CheckResponse();

        return await response.GetData<TResult>();
    }
}