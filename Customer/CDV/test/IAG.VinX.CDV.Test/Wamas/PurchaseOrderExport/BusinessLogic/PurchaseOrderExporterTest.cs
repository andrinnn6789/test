using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.DataAccess;
using IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;
using IAG.VinX.CDV.Wamas.PurchaseOrderExport.BusinessLogic;
using IAG.VinX.CDV.Wamas.PurchaseOrderExport.Dto;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Wamas.PurchaseOrderExport.BusinessLogic;

public class PurchaseOrderExporterTest
{
    [Fact]
    public void SetConfig_ShouldCreateConnection()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var ftpConfig = new WamasFtpConfig();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory => factory.CreateConnection(connectionString, It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var purchaseOrderExporter = new PurchaseOrderExporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);

        // Act
        purchaseOrderExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);


        // Assert
        fakeConnectionFactory.Verify(
            con => con.CreateConnection(connectionString, It.IsAny<IUserContext>(), It.IsAny<ILogger>()), Times.Once);
    }

    [Fact]
    public void ExportPurchaseOrders_WhenSuccess_ShouldReturnJobResultSuccess_ShouldExportFile()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var ftpConfig = new WamasFtpConfig();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PurchaseOrder>())
            .Returns(PurchaseOrderExporterTestData.GetPurchaseOrder().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PurchaseOrderReference>())
            .Returns(PurchaseOrderExporterTestData.GetPurchaseOrderReferences().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PurchaseOrderLine>())
            .Returns(PurchaseOrderExporterTestData.GetPurchaseOrderLines().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PurchaseOrderPartner>())
            .Returns(PurchaseOrderExporterTestData.GetPurchaseOrderPartners().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Document>())
            .Returns(PurchaseOrderExporterTestData.GetDocuments().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.Update(It.IsAny<Document>()));
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var purchaseOrderExporter = new PurchaseOrderExporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        purchaseOrderExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = purchaseOrderExporter.ExportPurchaseOrders(DateTime.Now.AddDays(1));

        // Assert
        Assert.Equal(JobResultEnum.Success, jobResult.Result);
        Assert.Equal(5, jobResult.ExportedCount);
        Assert.Equal(0, jobResult.ErrorCount);

        fakeFtpConnector.Verify(ftp => ftp.SetConfig(ftpConfig), Times.Once);
        fakeFtpConnector.Verify(ftp => ftp.UploadFile(It.IsAny<byte[]>(), It.Is<string>(s => s.Contains("IBD00008"))),
            Times.Once);
    }

    [Fact]
    public void ExportPurchaseOrders_WhenExceptionReadingData_ShouldReturnJobResultFailed()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var ftpConfig = new WamasFtpConfig();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeException = new Exception("This is a fake exception");
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PurchaseOrder>()).Throws(fakeException);
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var purchaseOrderExporter = new PurchaseOrderExporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        purchaseOrderExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = purchaseOrderExporter.ExportPurchaseOrders(DateTime.Now.AddDays(1));

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ExportedCount);
        Assert.Equal(1, jobResult.ErrorCount);
        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Wamas.Fehler beim Exportieren von Bestellungen zu WAMAS, {0}", fakeException.Message),
            Times.Once);
    }

    [Fact]
    public void ExportPurchaseOrders_WhenExceptionWritingLogisticsState_ShouldReturnJobResultPartialSuccess()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var ftpConfig = new WamasFtpConfig();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        var fakeException = new Exception("This is a fake exception");
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PurchaseOrder>())
            .Returns(PurchaseOrderExporterTestData.GetPurchaseOrder().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PurchaseOrderReference>())
            .Returns(PurchaseOrderExporterTestData.GetPurchaseOrderReferences().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PurchaseOrderLine>())
            .Returns(PurchaseOrderExporterTestData.GetPurchaseOrderLines().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PurchaseOrderPartner>())
            .Returns(PurchaseOrderExporterTestData.GetPurchaseOrderPartners().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Document>()).Throws(fakeException);
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var purchaseOrderExporter = new PurchaseOrderExporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        purchaseOrderExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = purchaseOrderExporter.ExportPurchaseOrders(DateTime.Now.AddDays(1));

        // Assert
        Assert.Equal(JobResultEnum.PartialSuccess, jobResult.Result);
        Assert.Equal(5, jobResult.ExportedCount);
        Assert.Equal(1, jobResult.ErrorCount);
        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Wamas.Fehler beim Bestaetigen des Logistikstatus {0} nach Export von Bestellung (Beleg-ID {1}) zu WAMAS, {2}",
                LogisticState.TransmittedToLogistics, "1", fakeException.Message), Times.AtLeastOnce);
    }
}

