using System;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.BusinessLogic;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess.Dto;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.Dto;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess;

public class DdInvoiceAccess : IDdInvoiceAccess
{
    private readonly ISybaseConnection _sybaseConnection;
    private readonly IReceiptCalculator _receiptCalculator;

    #region public

    public DdInvoiceAccess(ISybaseConnection sybaseConnection, IReceiptCalculator receiptCalculator)
    {
        _sybaseConnection = sybaseConnection;
        _receiptCalculator = receiptCalculator;
    }

    public bool CreateInvoice(DdInvoiceSdl invoice, int adrIdZfv)
    {
        var externalId = GetExternalId(invoice);
        if (ReceiptExists(externalId))
            return false;

        var transformer = new InvoiceTransformer(this, _receiptCalculator);
        _sybaseConnection.BeginTransaction();
        try
        {
            var creditorReceipt = transformer.GetCreditorReceipt(invoice, externalId);
            var debtorReceipt = transformer.GetDebtorReceipt(invoice, externalId, adrIdZfv);
            InsertReceipt(debtorReceipt);

            creditorReceipt.InvoiceId = debtorReceipt.Id;
            InsertReceipt(creditorReceipt);

            _sybaseConnection.Commit();
        }
        catch (Exception)
        {
            _sybaseConnection.Rollback();
            throw;
        }

        return true;
    }

    private bool ReceiptExists(string externalId)
    {
        return _sybaseConnection.GetQueryable<Receipt>().FirstOrDefault(r => r.ExternalId == externalId) != null; 
    }

    public Article GetArticle(string article)
    {
        int.TryParse(article, out var articleNumber);
        return _sybaseConnection.GetQueryable<Article>().First(a => a.Number == articleNumber);
    }

    public IQueryable<AddressSimple> GetAddress()
    {
        return _sybaseConnection.GetQueryable<AddressSimple>();
    }

    public int GetNextReceiptNumber(ReceiptType receiptType)
    {
        return _sybaseConnection.QueryValuesBySql<int>(@"
                SELECT IsNull(MAX(Bel_BelegNr), 0) + 1 FROM Beleg WHERE Bel_Belegtyp = ?
            ", (int)receiptType).First();
    }

    #endregion

    #region private

    private static string GetExternalId(DdInvoiceSdl invoice)
    {
        return $"{invoice.Id} / {invoice.InvoiceNumber}";
    }

    private void InsertReceipt(Receipt receipt)
    {
        _sybaseConnection.Insert(receipt);
        foreach (var position in receipt.Positions)
        {
            position.ReceiptId = receipt.Id;
            _sybaseConnection.Insert(position);
        }

        foreach (var position in receipt.Packages)
        {
            position.ReceiptId = receipt.Id;
            _sybaseConnection.Insert(position);
        }
    }

    #endregion
}