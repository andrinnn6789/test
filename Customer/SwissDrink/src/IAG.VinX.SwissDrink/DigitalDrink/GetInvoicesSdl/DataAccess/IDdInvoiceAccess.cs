using System.Linq;

using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess.Dto;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.Dto;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess;

public interface IDdInvoiceAccess
{
    bool CreateInvoice(DdInvoiceSdl invoice, int adrIdZfv);
    Article GetArticle(string articleNumber);
    IQueryable<AddressSimple> GetAddress();
    int GetNextReceiptNumber(ReceiptType receiptType);
}