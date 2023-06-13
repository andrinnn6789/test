using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace IAG.Infrastructure.Rest.Atlas;

public class AtlasClient : RestClient
{
    public AtlasClient(IHttpConfig config, IRequestResponseLogger logger = null) : base(config, logger)
    {
    }

    public AtlasClient(IHttpConfig config, HttpProxy proxy, IRequestResponseLogger logger = null) : base(config, proxy, logger)
    {
    }

    public IHttpConfig AtlasConfiguration => Configuration;

    [ExcludeFromCodeCoverage]      // can only be tested by integration tests!
    public override async Task<IRestResponse> ExecuteAsync(JsonRestRequest request)
    {
        PrepareRequest(request);

        var httpResponse = await SendAsync(request);
        return new AtlasRestResponse(httpResponse);
    }
}