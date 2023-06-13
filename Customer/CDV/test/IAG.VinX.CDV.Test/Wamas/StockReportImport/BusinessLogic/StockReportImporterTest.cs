using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.DataAccess;
using IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;
using IAG.VinX.CDV.Wamas.StockReportImport.BusinessLogic;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Wamas.StockReportImport.BusinessLogic;

public class StockReportImporterTest
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
        var stockReportImporter = new StockReportImporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);

        // Act
        stockReportImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Assert
        fakeConnectionFactory.Verify(
            con => con.CreateConnection(connectionString, It.IsAny<IUserContext>(), It.IsAny<ILogger>()), Times.Once);
    }

    [Theory]
    [InlineData(@"Wamas\StockReportImport\TestData\STOCKBEGIN00008_1.csv", 1)]
    [InlineData(@"Wamas\StockReportImport\TestData\STOCKBEGIN00008_2.csv", 1)]
    [InlineData(@"Wamas\StockReportImport\TestData\STOCKBEGIN00008_3.csv", 0)]
    public void ImportStockReports_WhenSuccess_ShouldReturnJobResult_ShouldDoAction(string fileName,
        int expectedImportedCount)
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fileList = new List<string> { fileName };

        var ftpConfig = new WamasFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        fakeFtpConnector.Setup(f => f.GetFiles("STOCKBEGIN00008", "csv")).Returns(fileList);

        using var stream = new MemoryStream();
        using var fileStream = File.OpenRead(fileName);
        fileStream.CopyTo(stream);
        fakeFtpConnector.Setup(f => f.DownloadFile(fileName)).Returns(stream.ToArray());

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        fakeVinXConnector.Setup(vc => vc.GetQueryable<InventoryLog>())
            .Returns(StockReportImporterTestData.GetInventoryLogs().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Article>())
            .Returns(StockReportImporterTestData.GetArticles().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Employee>())
            .Returns(StockReportImporterTestData.GetUsers().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<StorageLocation>())
            .Returns(StockReportImporterTestData.GetStorageLocations().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Warehouse>())
            .Returns(StockReportImporterTestData.GetWarehouses().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.Insert(It.IsAny<InventoryLog>()));
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var stockReportImporter = new StockReportImporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        stockReportImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = stockReportImporter.ImportStockReports();

        // Assert
        Assert.Equal(JobResultEnum.Success, jobResult.Result);
        Assert.Equal(expectedImportedCount, jobResult.ImportedCount);
        Assert.Equal(0, jobResult.ErrorCount);
    }
    
    [Fact]
    public void ImportStockReports_WhenUserNotFound_ShouldThrowException_ShouldImportNothing()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fileList = new List<string> { @"Wamas\StockReportImport\TestData\STOCKBEGIN00008_1.csv" };

        var ftpConfig = new WamasFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        fakeFtpConnector.Setup(f => f.GetFiles("STOCKBEGIN00008", "csv")).Returns(fileList);

        using var stream = new MemoryStream();
        using var fileStream = File.OpenRead(@"Wamas\StockReportImport\TestData\STOCKBEGIN00008_1.csv");
        fileStream.CopyTo(stream);
        fakeFtpConnector.Setup(f => f.DownloadFile(@"Wamas\StockReportImport\TestData\STOCKBEGIN00008_1.csv")).Returns(stream.ToArray());

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        fakeVinXConnector.Setup(vc => vc.GetQueryable<InventoryLog>())
            .Returns(StockReportImporterTestData.GetInventoryLogs().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Article>())
            .Returns(StockReportImporterTestData.GetArticles().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<StorageLocation>())
            .Returns(StockReportImporterTestData.GetStorageLocations().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Warehouse>())
            .Returns(StockReportImporterTestData.GetWarehouses().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.Insert(It.IsAny<InventoryLog>()));
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var stockReportImporter = new StockReportImporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        stockReportImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = stockReportImporter.ImportStockReports();

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ImportedCount);
        Assert.Equal(1, jobResult.ErrorCount);
        
        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Wamas.Fehler beim Importieren des Bestandsberichts aus WAMAS-Request ({0}) vom {1} aus der Bestandsbericht-Datei {2} aus WAMAS, {3}",
                "5", "24.08.2022 08:57:23", @"Wamas\StockReportImport\TestData\STOCKBEGIN00008_1.csv",
                "Employee: Support not found"),
            Times.Once);
    }
    
    [Fact]
    public void ImportStockReports_WhenStoragePlaceInsertFailed_ShouldThrowException_ShouldImportNothing()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fileList = new List<string> { @"Wamas\StockReportImport\TestData\STOCKBEGIN00008_1.csv" };

        var ftpConfig = new WamasFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        fakeFtpConnector.Setup(f => f.GetFiles("STOCKBEGIN00008", "csv")).Returns(fileList);

        using var stream = new MemoryStream();
        using var fileStream = File.OpenRead(@"Wamas\StockReportImport\TestData\STOCKBEGIN00008_1.csv");
        fileStream.CopyTo(stream);
        fakeFtpConnector.Setup(f => f.DownloadFile(@"Wamas\StockReportImport\TestData\STOCKBEGIN00008_1.csv")).Returns(stream.ToArray());

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        fakeVinXConnector.Setup(vc => vc.GetQueryable<InventoryLog>())
            .Returns(StockReportImporterTestData.GetInventoryLogs().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Article>())
            .Returns(StockReportImporterTestData.GetArticles().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Employee>())
            .Returns(StockReportImporterTestData.GetUsers().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<StorageLocation>())
            .Returns(StockReportImporterTestData.GetStorageLocations().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.Insert(It.IsAny<StorageLocation>()))
            .Throws(new Exception());
        fakeVinXConnector.Setup(vc => vc.Insert(It.IsAny<InventoryLog>()));
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var stockReportImporter = new StockReportImporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        stockReportImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = stockReportImporter.ImportStockReports();

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ImportedCount);
        Assert.Equal(1, jobResult.ErrorCount);

        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Wamas.Fehler beim Importieren des Bestandsberichts aus WAMAS-Request ({0}) vom {1} aus der Bestandsbericht-Datei {2} aus WAMAS, {3}",
                "5", "24.08.2022 08:57:23", @"Wamas\StockReportImport\TestData\STOCKBEGIN00008_1.csv",
                "StorageLocation: 99-99-99-99 not found and failed to create"),
            Times.Once);
    }

    [Theory]
    [InlineData(@"Wamas\StockReportImport\TestData\STOCKBEGIN00008_Fehler-Datentyp.csv",
        "CDV.Job.Wamas.Fehler beim Importieren der Bestandsbericht-Datei {0} aus WAMAS, {1}")]
    [InlineData(@"Wamas\StockReportImport\TestData\STOCKBEGIN00008_Fehler-NotFound.csv",
        "CDV.Job.Wamas.Fehler beim Importieren des Bestandsberichts aus WAMAS-Request ({0}) vom {1} aus der Bestandsbericht-Datei {2} aus WAMAS, {3}")]
    public void ImportStockReports_WhenNoSuccess_ShouldReturnJobResult_ShouldLogIfErrors(string fileName,
        string expectedErrorMessage)
    {
        // Arrange
        const string connectionString = "fakeConnectionString";

        var fileList = new List<string> { fileName };

        var ftpConfig = new WamasFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        fakeFtpConnector.Setup(f => f.GetFiles("STOCKBEGIN00008", "csv")).Returns(fileList);

        using var stream = new MemoryStream();
        using var fileStream = File.OpenRead(fileName);
        fileStream.CopyTo(stream);
        fakeFtpConnector.Setup(f => f.DownloadFile(fileName)).Returns(stream.ToArray());

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        fakeVinXConnector.Setup(vc => vc.GetQueryable<InventoryLog>())
            .Returns(StockReportImporterTestData.GetInventoryLogs().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Article>())
            .Returns(StockReportImporterTestData.GetArticles().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Employee>())
            .Returns(StockReportImporterTestData.GetUsers().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<StorageLocation>())
            .Returns(StockReportImporterTestData.GetStorageLocations().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Warehouse>())
            .Returns(StockReportImporterTestData.GetWarehouses().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.Insert(It.IsAny<InventoryLog>()));
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var stockReportImporter = new StockReportImporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        stockReportImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = stockReportImporter.ImportStockReports();

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ImportedCount);
        Assert.Equal(1, jobResult.ErrorCount);

        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error, expectedErrorMessage, It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public void ImportStockReports_WhenExceptionReadingData_ShouldReturnJobResultFailed()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fakeException = new Exception("This is a fake exception");
        var fakeFtpConnector = new Mock<IFtpConnector>();
        fakeFtpConnector.Setup(f => f.GetFiles("STOCKBEGIN00008", "csv")).Throws(fakeException);
        var ftpConfig = new WamasFtpConfig();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var stockReportImporter = new StockReportImporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        stockReportImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = stockReportImporter.ImportStockReports();

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ImportedCount);
        Assert.Equal(1, jobResult.ErrorCount);
        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Wamas.Fehler beim Importieren von Bestandsberichten aus WAMAS, {0}", fakeException.Message),
            Times.Once);
    }
}

public static class StockReportImporterTestData
{
    public static List<InventoryLog> GetInventoryLogs()
    {
        return new List<InventoryLog>
        {
            new()
            {
                Id = 341067,
                Batch = "WAMAS-7",
                UserId = 1,
                ArticleId = 20003814,
                PackageLevel = PackageLevel.BulkPackage,
                LevelFactor = 6,
                PackageCount = 6,
                BaseUnitCount = 36,
                Lot = "",
                ProcessingStatus = ProcessingStatus.Registered,
                AreaId = 1
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

    public static List<Employee> GetUsers()
    {
        return new List<Employee>
        {
            new()
            {
                Id = 1,
                Uref = "Support"
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
    
    public static List<Warehouse> GetWarehouses()
    {
        return new List<Warehouse>
        {
            new()
            {
                Id = 9,
                Name = "Rupperswil"
            }
        };
    }
}