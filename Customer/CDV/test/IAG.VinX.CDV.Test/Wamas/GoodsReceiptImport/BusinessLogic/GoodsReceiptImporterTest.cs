using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.BaseData.Dto.Enums;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.DataAccess;
using IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;
using IAG.VinX.CDV.Wamas.GoodsReceiptImport.BusinessLogic;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Wamas.GoodsReceiptImport.BusinessLogic;

public class GoodsReceiptImporterTest
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
        var goodsReceiptImporter = new GoodsReceiptImporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);

        // Act
        goodsReceiptImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Assert
        fakeConnectionFactory.Verify(
            con => con.CreateConnection(connectionString, It.IsAny<IUserContext>(), It.IsAny<ILogger>()), Times.Once);
    }

    [Theory]
    [InlineData(@"Wamas\GoodsReceiptImport\TestData\IBDACK00010_1_Zeile_geliefert=bestellt.csv", 1)]
    [InlineData(@"Wamas\GoodsReceiptImport\TestData\IBDACK00010_1_Zeile_Mindermenge_geliefert.csv", 1)]
    [InlineData(@"Wamas\GoodsReceiptImport\TestData\IBDACK00010_2_Zeilen_geliefert=bestellt.csv", 1)]
    [InlineData(@"Wamas\GoodsReceiptImport\TestData\IBDACK00010_3_Zeilen_keine_Lieferung.csv", 1)]
    [InlineData(@"Wamas\GoodsReceiptImport\TestData\IBDACK00010_1_Zeile_bereits_importiert.csv", 0)]
    public void ImportGoodsReceipts_WhenSuccess_ShouldReturnJobResult_ShouldDoAction(string fileName,
        int expectedImportedCount)
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fileList = new List<string> { fileName };

        var ftpConfig = new WamasFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        fakeFtpConnector.Setup(f => f.GetFiles("IBDACK00010", "csv")).Returns(fileList);

        using var stream = new MemoryStream();
        using var fileStream = File.OpenRead(fileName);
        fileStream.CopyTo(stream);
        fakeFtpConnector.Setup(f => f.DownloadFile(fileName)).Returns(stream.ToArray);

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Document>())
            .Returns(GoodsReceiptImporterTestData.GetDocuments().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<ArticlePosition>())
            .Returns(GoodsReceiptImporterTestData.GetArticlePositions().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.Update(It.IsAny<Document>()));
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var goodsReceiptImporter = new GoodsReceiptImporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        goodsReceiptImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = goodsReceiptImporter.ImportGoodsReceipts();

        // Assert
        Assert.Equal(JobResultEnum.Success, jobResult.Result);
        Assert.Equal(expectedImportedCount, jobResult.ImportedCount);
        Assert.Equal(0, jobResult.ErrorCount);
    }

    [Theory]
    [InlineData(@"Wamas\GoodsReceiptImport\TestData\IBDACK00010_Fehler-Datentyp.csv",
        "CDV.Job.Wamas.Fehler beim Importieren der Wareneingangs-Datei {0} aus WAMAS, {1}")]
    [InlineData(@"Wamas\GoodsReceiptImport\TestData\IBDACK00010_Fehler-Mapping.csv",
        "CDV.Job.Wamas.Fehler beim Importieren der Wareneingangs-Datei {0} aus WAMAS, {1}")]
    public void ImportGoodsReceipts_WhenNoSuccess_ShouldReturnJobResult_ShouldLogIfErrors(string fileName,
        string expectedErrorMessage)
    {
        // Arrange
        const string connectionString = "fakeConnectionString";

        var fileList = new List<string> { fileName };

        var ftpConfig = new WamasFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        fakeFtpConnector.Setup(f => f.GetFiles("IBDACK00010", "csv")).Returns(fileList);

        using var stream = new MemoryStream();
        using var fileStream = File.OpenRead(fileName);
        fileStream.CopyTo(stream);
        fakeFtpConnector.Setup(f => f.DownloadFile(fileName)).Returns(stream.ToArray);

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Document>())
            .Returns(GoodsReceiptImporterTestData.GetDocuments().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<ArticlePosition>())
            .Returns(GoodsReceiptImporterTestData.GetArticlePositions().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.Update(It.IsAny<Document>()));
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var goodsReceiptImporter = new GoodsReceiptImporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        goodsReceiptImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = goodsReceiptImporter.ImportGoodsReceipts();

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ImportedCount);
        Assert.Equal(1, jobResult.ErrorCount);

        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error, expectedErrorMessage, It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public void ImportGoodsReceipts_WhenExceptionReadingData_ShouldReturnJobResultFailed()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fakeException = new Exception("This is a fake exception");
        var fakeFtpConnector = new Mock<IFtpConnector>();
        fakeFtpConnector.Setup(f => f.GetFiles("IBDACK00010", "csv")).Throws(fakeException);
        var ftpConfig = new WamasFtpConfig();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var goodsReceiptImporter = new GoodsReceiptImporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        goodsReceiptImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = goodsReceiptImporter.ImportGoodsReceipts();

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ImportedCount);
        Assert.Equal(1, jobResult.ErrorCount);
        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Wamas.Fehler beim Importieren von Wareneingaengen aus WAMAS, {0}", fakeException.Message),
            Times.Once);
    }
}

