using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using IAG.Infrastructure.Rest;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.Dto;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.HttpAccess;

public class InvoiceClient : IInvoiceClient
{
    private RestClient _restClient;

    public void InitClient(IRequestResponseLogger logger, IHttpConfig httpConfig)
    {
        _restClient = new RestClient(httpConfig, logger);
    }

    public async Task<IEnumerable<DdInvoiceSdl>> GetInvoicesSdl(DateTime createdAt)
    {
        var request = new JsonRestRequest(HttpMethod.Get, "mid/invoice-sdl/");
        request.SetQueryParameter("created_from", createdAt.ToString("o"));
        request.SetQueryParameter("limit", 100);
        request.SetQueryParameter("offset", 0);
        var result = await _restClient.ExecuteAsync(request);

        await result.CheckResponse();
        var data = await result.GetData<DdResultResponse<DdInvoiceSdl>>();
        return data.Results;
    }
}