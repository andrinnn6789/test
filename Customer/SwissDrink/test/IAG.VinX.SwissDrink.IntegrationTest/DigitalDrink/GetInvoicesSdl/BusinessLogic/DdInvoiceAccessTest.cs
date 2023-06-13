using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.BusinessLogic;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.Dto;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.HttpAccess;

using Moq;

using Xunit;

namespace IAG.VinX.SwissDrink.IntegrationTest.DigitalDrink.GetInvoicesSdl.BusinessLogic;

public class ReceiveInvoicesSdlLogicTest
{
    private readonly ReceiveInvoicesSdlLogic _logic;

    private readonly Mock<IDdInvoiceAccess> _invoiceAccessMock;
    private readonly Mock<IInvoiceClient> _invoiceClientMock;

    public ReceiveInvoicesSdlLogicTest()
    {
        _invoiceAccessMock = new Mock<IDdInvoiceAccess>();
        _invoiceClientMock = new Mock<IInvoiceClient>();

        _logic = new ReceiveInvoicesSdlLogic(_invoiceAccessMock.Object, _invoiceClientMock.Object);
    }

    [Fact]
    public async Task ReceiveInvoicesAsyncTest()
    {
        _invoiceClientMock.Setup(m => m.GetInvoicesSdl(It.IsAny<DateTime>()))
            .ReturnsAsync((DateTime _) => new List<DdInvoiceSdl> { new () });

        var invoices = await _logic.ReceiveInvoicesAsync(DateTime.Today);

        Assert.NotEmpty(invoices);
    }

    [Fact]
    public async Task ReceiveInvoicesAsyncExceptionTest()
    {
        _invoiceClientMock.Setup(m => m.GetInvoicesSdl(It.IsAny<DateTime>())).ThrowsAsync(new Exception());

        await Assert.ThrowsAsync<LocalizableException>(() => _logic.ReceiveInvoicesAsync(DateTime.Today));
    }


    [Fact]
    public void ProcessInvoiceTest()
    {
        _invoiceAccessMock
            .Setup(m => m.CreateInvoice(It.IsAny<DdInvoiceSdl>(), It.IsAny<int>()));

        _logic.ProcessInvoice(new DdInvoiceSdl(), 0);
    }

    [Fact]
    public void ProcessInvoiceExceptionTest()
    {
        _invoiceAccessMock.Setup(m => m.CreateInvoice(It.IsAny<DdInvoiceSdl>(), It.IsAny<int>())).Throws<Exception>();

        Assert.Throws<LocalizableException>(() => _logic.ProcessInvoice(new DdInvoiceSdl(), 0));
    }
}