public static class GoodsReceiptImporterTestData
{
    public static List<Document> GetDocuments()
    {
        return new List<Document>
        {
            new()
            {
                Id = 341067,
                Date = new DateTime(2022, 07, 27, 08, 00, 00),
                LogisticState = LogisticState.TransmittedToLogistics,
                DocumentType = ReceiptType.Order
            },
            new()
            {
                Id = 326767,
                Date = new DateTime(2022, 07, 27, 08, 00, 00),
                LogisticState = LogisticState.TransmittedToLogistics,
                DocumentType = ReceiptType.Order
            },
            new()
            {
                Id = 262424,
                Date = new DateTime(2022, 07, 27, 08, 00, 00),
                LogisticState = LogisticState.TransmittedToLogistics,
                DocumentType = ReceiptType.Order
            },
            new()
            {
                Id = 342060,
                Date = new DateTime(2022, 07, 27, 08, 00, 00),
                LogisticState = LogisticState.TransmittedToLogistics,
                DocumentType = ReceiptType.Order
            },
            new()
            {
                Id = 342061,
                Date = new DateTime(2022, 07, 27, 08, 00, 00),
                LogisticState = LogisticState.TransmittedToLogistics,
                DocumentType = ReceiptType.WarehouseEntry
            },
            new()
            {
                Id = 420000,
                Date = new DateTime(2022, 07, 27, 08, 00, 00),
                LogisticState = LogisticState.LogisticsCompleted,
                DocumentType = ReceiptType.Order
            },
            new()
            {
                Id = 420001,
                Date = new DateTime(2022, 07, 27, 08, 00, 00),
                LogisticState = LogisticState.TransmittedToLogistics,
                DocumentType = ReceiptType.WarehouseEntry
            },
        };
    }

    public static List<ArticlePosition> GetArticlePositions()
    {
        return new List<ArticlePosition>
        {
            new()
            {
                Id = 1,
                DocumentId = 341067,
                Quantity = 150
            },
            new()
            {
                Id = 1,
                DocumentId = 326767,
                Quantity = 240
            },
            new()
            {
                Id = 1,
                DocumentId = 262424,
                Quantity = 300
            },
            new()
            {
                Id = 2,
                DocumentId = 262424,
                Quantity = 1200
            },
            new()
            {
                Id = 1,
                DocumentId = 342060,
                Quantity = 1548
            },
            new()
            {
                Id = 2,
                DocumentId = 342060,
                Quantity = 672
            },
            new()
            {
                Id = 1,
                DocumentId = 420000,
                Quantity = 20
            },
            new()
            {
                Id = 1,
                DocumentId = 420001,
                Quantity = 20
            }
        };
    }
}