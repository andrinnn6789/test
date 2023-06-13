using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.CDV.Wamas.ArticleExport.BusinessLogic;
using IAG.VinX.CDV.Wamas.ArticleExport.Dto;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.DataAccess;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

using Article = IAG.VinX.CDV.Wamas.ArticleExport.Dto.Article;

namespace IAG.VinX.CDV.Test.Wamas.ArticleExport.BusinessLogic;

public class ArticleExporterTest
{
    [Fact]
    public void SetConfig_ShouldCreateConnection()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var ftpConfig = new WamasFtpConfig();
        fakeFtpConnector.Setup(f => f.SetConfig(ftpConfig));
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory => factory.CreateConnection(connectionString, It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var addressExporter = new ArticleExporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);

        // Act
        addressExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Assert
        fakeConnectionFactory.Verify(
            con => con.CreateConnection(connectionString, It.IsAny<IUserContext>(), It.IsAny<ILogger>()), Times.Once);
    }

    [Fact]
    public void ExportArticles_WhenNoException_ShouldReturnJobResultSuccess_ShouldExportFile()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var ftpConfig = new WamasFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        fakeFtpConnector.Setup(f => f.SetConfig(ftpConfig));
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Article>())
            .Returns(ArticleExporterTestData.GetArticles().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<ArticleQuantityUnit>())
            .Returns(ArticleExporterTestData.GetArticleQuantityUnits().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<ArticleDescription>())
            .Returns(ArticleExporterTestData.GetArticleDescriptions().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<ArticleAlias>())
            .Returns(ArticleExporterTestData.GetArticleAliases().AsQueryable);
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var addressExporter = new ArticleExporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        addressExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = addressExporter.ExportArticles(DateTime.MinValue);

        // Assert
        Assert.Equal(JobResultEnum.Success, jobResult.Result);
        Assert.Equal(8, jobResult.ExportedCount);
        Assert.Equal(0, jobResult.ErrorCount);
        fakeFtpConnector.Verify(ftp => ftp.SetConfig(ftpConfig), Times.Once);
        fakeFtpConnector.Verify(ftp => ftp.UploadFile(It.IsAny<byte[]>(), It.Is<string>(s => s.Contains("ITEM00011"))),
            Times.Once);
    }

    [Fact]
    public void ExportArticles_WhenException_ShouldReturnJobResultFailed_ShouldLogErrorMessage()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var ftpConfig = new WamasFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeException = new Exception("This is a fake exception");
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Article>()).Throws(fakeException);
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var addressExporter = new ArticleExporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        addressExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = addressExporter.ExportArticles(DateTime.MinValue);

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ExportedCount);
        Assert.Equal(1, jobResult.ErrorCount);
        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Wamas.Fehler beim Exportieren von Artikeln zu WAMAS, {0}", fakeException.Message), Times.Once);
    }
}

public static class ArticleExporterTestData
{
    public static List<Article> GetArticles()
    {
        return new List<Article>
        {
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 1,
                RecordDate = new DateTime(2022, 07, 22, 08, 00, 00),
                DatasetType = "ITEM00011",
                ClientId = "CDV",
                ItemNumber = "1",
                Variant = "2000",
                BaseQuantityUnit = "A1",
                WarehouseQuantityUnit = "G1",
                ItemType = "Wein",
                GoodsValue = 12.50M,
                GoodsValueUnit = "CHF",
                OriginCountry = "IT"
            }
        };
    }

    public static List<ArticleQuantityUnit> GetArticleQuantityUnits()
    {
        return new List<ArticleQuantityUnit>
        {
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 2,
                RecordDate = new DateTime(2022, 07, 22, 08, 00, 00),
                DatasetType = "ITEMDESC00005",
                ClientId = "CDV",
                ItemNumber = "1",
                Variant = "2000",
                Denominator = 1,
                QuantityUnit = "G1",
                Numerator = "6",
                ReferenceQuantityUnit = "A1"
            },
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 3,
                RecordDate = new DateTime(2022, 07, 22, 08, 00, 00),
                DatasetType = "ITEMDESC00005",
                ClientId = "CDV",
                ItemNumber = "1",
                Variant = "2000",
                Denominator = 1,
                QuantityUnit = "G2",
                Numerator = "12",
                ReferenceQuantityUnit = "A1"
            }
        };
    }

    public static List<ArticleDescription> GetArticleDescriptions()
    {
        return new List<ArticleDescription>
        {
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 4,
                RecordDate = new DateTime(2022, 07, 22, 08, 00, 00),
                DatasetType = "ITEMDESC00005",
                ClientId = "CDV",
                ItemNumber = "1",
                Variant = "2000",
                Description = "Rotwein",
                Language = "ger"
            },
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 5,
                RecordDate = new DateTime(2022, 07, 22, 08, 00, 00),
                DatasetType = "ITEMDESC00005",
                ClientId = "CDV",
                ItemNumber = "1",
                Variant = "2000",
                Description = "Vino rosso",
                Language = "it"
            },
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 6,
                RecordDate = new DateTime(2022, 07, 22, 08, 00, 00),
                DatasetType = "ITEMDESC00005",
                ClientId = "CDV",
                ItemNumber = "1",
                Variant = "2000",
                Description = "Vin rouge",
                Language = "fra"
            }
        };
    }

    public static List<ArticleAlias> GetArticleAliases()
    {
        return new List<ArticleAlias>
        {
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 7,
                RecordDate = new DateTime(2022, 07, 22, 08, 00, 00),
                DatasetType = "ITEMALIAS00006",
                ClientId = "CDV",
                ItemNumber = "1",
                Variant = "2000",
                ItemAliasNumber = "1234567891234",
                Kind = "GTIN",
                QuantityUnit = "A1"
            },
            new()
            {
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 8,
                RecordDate = new DateTime(2022, 07, 22, 08, 00, 00),
                DatasetType = "ITEMALIAS00006",
                ClientId = "CDV",
                ItemNumber = "1",
                Variant = "2000",
                ItemAliasNumber = "9876543219873",
                Kind = "GTIN",
                QuantityUnit = "G1"
            }
        };
    }
}