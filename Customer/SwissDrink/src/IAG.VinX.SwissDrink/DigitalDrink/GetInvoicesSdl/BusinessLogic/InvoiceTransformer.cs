
using System;
using System.Collections.Generic;
using System.Linq;

using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.Basket.Enum;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess.Dto;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.Dto;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.BusinessLogic;

public class InvoiceTransformer
{
    private readonly IDdInvoiceAccess _ddInvoiceAccess;
    private readonly IReceiptCalculator _receiptCalculator;

    #region public

    public InvoiceTransformer(IDdInvoiceAccess ddInvoiceAccess, IReceiptCalculator receiptCalculator)
    {
        _ddInvoiceAccess = ddInvoiceAccess;
        _receiptCalculator = receiptCalculator;
    }

    public Receipt GetDebtorReceipt(DdInvoiceSdl invoice, string externalId, int adrIdZfv)
    {
        var debtorAddress = _ddInvoiceAccess.GetAddress().First(a => a.Id == adrIdZfv);
        decimal.TryParse(invoice.CustomerNumber, out var addressNumber);
        var deliveryAddress = _ddInvoiceAccess.GetAddress().FirstOrDefault(adr => adr.Number == addressNumber);
        if (deliveryAddress == null)
        {
            throw new ApplicationException($"Debtor for number {invoice.CustomerNumber} not found");
        }
        var glnDec = decimal.Parse(invoice.WholeSalerGln);
        var receipt = CreateReceipt(invoice, externalId);
        receipt.VatCalculation = debtorAddress.VatCalculation;
        receipt.Type = ReceiptType.Invoice;
        receipt.Number = _ddInvoiceAccess.GetNextReceiptNumber(ReceiptType.Invoice);
        receipt.WholeSalerGln = glnDec;
        receipt.DeliveryAddressId = deliveryAddress.Id;
        receipt.AddressId = adrIdZfv;
        receipt.ConditionAddressId = adrIdZfv;
        receipt.InvoiceAddressId = adrIdZfv;
        receipt.PaymentConditionId = debtorAddress.PaymentConditionId;
        receipt.PaymentMeansId = debtorAddress.PaymentMeansSellId;
        receipt.Language = debtorAddress.Language;
        receipt.DueDate = invoice.InvoiceDate.AddDays(debtorAddress.PaymentDays);
        receipt.PriceGroupId = debtorAddress.PriceGroupId;

        return AddPositionsAndCalculate(invoice, receipt);
    }

    public Receipt GetCreditorReceipt(DdInvoiceSdl invoice, string externalId)
    {
        decimal.TryParse(invoice.WholeSalerGln, out var glnDec);
        var creditorAddress = _ddInvoiceAccess.GetAddress().FirstOrDefault(adr => adr.Gln == glnDec);
        if (creditorAddress == null)
        {
            throw new ApplicationException($"Creditor for GLN {invoice.WholeSalerGln} not found");
        }

        var receipt = CreateReceipt(invoice, externalId);
        receipt.VatCalculation = creditorAddress.VatCalculation;
        receipt.Type = ReceiptType.CreditNote;
        receipt.Number = _ddInvoiceAccess.GetNextReceiptNumber(ReceiptType.CreditNote);
        receipt.AddressId = creditorAddress.Id;
        receipt.SupplierId = creditorAddress.Id;
        receipt.PaymentConditionId = creditorAddress.PaymentConditionId;
        receipt.DueDate = invoice.InvoiceDate.AddDays(creditorAddress.PaymentDays);
        receipt.PaymentMeansId = creditorAddress.PaymentMeansPurchaseId;
        receipt.Language = creditorAddress.Language;
        receipt.PaymenReference = invoice.PaymentReferenceNumber;
        receipt.PriceGroupId = creditorAddress.PriceGroupId;
        return AddPositionsAndCalculate(invoice, receipt);
    }

    #endregion

    #region private

    private Receipt CreateReceipt(DdInvoiceSdl invoice, string externalId)
    {
        var receipt = new Receipt
        {
            ReceiptState = ReceiptStatusEnum.InProgress,
            DiscountKind = 20,
            CurrencyId = 1,
            ExchangeRate = 1,
            ExternalId = externalId,
            ReceiptDate = invoice.InvoiceDate,
            InvoiceDate = invoice.InvoiceDate,
            DeliveryDate = invoice.DeliveryDate,
            DeliveryNumber = invoice.DeliveryNumber,
            OrderDate = invoice.OrderDate,
            TotalDd = invoice.TotalValue,
            PricingDate = invoice.DeliveryDate
        };
        return receipt;
    }

    private Receipt AddPositionsAndCalculate(DdInvoiceSdl invoice, Receipt receipt)
    {
        AddReceiptPosition(receipt, invoice.ArticlePositions);
        AddReturnedPackagesWithoutFees(receipt, invoice.PackagePositions);
        _receiptCalculator.CalculatePrices(receipt);

        return receipt;
    }

    private void AddReceiptPosition(Receipt receipt, IEnumerable<DdArticlePositionSdl> positions)
    {
        foreach (var position in positions)
        {
            var article = _ddInvoiceAccess.GetArticle(position.Number);
            receipt.Positions.Add(new ReceiptPosition
            {
                SequenceNumber = position.Sequence,
                ArticleId = article.Id,
                FillingId = article.FillingId,
                PackageId = article.PackageId,
                Text = article.Description,
                QuantityPackages = position.Quantity,
                QuantityUnits = position.UnitQuantity * (article.IsTank ? article.FillingsPerPackage : 1),
                VatId = receipt.VatContextSell ? article.VatSellId : article.VatBuyId,
                VatRate = receipt.VatContextSell ? article.VatSellRate : article.VatBuyRate,
                IsNet = true,
                PriceCalculationKind = PriceCalculationKind.Calculated,
                DdTotal = position.TotalPrice,
                DdPrice = position.UnitPrice
            });
        }
    }

    private void AddReturnedPackagesWithoutFees(Receipt receipt, IEnumerable<DdPackagePositionSdl> positions)
    {
        foreach (var position in positions)
        {
            var article = _ddInvoiceAccess.GetArticle(position.Number);
            if (article.ArticleKind == ArticleCategoryKind.Fee || position.QuantityReturned == 0)
                continue;

            receipt.Packages.Add(new PackagePosition
            {
                ArticleId = article.Id,
                QuantityDelivered = 0,
                QuantityReturned = position.QuantityReturned,
                VatId = receipt.VatContextSell ? article.VatSellId : article.VatBuyId,
                VatRate = receipt.VatContextSell ? article.VatSellRate : article.VatBuyRate,
                UnitPrice = article.BasePrice?? 0,
                DdPrice = position.UnitPrice
            });
        }
    }

    #endregion
}