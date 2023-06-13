using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.Infrastructure.Rest;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.Dto;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.HttpAccess;

public interface IInvoiceClient
{
    void InitClient(IRequestResponseLogger logger, IHttpConfig httpConfig);

    Task<IEnumerable<DdInvoiceSdl>> GetInvoicesSdl(DateTime createdAt);
}