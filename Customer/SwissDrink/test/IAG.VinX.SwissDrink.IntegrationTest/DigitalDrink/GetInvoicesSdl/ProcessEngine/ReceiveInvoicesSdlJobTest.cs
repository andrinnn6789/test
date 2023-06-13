using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.Infrastructure.ProcessEngine.Execution;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.BaseData.Dto.Sybase;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.DataAccess;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.Dto;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.HttpAccess;
using IAG.VinX.SwissDrink.DigitalDrink.GetInvoicesSdl.ProcessEngine;

using Moq;

using Xunit;

namespace IAG.VinX.SwissDrink.IntegrationTest.DigitalDrink.GetInvoicesSdl.ProcessEngine;

public class ReceiveInvoicesSdlJobTest
{

    [Fact]
    public void ReceiveInvoicesSdlJob()
    {
        var mockConnection = new Mock<ISybaseConnection>();
        mockConnection.Setup(m => m.GetQueryable<ProviderSettingVx>()).Returns(
            new List<ProviderSettingVx>
            {
                new()
                {
                    DataProvider = DataProvider.DigitalDrinkMiddleware
                }
            }.AsQueryable);
        var connectionFactory = SybaseConnectionFactoryHelper.CreateFactoryMock(mockConnection.Object);
        var invoiceClientMock = new Mock<IInvoiceClient>();
        invoiceClientMock.Setup(m => m.GetInvoicesSdl(It.IsAny<DateTime>())).Returns(
            Task.FromResult<IEnumerable<DdInvoiceSdl>>(
                new List<DdInvoiceSdl>
                {
                    new()
                    {
                        OrderNumber = "123",
                        CreatedAt = DateTime.Today
                    },
                    new()
                    {
                        OrderNumber = "ex"
                    }
                }));
        var invoiceAccessMock = new Mock<IDdInvoiceAccess>();
        invoiceAccessMock
            .Setup(m => m.CreateInvoice(It.Is<DdInvoiceSdl>(i => i.OrderNumber == "ex"), It.IsAny<int>()))
            .Throws(new ApplicationException());

        var job = new ReceiveInvoicesSdlJob(
            new MockILogger<ReceiveInvoicesSdlJob>(),
            connectionFactory,
            invoiceClientMock.Object,
            invoiceAccessMock.Object
        );

        var jobInfrastructureMock = new Mock<IJobInfrastructure>();
        jobInfrastructureMock.Setup(m => m.GetJobData<ReceiveInvoicesSdlJobData>()).Returns(new ReceiveInvoicesSdlJobData());
        job.Execute(jobInfrastructureMock.Object);
        Assert.True(job.Result.Result == JobResultEnum.PartialSuccess);
        Assert.True(job.Result.ErrorCount == 1);
    }

    [Fact]
    public void ReceiveInvoicesSdlJobConfig()
    {
        _ = new ReceiveInvoicesSdlJobConfig().AddressIdZfv = 1;
    }
}