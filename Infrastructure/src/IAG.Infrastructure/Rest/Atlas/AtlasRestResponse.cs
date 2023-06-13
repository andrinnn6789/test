using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.Rest.Atlas.Dto;

namespace IAG.Infrastructure.Rest.Atlas;

public class AtlasRestResponse : JsonRestResponse
{
    public AtlasRestResponse(HttpResponseMessage httpResponse) : base(httpResponse)
    {
        DeserializeConverter = new JsonTrimmingConverter();
    }
        
    public override async Task<string> GetErrorMessage()
    {
        string errorMsg = null;
        try
        {
            var errorResponse = await GetData<AtlasResponse<BaseResource>>();
            errorMsg = errorResponse.Resource.FirstOrDefault()?.Message;
        }
        catch (System.Exception)
        {
            // ignored
        }

        if (string.IsNullOrEmpty(errorMsg))
        {
            errorMsg = await base.GetErrorMessage();
        }

        return errorMsg;
    }
}