using System;
using System.Collections.Generic;
using System.Linq;

using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Basket.Dto;
using IAG.VinX.Basket.Enum;
using IAG.VinX.Basket.Interface;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.BusinessLogic;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess.Dto;

using Moq;

using Xunit;
using PackagePosition = IAG.VinX.Basket.Dto.PackagePosition;

namespace IAG.VinX.SwissDrink.IntegrationTest.DigitalDrink.GetInvoicesSdl.BusinessLogic;

public class ReceiptCalculatorTest
{
    private readonly ReceiptCalculator _calculator;

    public ReceiptCalculatorTest()
    {
        var baskeCalculatorMock = new Mock<IBasketCalculator<OnlineAddress, Basket<OnlineAddress>, OnlineOrder>>();
        baskeCalculatorMock.Setup(m => m.Calculate(It.IsAny<Basket<OnlineAddress>>())).Callback((Basket<OnlineAddress> basket) =>
        {
            var position = basket.Positions[0];
            position.UnitPrice = 1;
            position.Packages.Add(new PackagePosition());
            position.Charges.Add(new ChargePosition
            {
                ArticleId = 5
            });
            position.Article = new Article();
            position = basket.Positions[1];
            position.UnitPrice = 2;
            position.Packages.Add(new PackagePosition());
            position.Charges.Add(new ChargePosition
            {
                ArticleId = 5
            });
            position.Article = new Article();
            position = basket.Positions[2];
            position.UnitPrice = 3;
            position.Packages.Add(new PackagePosition());
            position.Charges.Add(new ChargePosition
            {
                ArticleId = 5
            });
            position.Article = new Article();

            basket.Charges.Add(new ChargePosition
            {
                ArticleId = 5,
                PosType = OrderPositionType.Fee
            });
            basket.Charges.Add(new ChargePosition
            {
                ArticleId = 5,
                PosType = OrderPositionType.Fee,
                VatId = 1
            });
            basket.Charges.Add(new ChargePosition
            {
                ArticleId = 6,
                PosType = OrderPositionType.Fee
            });

        });
        _calculator = new ReceiptCalculator(baskeCalculatorMock.Object);
    }

    [Fact]
    public void CalculatePrices()
    {
        var receipt = new Receipt
        {
            AddressId = 1,
            Type = ReceiptType.Invoice
        };
        receipt.Positions.AddRange(
            new List<ReceiptPosition>
            {
                new()
                {
                    ArticleId = 1,
                    UnitPrice = 1,
                    QuantityUnits = 10,
                    QuantityPackages = 2,
                    VatId = 1
                },
                new()
                {
                    ArticleId = 2,
                    UnitPrice = 3,
                    QuantityUnits = 10,
                    QuantityPackages = 2,
                    VatId = 1
                },
                new()
                {
                    ArticleId = 3,
                    UnitPrice = 3,
                    QuantityUnits = 10,
                    QuantityPackages = 2,
                    VatId = 3
                }
            }
        );
        receipt.Packages.AddRange(
            new List<SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess.Dto.PackagePosition>
            {
                new()
                {
                    ArticleId = 99,
                    UnitPrice = 2,
                    QuantityDelivered = 10,
                    QuantityReturned = 2
                },
                new()
                {
                    ArticleId = 99,
                    UnitPrice = 2,
                    QuantityDelivered = 10,
                    QuantityReturned = 2
                },
            }
        );
        _calculator.CalculatePrices(receipt);
        Assert.Equal(0, receipt.TotalDd);
        Assert.Equal(1, receipt.Positions.First().UnitPrice);
        Assert.Equal(7, receipt.Packages.Count); // 3x fee + 2x package + 2x charge
    }

    [Fact]
    public void VatContextType()
    {
        var receipt = new Receipt();
        Assert.Throws<ApplicationException>(() => receipt.VatContextSell);
        receipt.Type = ReceiptType.CreditNote;
        Assert.False(receipt.VatContextSell);
        receipt.Type = ReceiptType.Invoice;
        Assert.True(receipt.VatContextSell);
    }
}