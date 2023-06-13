using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Atlas;

namespace IAG.PerformX.HGf.ProcessEngine.LGAV.AtlasRest;

public abstract class BaseResultClient<T> : AtlasClient
{
    protected BaseResultClient(IHttpConfig config, string resourcePath, IRequestResponseLogger logger) : base(config, logger)
    {
        ResourcePath = resourcePath;
    }

    protected string ResourcePath { get; }

    public async Task Put(T data, int id)
    {
        var request = new JsonRestRequest(HttpMethod.Put, ResourcePath + "/{id}");
        request.SetUrlSegment("id", id.ToString());
        request.SetJsonBody(data);

        var response = await ExecuteAsync(request);
        await response.CheckResponse();
    }
}