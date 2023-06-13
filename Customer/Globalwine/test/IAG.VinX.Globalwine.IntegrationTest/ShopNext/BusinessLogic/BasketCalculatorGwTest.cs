using System.Collections.Generic;

using IAG.Infrastructure.TestHelper.Globalization.Mocks;
using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Basket.BusinessLogic;
using IAG.VinX.Basket.Dto;
using IAG.VinX.Basket.Enum;
using IAG.VinX.Basket.Interface;
using IAG.VinX.Globalwine.ShopNext.Dto.Gw;

using Moq;

using Xunit;

using Article = IAG.VinX.BaseData.Dto.Sybase.Article;

namespace IAG.VinX.Globalwine.IntegrationTest.ShopNext.BusinessLogic;

public class BasketCalculatorGwTest
{
    private readonly BasketCalculator<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw> _calculator;

    public BasketCalculatorGwTest()
    {
        var mockBasketReader = new Mock<IBasketDataReader>();
        mockBasketReader.Setup(m => m.Company).Returns(new Company
        {
            SkontoAllowed = true
        });
        mockBasketReader.Setup(m => m.BasketAddress).Returns(new Address
        {
            VatCalculation = VatCalculationType.Inclusive
        });
        mockBasketReader.Setup(m => m.ShopProvider).Returns(
            new List<ProviderSettingVx>
            {
                new()
                {
                    ConditionAddressId = 1
                }
            });
        mockBasketReader.Setup(m => m.Articles).Returns(
            new List<Article>
            {
                new()
                {
                    Id = 1
                }
            });
        mockBasketReader.Setup(m => m.Stock).Returns(
            new List<StockData>
            {
                new()
                {
                    ArticleId = 2,
                    QuantityAvailable = 123,
                    WarehouseId = 1
                }
            });
        var mockPositionPriceRuler = new Mock<IPositionPriceCalculator>();
        _ = mockPositionPriceRuler.Setup(m =>
            m.GetPriceData(It.IsAny<IBasketDataReader>(), It.IsAny<PriceParameter>())).Returns(
            new List<PriceData>
            {
                new()
                {
                    ArticleId = 1,
                    QuantityOrdered = 2,
                    BasePrice = 2,
                    UnitPrice = 3,
                    VatPriceBase = VatPriceBaseType.WithVat,
                    VatRate = 10, 
                    ArticleDecription = "xxx",
                    ArticleNumber = 111
                }
            });
        mockPositionPriceRuler.SetupGet(m => m.VatPriceBaseCustomer).Returns(VatCalculationType.Inclusive);
        var mockShippingCost = new Mock<IShippingCostCalculator>();
        mockShippingCost.Setup(m => m.Calculate<OnlineAddressGw, BasketGw<OnlineAddressGw>>(It.IsAny<BasketGw<OnlineAddressGw>>())).Returns(
            new List<ChargePosition>());
        var mockPositionFeeCalculator = new Mock<IPositionFeeCalculator>();
        var mockPositionPackageCalculator = new Mock<IPositionPackageCalculator>(); 
        var mockPaymentTerms = new Mock<IOrderPaymentTerms>();
        mockPaymentTerms.Setup(m => m.GetTerms(It.IsAny<decimal>(), It.IsAny<PaymentCondition>())).Returns("terms");
        _calculator = new BasketCalculator<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw>(
            mockBasketReader.Object,
            mockPositionPriceRuler.Object,
            new Mock<IOnlineClient<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw>>().Object,
            new Mock<ICustomerDiscountCalculator>().Object,
            new Mock<IQuantityDiscountCalculator>().Object,
            mockPaymentTerms.Object,
            mockShippingCost.Object,
            mockPositionFeeCalculator.Object,
            mockPositionPackageCalculator.Object, 
            new MockLocalizer<BasketCalculator<OnlineAddressGw, BasketGw<OnlineAddressGw>, OnlineOrderGw>>()
        );
    }


    [Fact]
    public void CalculateWithDefaultCondition()
    {
        var basket = new BasketGw<OnlineAddressGw>();
        _calculator.Calculate(basket);
    }
}