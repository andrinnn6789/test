using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.Dto;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.HttpAccess;
using IAG.VinX.SwissDrink.Resource;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.BusinessLogic;

public class ReceiveInvoicesSdlLogic
{
    private readonly IDdInvoiceAccess _invoiceAccess;
    private readonly IInvoiceClient _invoiceClient;

    public ReceiveInvoicesSdlLogic(IDdInvoiceAccess invoiceAccess, IInvoiceClient invoiceClient)
    {
        _invoiceAccess = invoiceAccess;
        _invoiceClient = invoiceClient;
    }

    public async Task<IEnumerable<DdInvoiceSdl>> ReceiveInvoicesAsync(DateTime lastTimestamp)
    {
        try
        {
            return await _invoiceClient.GetInvoicesSdl(lastTimestamp);
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.ReceiveInvoicesSdlFailed, ex);
        }
    }

    public bool ProcessInvoice(DdInvoiceSdl invoice, int adrIdZfv)
    {
        try
        {
            return _invoiceAccess.CreateInvoice(invoice, adrIdZfv);
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.CreateInvoiceSdlFailed, ex);
        }
    }
}