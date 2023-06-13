using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.DataAccess;
using IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;
using IAG.VinX.CDV.Wamas.PickListExport.BusinessLogic;
using IAG.VinX.CDV.Wamas.PickListExport.Dto;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Wamas.PickListExport.BusinessLogic;

public class PickListExporterTest
{
    [Fact]
    public void SetConfigTest()
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
        var pickListExporter = new PickListExporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);

        // Act
        pickListExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Assert
        fakeConnectionFactory.Verify(
            con => con.CreateConnection(connectionString, It.IsAny<IUserContext>(), It.IsAny<ILogger>()), Times.Once);
    }

    [Fact]
    public void ExportPickLists_WhenSuccess_ShouldReturnJobResultSuccess_ShouldExportFile()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var ftpConfig = new WamasFtpConfig();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PickList>())
            .Returns(PickListExporterTestData.GetPickList().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PickListReference>())
            .Returns(PickListExporterTestData.GetPickListReferences().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PickListLine>())
            .Returns(PickListExporterTestData.GetPickListLines().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PickListText>())
            .Returns(PickListExporterTestData.GetPickListTexts().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PickListPartner>())
            .Returns(PickListExporterTestData.GetPickListPartners().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Document>())
            .Returns(PickListExporterTestData.GetDocuments().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.Update(It.IsAny<Document>()));
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var pickListExporter = new PickListExporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        pickListExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = pickListExporter.ExportPickLists(DateTime.Now.AddDays(10), DateTime.Now.AddMinutes(-30));

        // Assert
        Assert.Equal(JobResultEnum.Success, jobResult.Result);
        Assert.Equal(6, jobResult.ExportedCount);
        Assert.Equal(0, jobResult.ErrorCount);

        fakeFtpConnector.Verify(ftp => ftp.SetConfig(ftpConfig), Times.Once);
        fakeFtpConnector.Verify(ftp => ftp.UploadFile(It.IsAny<byte[]>(), It.Is<string>(s => s.Contains("OBO00008"))),
            Times.Once);
    }

    [Fact]
    public void ExportPickLists_WhenExceptionReadingData_ShouldReturnJobResultFailed()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var ftpConfig = new WamasFtpConfig();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeException = new Exception("This is a fake exception");
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PickList>()).Throws(fakeException);
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var pickListExporter = new PickListExporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        pickListExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = pickListExporter.ExportPickLists(DateTime.Now.AddDays(10), DateTime.Now.AddMinutes(-30));

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ExportedCount);
        Assert.Equal(1, jobResult.ErrorCount);
        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Wamas.Fehler beim Exportieren von Ruestscheinen zu WAMAS, {0}", fakeException.Message),
            Times.Once);
    }

    [Fact]
    public void ExportPickLists_WhenExceptionWritingLogisticsState_ShouldReturnJobResultPartialSuccess()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var ftpConfig = new WamasFtpConfig();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        var fakeException = new Exception("This is a fake exception");
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PickList>())
            .Returns(PickListExporterTestData.GetPickList().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PickListReference>())
            .Returns(PickListExporterTestData.GetPickListReferences().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PickListLine>())
            .Returns(PickListExporterTestData.GetPickListLines().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PickListText>())
            .Returns(PickListExporterTestData.GetPickListTexts().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PickListPartner>())
            .Returns(PickListExporterTestData.GetPickListPartners().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Document>()).Throws(fakeException);
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var pickListExporter = new PickListExporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        pickListExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = pickListExporter.ExportPickLists(DateTime.Now.AddDays(10), DateTime.Now.AddMinutes(-30));

        // Assert
        Assert.Equal(JobResultEnum.PartialSuccess, jobResult.Result);
        Assert.Equal(6, jobResult.ExportedCount);
        Assert.Equal(1, jobResult.ErrorCount);
        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Wamas.Fehler beim Bestaetigen des Logistikstatus {0} nach Export von Ruestschein (Beleg-ID {1}) zu WAMAS, {2}",
                LogisticState.TransmittedToLogistics, "1", fakeException.Message), Times.AtLeastOnce);
    }
}

