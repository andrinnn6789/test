using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.Common.TestHelper.DataLayerSybase;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.Basket.Enum;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.BusinessLogic;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess.Dto;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.Dto;

using Moq;

using Xunit;

namespace IAG.VinX.SwissDrink.IntegrationTest.DigitalDrink.GetInvoicesSdl.DataAccess;

public class DdInvoiceAccessTest
{
    private readonly DdInvoiceAccess _invoiceAccess;
    private readonly ISybaseConnection _connection;

    public DdInvoiceAccessTest()
    {
        _connection = SybaseConnectionFactoryHelper.CreateFactory()
            .CreateConnection();
        var calculatorMock = new Mock<IReceiptCalculator>();
        _invoiceAccess = new DdInvoiceAccess(_connection, calculatorMock.Object);
    }

    [Fact]
    public void CreateInvoice()
    {
        SybaseTransctionHelper.ExecuteInRollbackTransaction(_connection, () =>
        {
            var adr = _connection.GetQueryable<AddressSimple>().First();
            var art = _connection.GetQueryable<Article>().First();
            var artPack = _connection.GetQueryable<Article>().First(a => a.ArticleKind == ArticleCategoryKind.Package);
            var invoice = new DdInvoiceSdl
            {
                InvoiceNumber = "123", 
                CustomerNumber = adr.Number.ToString("##########"),
                TotalValue = 2,
                WholeSalerGln = adr.Gln.ToString(CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.'),
                ArticlePositions = new List<DdArticlePositionSdl>
                {
                    new ()
                    {
                        Number = art.Number.ToString("##########"),
                        Sequence = 1,
                        Quantity = 2,
                        UnitPrice = 2,
                        UnitQuantity = 10
                    }
                },
                PackagePositions = new List<DdPackagePositionSdl>
                {
                    new ()
                    {
                        Number = art.Number.ToString("##########"),
                        UnitPrice = 1
                    },
                    new ()
                    {
                        Number = artPack.Number.ToString("##########"),
                        QuantityDelivered = 2,
                        QuantityReturned = 3,
                        UnitPrice = 1
                    }
                }
            };
            Assert.True(_invoiceAccess.CreateInvoice(invoice, adr.Id));
            Assert.False(_invoiceAccess.CreateInvoice(invoice, adr.Id));
        });
    }

    [Fact]
    public void CreateInvoiceFailAdrDelivery()
    {
        var adr = _connection.GetQueryable<AddressSimple>().First();
        Assert.ThrowsAny<Exception>(() => _invoiceAccess.CreateInvoice(
            new DdInvoiceSdl
            {
                WholeSalerGln = adr.Gln.ToString(CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.'),
            }, adr.Id));
    }

    [Fact]
    public void CreateInvoiceFailAdrGln()
    {
        var adr = _connection.GetQueryable<AddressSimple>().First();
        Assert.ThrowsAny<Exception>(() => _invoiceAccess.CreateInvoice(
            new DdInvoiceSdl
            {
                CustomerNumber = adr.Number.ToString("##########")
            }, 0));
    }
}