using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.DataAccess;
using IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;
using IAG.VinX.CDV.Wamas.StockAdjustmentImport.BusinessLogic;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Wamas.StockAdjustmentImport.BusinessLogic;

public class StockAdjustmentImporterTest
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
        var stockAdjustmentImporter =
            new StockAdjustmentImporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);

        // Act
        stockAdjustmentImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Assert
        fakeConnectionFactory.Verify(
            con => con.CreateConnection(connectionString, It.IsAny<IUserContext>(), It.IsAny<ILogger>()), Times.Once);
    }

    [Theory]
    [InlineData(@"Wamas\StockAdjustmentImport\TestData\STOCKADJ00007_1.csv", 1)]
    [InlineData(@"Wamas\StockAdjustmentImport\TestData\STOCKADJ00007_2.csv", 0)]
    [InlineData(@"Wamas\StockAdjustmentImport\TestData\STOCKADJ00007_3.csv", 1)]
    [InlineData(@"Wamas\StockAdjustmentImport\TestData\STOCKADJ00007_4.csv", 0)]
    public void ImportStockAdjustments_WhenSuccess_ShouldReturnJobResult_ShouldDoAction(string fileName,
        int expectedImportedCount)
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fileList = new List<string> { fileName };

        var ftpConfig = new WamasFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        fakeFtpConnector.Setup(f => f.GetFiles("STOCKADJ00007", "csv")).Returns(fileList);

        using var stream = new MemoryStream();
        using var fileStream = File.OpenRead(fileName);
        fileStream.CopyTo(stream);
        fakeFtpConnector.Setup(f => f.DownloadFile(fileName)).Returns(stream.ToArray());

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        fakeVinXConnector.Setup(vc => vc.GetQueryable<StockMovement>())
            .Returns(StockAdjustmentImporterTestData.GetStockMovements().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Article>())
            .Returns(StockAdjustmentImporterTestData.GetArticles().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Warehouse>())
            .Returns(StockAdjustmentImporterTestData.GetWarehouses().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<StorageLocation>())
            .Returns(StockAdjustmentImporterTestData.GetStorageLocations().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.Insert(It.IsAny<StockMovement>()));
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var stockAdjustmentImporter =
            new StockAdjustmentImporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        stockAdjustmentImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = stockAdjustmentImporter.ImportStockAdjustments();

        // Assert
        Assert.Equal(JobResultEnum.Success, jobResult.Result);
        Assert.Equal(expectedImportedCount, jobResult.ImportedCount);
        Assert.Equal(0, jobResult.ErrorCount);
    }

    [Theory]
    [InlineData(@"Wamas\StockAdjustmentImport\TestData\STOCKADJ00007_Fehler-Datentyp.csv",
        "CDV.Job.Wamas.Fehler beim Importieren der Bestandskorrektur-Datei {0} aus WAMAS, {1}")]
    [InlineData(@"Wamas\StockAdjustmentImport\TestData\STOCKADJ00007_Fehler-Mapping.csv",
        "CDV.Job.Wamas.Fehler beim Importieren der Bestandskorrektur zu Artikel ({0}) vom {1} aus der Bestandskorrektur-Datei {2} aus WAMAS, {3}")]
    [InlineData(@"Wamas\StockAdjustmentImport\TestData\STOCKADJ00007_Fehler-NotFound.csv",
        "CDV.Job.Wamas.Fehler beim Importieren der Bestandskorrektur zu Artikel ({0}) vom {1} aus der Bestandskorrektur-Datei {2} aus WAMAS, {3}")]
    public void ImportStockAdjustments_WhenNoSuccess_ShouldReturnJobResult_ShouldLogIfErrors(string fileName,
        string expectedErrorMessage)
    {
        // Arrange
        const string connectionString = "fakeConnectionString";

        var fileList = new List<string> { fileName };

        var ftpConfig = new WamasFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        fakeFtpConnector.Setup(f => f.GetFiles("STOCKADJ00007", "csv")).Returns(fileList);

        using var stream = new MemoryStream();
        using var fileStream = File.OpenRead(fileName);
        fileStream.CopyTo(stream);
        fakeFtpConnector.Setup(f => f.DownloadFile(fileName)).Returns(stream.ToArray());

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        fakeVinXConnector.Setup(vc => vc.GetQueryable<StockMovement>())
            .Returns(StockAdjustmentImporterTestData.GetStockMovements().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Article>())
            .Returns(StockAdjustmentImporterTestData.GetArticles().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Warehouse>())
            .Returns(StockAdjustmentImporterTestData.GetWarehouses().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<StorageLocation>())
            .Returns(StockAdjustmentImporterTestData.GetStorageLocations().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.Insert(It.IsAny<StockMovement>()));
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var stockAdjustmentImporter =
            new StockAdjustmentImporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        stockAdjustmentImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = stockAdjustmentImporter.ImportStockAdjustments();

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ImportedCount);
        Assert.Equal(1, jobResult.ErrorCount);

        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error, expectedErrorMessage, It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public void ImportStockAdjustments_WhenExceptionReadingData_ShouldReturnJobResultFailed()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fakeException = new Exception("This is a fake exception");
        var fakeFtpConnector = new Mock<IFtpConnector>();
        fakeFtpConnector.Setup(f => f.GetFiles("STOCKADJ00007", "csv")).Throws(fakeException);
        var ftpConfig = new WamasFtpConfig();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var stockAdjustmentImporter =
            new StockAdjustmentImporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        stockAdjustmentImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = stockAdjustmentImporter.ImportStockAdjustments();

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ImportedCount);
        Assert.Equal(1, jobResult.ErrorCount);
        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Wamas.Fehler beim Importieren von Bestandskorrekturen aus WAMAS, {0}", fakeException.Message),
            Times.Once);
    }
}

public static class StockAdjustmentImporterTestData
{
    public static List<StockMovement> GetStockMovements()
    {
        return new List<StockMovement>
        {
            new()
            {
                Id = 341067,
                ArticleId = 20003814,
                WarehouseId = 9,
                Date = default,
                MovementType = StockMovementType.InventoryIncoming,
                Quantity = 36,
                PackagingQuantity = 6,
                Description = "WAMAS-2022082302083511000267",
                Chargeable = 0,
                User = "Support",
                ForOrderProposal = -1,
                SectionId = 1,
                ClientId = 1
            }
        };
    }

    public static List<Article> GetArticles()
    {
        return new List<Article>
        {
            new()
            {
                Id = 20003814,
                ArticleNumber = 1000267
            },
            new()
            {
                Id = 305461,
                ArticleNumber = 1000865
            }
        };
    }

    public static List<Warehouse> GetWarehouses()
    {
        return new List<Warehouse>
        {
            new()
            {
                Id = 9,
                Name = "Rupperswil"
            },
            new()
            {
                Id = 5,
                Name = "Ligornetto"
            }
        };
    }

    public static List<StorageLocation> GetStorageLocations()
    {
        return new List<StorageLocation>
        {
            new()
            {
                Id = 1,
                WarehouseId = 9,
                ArticleId = 305461,
                Description = "11-01-02-01"
            },
            new()
            {
                Id = 2,
                WarehouseId = 9,
                ArticleId = 20003814,
                Description = "11-01-01-01"
            }
        };
    }
}