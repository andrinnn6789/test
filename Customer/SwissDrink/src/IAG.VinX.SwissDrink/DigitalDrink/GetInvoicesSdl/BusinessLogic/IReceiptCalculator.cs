using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess.Dto;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.BusinessLogic;

public interface IReceiptCalculator
{
    void CalculatePrices(Receipt receipt);
}