public static class PurchaseOrderExporterTestData
{
    public static List<PurchaseOrder> GetPurchaseOrder()
    {
        return new List<PurchaseOrder>
        {
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 1,
                RecordDate = new DateTime(2022, 07, 27, 08, 00, 00),
                DatasetType = "IBD00008",
                ClientId = "CDV",
                Id = "1",
                WarehouseLocation = "1",
                Type = "CDV",
                DeliveryTimeSlotFrom = DateTime.Now
            },
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 2,
                RecordDate = new DateTime(2022, 07, 27, 08, 00, 00),
                DatasetType = "IBD00008",
                ClientId = "CDV",
                Id = "2",
                WarehouseLocation = "2",
                Type = "CDV",
                DeliveryTimeSlotFrom = DateTime.Now.AddDays(2)
            }
        };
    }
    
    public static List<PurchaseOrderReference> GetPurchaseOrderReferences()
    {
        return new List<PurchaseOrderReference>
        {
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 1,
                RecordDate = new DateTime(2022, 07, 27, 08, 00, 00),
                DatasetType = "IBDEXTREF00006",
                ClientId = "CDV",
                Id = "1",
                KeyName = "VinX-Belegnummer",
                Number = "120",
                RecordState = "Normal",
                DeliveryTimeSlotFrom = DateTime.Now
            },
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 1,
                RecordDate = new DateTime(2022, 07, 27, 08, 00, 00),
                DatasetType = "IBDEXTREF00006",
                ClientId = "CDV",
                Id = "2",
                KeyName = "VinX-Belegnummer",
                Number = "121",
                RecordState = "Normal",
                DeliveryTimeSlotFrom = DateTime.Now.AddDays(2)
            }
        };
    }

    public static List<PurchaseOrderLine> GetPurchaseOrderLines()
    {
        return new List<PurchaseOrderLine>
        {
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 2,
                RecordDate = new DateTime(2022, 07, 27, 08, 00, 00),
                DatasetType = "IBDL00006",
                ClientId = "CDV",
                Id = "1",
                LineId = "1",
                ArticleNumber = "1",
                Variant = "2000",
                OrderedQuantity = 10,
                DeliveryTimeSlotFrom = DateTime.Now
            },
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 3,
                RecordDate = new DateTime(2022, 07, 27, 08, 00, 00),
                DatasetType = "IBDL00006",
                ClientId = "CDV",
                Id = "1",
                LineId = "2",
                ArticleNumber = "2",
                Variant = "2000",
                OrderedQuantity = 5,
                DeliveryTimeSlotFrom = DateTime.Now
            },
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 2,
                RecordDate = new DateTime(2022, 07, 27, 08, 00, 00),
                DatasetType = "IBDL00006",
                ClientId = "CDV",
                Id = "2",
                LineId = "100",
                ArticleNumber = "1",
                Variant = "2000",
                OrderedQuantity = 10,
                DeliveryTimeSlotFrom = DateTime.Now.AddDays(2)
            }
        };
    }

    public static List<PurchaseOrderPartner> GetPurchaseOrderPartners()
    {
        return new List<PurchaseOrderPartner>
        {
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 4,
                RecordDate = new DateTime(2022, 07, 27, 08, 00, 00),
                DatasetType = "IBDPARTNER00008",
                ClientId = "CDV",
                Id = "1",
                RoleKind = "SUPPLIER",
                PartnerId = "1",
                Language = "ger",
                DeliveryTimeSlotFrom = DateTime.Now
            },
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 3,
                RecordDate = new DateTime(2022, 07, 27, 08, 00, 00),
                DatasetType = "IBDPARTNER00008",
                ClientId = "CDV",
                Id = "2",
                RoleKind = "SUPPLIER",
                PartnerId = "1",
                Language = "ger",
                DeliveryTimeSlotFrom = DateTime.Now.AddDays(2)
            }
        };
    }

    public static List<Document> GetDocuments()
    {
        return new List<Document>
        {
            new()
            {
                Id = 1,
                Date = new DateTime(2022, 07, 27, 08, 00, 00),
                LogisticState = LogisticState.LogisticsCompleted
            },
            new()
            {
                Id = 2,
                Date = new DateTime(2022, 07, 27, 08, 00, 00),
                LogisticState = LogisticState.LogisticsCompleted
            }
        };
    }
}