public static class PickListExporterTestData
{
    public static List<PickList> GetPickList()
    {
        return new List<PickList>
        {
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 1,
                RecordDate = new DateTime(2022, 07, 27, 08, 00, 00),
                DatasetType = "OBO00008",
                ClientId = "CDV",
                Id = "1",
                WarehouseLocation = "1",
                Type = "CDV",
                RequestedDeliveryDateFrom = DateTime.Now
            },
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 2,
                RecordDate = new DateTime(2022, 12, 05, 08, 00, 00),
                DatasetType = "OBO00008",
                ClientId = "CDV",
                Id = "2",
                WarehouseLocation = "1",
                Type = "CDV",
                RequestedDeliveryDateFrom = DateTime.Now,
                CreateDate = DateTime.Now
            }
        };
    }
    
    public static List<PickListReference> GetPickListReferences()
    {
        return new List<PickListReference>
        {
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 1,
                RecordDate = new DateTime(2022, 07, 27, 08, 00, 00),
                DatasetType = "OBOEXTREF00006",
                ClientId = "CDV",
                Id = "1",
                KeyName = "VinX-Belegnummer",
                Number = "120",
                RecordState = "Normal"
            },
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 2,
                RecordDate = new DateTime(2022, 12, 05, 08, 00, 00),
                DatasetType = "OBOEXTREF00006",
                ClientId = "CDV",
                Id = "2",
                KeyName = "VinX-Belegnummer",
                Number = "121",
                RecordState = "Normal",
                CreateDate = DateTime.Now
            }
        };
    }

    public static List<PickListLine> GetPickListLines()
    {
        return new List<PickListLine>
        {
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 2,
                RecordDate = new DateTime(2022, 07, 27, 08, 00, 00),
                DatasetType = "OBOL00006",
                ClientId = "CDV",
                Id = "1",
                LineId = "1",
                ArticleNumber = "1",
                Variant = "2000",
                PickListQuantity = 10,
                RequestedDeliveryDateFrom = DateTime.Now
            },
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 3,
                RecordDate = new DateTime(2022, 07, 27, 08, 00, 00),
                DatasetType = "OBOL00006",
                ClientId = "CDV",
                Id = "1",
                LineId = "2",
                ArticleNumber = "1",
                Variant = "2000",
                PickListQuantity = 5,
                RequestedDeliveryDateFrom = DateTime.Now
            },
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 4,
                RecordDate = new DateTime(2022, 12, 05, 08, 00, 00),
                DatasetType = "OBOL00006",
                ClientId = "CDV",
                Id = "3",
                LineId = "1",
                ArticleNumber = "1",
                Variant = "2000",
                PickListQuantity = 10,
                RequestedDeliveryDateFrom = DateTime.Now,
                CreateDate = DateTime.Now
            }
        };
    }
    
    public static List<PickListText> GetPickListTexts()
    {
        return new List<PickListText>
        {
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 4,
                RecordDate = new DateTime(2022, 07, 27, 08, 00, 00),
                DatasetType = "OBOTEXT00008",
                ClientId = "CDV",
                Id = "1",
                TextTypeClientId = "CDV",
                TextType = "WA-Auftrag",
                Language = "ger",
                Sequence = "1",
                Text = @"{\rtf1\ansi\ansicpg1252\deff0{\fonttbl{\f0\fnil\fcharset0 Arial;}}
        {\colortbl ;\red0\green0\blue0;}
        \viewkind4\uc1\pard\cf1\lang2055\b\fs22 KORR RS- Bitte noch Pintia retour holen - Achtung, Jahrgang kontrollieren und genau rapportieren für Kontrolle durch Lagermanagement. Bitte auch die Etikette nochmals genau kontrollieren und an Leiter des Lagers melden, falls etwas nicht in Ordnung ist. Vielen Danke\b0\fs20 
            \p",
                RecordState = "Normal",
                RequestedDeliveryDateFrom = DateTime.Now
            },
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 4,
                RecordDate = new DateTime(2022, 12, 05, 08, 00, 00),
                DatasetType = "OBOTEXT00008",
                ClientId = "CDV",
                Id = "2",
                TextTypeClientId = "CDV",
                TextType = "WA-Auftrag",
                Language = "ger",
                Sequence = "1",
                Text = @"{\rtf1\ansi\ansicpg1252\deff0{\fonttbl{\f0\fnil\fcharset0 Arial;}}
        {\colortbl ;\red0\green0\blue0;}
        \viewkind4\uc1\pard\cf1\lang2055\b\fs22 KORR RS- Bitte noch Pintia retour holen - Achtung, Jahrgang kontrollieren und genau rapportieren für Kontrolle durch Lagermanagement. Bitte auch die Etikette nochmals genau kontrollieren und an Leiter des Lagers melden, falls etwas nicht in Ordnung ist. Vielen Danke\b0\fs20 
            \p",
                RecordState = "Normal",
                RequestedDeliveryDateFrom = DateTime.Now,
                CreateDate = DateTime.Now
            }
        };
    }

    public static List<PickListPartner> GetPickListPartners()
    {
        return new List<PickListPartner>
        {
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 4,
                RecordDate = new DateTime(2022, 07, 27, 08, 00, 00),
                DatasetType = "OBOPARTNER00008",
                ClientId = "CDV",
                Id = "1",
                RoleKind = "CONTRACTEE",
                PartnerId = "1",
                Language = "ger",
                RequestedDeliveryDateFrom = DateTime.Now
            },
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 4,
                RecordDate = new DateTime(2022, 12, 05, 08, 00, 00),
                DatasetType = "OBOPARTNER00008",
                ClientId = "CDV",
                Id = "2",
                RoleKind = "CONTRACTEE",
                PartnerId = "2",
                Language = "ger",
                RequestedDeliveryDateFrom = DateTime.Now,
                CreateDate = DateTime.Now
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
            }
        };
    }
}