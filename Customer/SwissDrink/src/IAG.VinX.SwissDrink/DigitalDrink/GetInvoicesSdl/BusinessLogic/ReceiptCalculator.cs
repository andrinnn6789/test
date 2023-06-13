using System.Linq;

using IAG.Infrastructure.Formatter;
using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Basket.Dto;
using IAG.VinX.Basket.Enum;
using IAG.VinX.Basket.Interface;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess.Dto;

using PackagePosition = IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess.Dto.PackagePosition;

namespace IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.BusinessLogic;

public class ReceiptCalculator : IReceiptCalculator
{
    private const decimal RoundingTemplate = 0.01m;
    private const decimal RoundingTemplateCharges = 0.0001m;

    private readonly IBasketCalculator<OnlineAddress, Basket<OnlineAddress>, OnlineOrder> _basketCalculator;

    #region public

    public ReceiptCalculator(IBasketCalculator<OnlineAddress, Basket<OnlineAddress>, OnlineOrder> basketCalculator)
    {
        _basketCalculator = basketCalculator;
    }

    public  void CalculatePrices(Receipt receipt)
    { 
        var basket = CreateBasket(receipt);
        _basketCalculator.Calculate(basket);
        TransferPricesToReceipt(receipt, basket);
    }

    #endregion

    #region private

    private void TransferPricesToReceipt(Receipt receipt, Basket<OnlineAddress> basket)
    {
        foreach (var position in receipt.Positions)
        {
            var basketPosition = basket.Positions.First(p => p.ArticleId == position.ArticleId);
            position.UnitPrice = Roundings.DecimalRounding(basketPosition.UnitPrice, RoundingTemplate);
            position.VatRateInPrice = receipt.VatCalculation == VatCalculationType.Inclusive 
                ? position.UnitPrice * basketPosition.ApplicableTaxRate
                : 0;
            // ReSharper disable once PossibleInvalidOperationException
            TransferPosCharges(receipt, basketPosition, position.VatId.Value);
            TransferPosPackages(receipt, basketPosition);
        }

        TransferBasketFees(receipt, basket);
        var returnedPackages = receipt.Packages.Sum(p => p.QuantityReturned * p.UnitPrice);
        receipt.TotalArticles = basket.LineTotalAmount;
        receipt.TotalFee = basket.ChargeTotalAmount;
        receipt.TotalPackages = basket.PackagesTotalAmount - returnedPackages;
        receipt.TotalVat = basket.TaxTotalAmount;
        receipt.TotalDiscounts = basket.DiscountTotalAmount;
        receipt.TotalNet = basket.GrandTotalAmount - returnedPackages;
    }

    private void TransferBasketFees(Receipt receipt, Basket<OnlineAddress> basket)
    {
        foreach (var feePos in basket.Charges.Where(c => c.PosType == OrderPositionType.Fee))
        {
            var chargePos = FindOrCreatePackagePosition(receipt, feePos.ArticleId, feePos.VatId);

            chargePos.QuantityDelivered += feePos.BilledQuantity;
            chargePos.UnitPrice = Roundings.DecimalRounding(feePos.UnitPrice, RoundingTemplateCharges);
            chargePos.VatRate = feePos.ApplicableTaxRate;
            chargePos.VatRateInPrice = receipt.VatCalculation == VatCalculationType.Inclusive 
                ? chargePos.UnitPrice * feePos.ApplicableTaxRate
                : 0;
        }
    }

    private void TransferPosCharges(Receipt receipt, BasketPosition basketPosition, int vatId)
    {
        foreach (var charge in basketPosition.Charges)
        {
            var chargePos = FindOrCreatePackagePosition(receipt, charge.ArticleId, vatId);

            chargePos.QuantityDelivered += charge.BilledQuantity;
            chargePos.UnitPrice = Roundings.DecimalRounding(charge.UnitPrice, RoundingTemplateCharges);
            chargePos.VatRate = charge.ApplicableTaxRate;
            chargePos.VatRateInPrice = receipt.VatCalculation == VatCalculationType.Inclusive 
                ? chargePos.UnitPrice * charge.ApplicableTaxRate
                : 0;
        }
    }

    private void TransferPosPackages(Receipt receipt, BasketPosition basketPosition)
    {
        foreach (var package in basketPosition.Packages)
        {
            var packagePos = FindOrCreatePackagePosition(receipt, package.ArticleId, package.VatId);

            packagePos.QuantityDelivered += package.BilledQuantity;
            packagePos.UnitPrice = Roundings.DecimalRounding(package.UnitPrice, RoundingTemplate);
            packagePos.VatRate = 0;
            packagePos.VatRateInPrice = 0;
        }
    }

    private static PackagePosition FindOrCreatePackagePosition(Receipt receipt, int articleId, int vatId)
    {
        var packagePos = receipt.Packages.FirstOrDefault(p => p.ArticleId == articleId && p.VatId == vatId);
        if (packagePos == null)
        {
            packagePos = new PackagePosition
            {
                ArticleId = articleId,
                VatId = vatId
            };
            receipt.Packages.Add(packagePos);
        }

        return packagePos;
    }

    private Basket<OnlineAddress> CreateBasket(Receipt receipt)
    {
        var basket = new Basket<OnlineAddress>
        {
            Action = BasketActionType.Calculate,
            ConditionAddressId = receipt.AddressId,
            VatCalculation = receipt.VatCalculation,
            ValidDate = receipt.PricingDate,
            VatContextSell = receipt.VatContextSell
        };
        foreach (var position in receipt.Positions)
        {
            basket.Positions.Add(
                new BasketPosition
                {
                    BasketParams = basket,
                    ArticleId = position.ArticleId,
                    OrderedQuantity = position.QuantityUnits
                });    
        }
        return basket;
    }

    #endregion
}