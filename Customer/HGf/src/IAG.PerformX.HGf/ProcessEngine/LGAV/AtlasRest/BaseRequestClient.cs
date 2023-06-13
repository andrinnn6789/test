using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Atlas;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest;

public abstract class BaseRequestClient<T> : AtlasClient
{
    protected BaseRequestClient(IHttpConfig config, string resourcePath, IRequestResponseLogger logger) : base(config, logger)
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
}