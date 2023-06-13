using IAG.Common.DataLayerNHibernate;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.CDV.Gastivo.Common.Config;
using IAG.VinX.CDV.Gastivo.Common.Ftp;
using IAG.VinX.CDV.Gastivo.ArticleExport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using Moq;

using NHibernate;

using Xunit;

namespace IAG.VinX.CDV.Test.Gastivo.ArticleExport.BusinessLogic;

public class ArticleExporterTest
{
    [Fact]
    public void SetConfig_ShouldCreateSession()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        const string imageUrlTemplate = "https://www.casadelvino.ch/ShopImage/artikel/list/{0}/0/{0}.jpg?width=1140&height=1140";
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var ftpConfig = new GastivoFtpConfig();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXSession = new Mock<ISession>();
        var fakeVinXSessionContext = new Mock<ISessionContext>();
        fakeVinXSessionContext.Setup(sc => sc.Session).Returns(fakeVinXSession.Object);
        var fakeSessionContextFactory = new Mock<ISessionContextFactory>();
        fakeSessionContextFactory
            .Setup(factory => factory.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(fakeVinXSessionContext.Object);
        var articleExporter = new ArticleExporter(fakeSessionContextFactory.Object, fakeFtpConnector.Object);

        // Act
        articleExporter.SetConfig(ftpConfig, connectionString, imageUrlTemplate, fakeMessageLogger.Object);

        // Assert
        fakeSessionContextFactory.Verify(
            con => con.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(),It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public void ExportArticles_WhenNoException_ShouldReturnJobResultSuccess_ShouldExportFile()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        const string imageUrlTemplate = "https://www.casadelvino.ch/ShopImage/artikel/list/{0}/0/{0}.jpg?width=1140&height=1140";
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var ftpConfig = new GastivoFtpConfig();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXSession = new Mock<ISession>();
        fakeVinXSession.Setup(vc => vc.Query<Article>())
            .Returns(ArticleExporterTestData.GetArticles().AsQueryable());
        var fakeVinXSessionContext = new Mock<ISessionContext>();
        fakeVinXSessionContext.Setup(sc => sc.Session).Returns(fakeVinXSession.Object);
        var fakeSessionFactory = new Mock<ISessionContextFactory>();
        fakeSessionFactory
            .Setup(factory => factory.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(fakeVinXSessionContext.Object);
        var articleExporter = new ArticleExporter(fakeSessionFactory.Object, fakeFtpConnector.Object);
        articleExporter.SetConfig(ftpConfig, connectionString, imageUrlTemplate, fakeMessageLogger.Object);

        // Act
        var jobResult = articleExporter.ExportArticles();

        // Assert
        Assert.Equal(JobResultEnum.Success, jobResult.Result);
        Assert.Equal(4, jobResult.ExportedCount);
        Assert.Equal(0, jobResult.ErrorCount);
        fakeFtpConnector.Verify(ftp => ftp.SetConfig(ftpConfig), Times.Once);
        fakeFtpConnector.Verify(
            ftp => ftp.UploadFile(It.IsAny<byte[]>(), It.Is<string>(s => s.Contains("articles"))), Times.Once);
    }

    [Fact]
    public void ExportArticles_WhenException_ShouldReturnJobResultFailed_ShouldLogErrorMessage()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        const string imageUrlTemplate = "https://www.casadelvino.ch/ShopImage/artikel/list/{0}/0/{0}.jpg?width=1140&height=1140";
        var ftpConfig = new GastivoFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeException = new Exception("This is a fake exception");
        var fakeVinXSession = new Mock<ISession>();
        fakeVinXSession.Setup(vc => vc.Query<Article>()).Throws(fakeException);
        var fakeVinXSessionContext = new Mock<ISessionContext>();
        fakeVinXSessionContext.Setup(sc => sc.Session).Returns(fakeVinXSession.Object);
        var fakeSessionFactory = new Mock<ISessionContextFactory>();
        fakeSessionFactory
            .Setup(factory => factory.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(fakeVinXSessionContext.Object);
        var articleExporter = new ArticleExporter(fakeSessionFactory.Object, fakeFtpConnector.Object);
        articleExporter.SetConfig(ftpConfig, connectionString, imageUrlTemplate, fakeMessageLogger.Object);

        // Act
        var jobResult = articleExporter.ExportArticles();

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ExportedCount);
        Assert.Equal(1, jobResult.ErrorCount);
        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Gastivo.Fehler beim Exportieren von Artikeln zu Gastivo, {0}", fakeException),
            Times.Once);
    }
}

public static class ArticleExporterTestData
{
    public static IEnumerable<Article> GetArticles()
    {
        return new List<Article>
        {
            new()
            {
                Id = 1,
                ArticleNumber = 1,
                ProductTitle = "Cola",
                ProductTitleItalian = "",
                ProductTitleFrench = "",
                EanCode1 = 1234567899876,
                EanCode2 = 9876543211234,
                EanCode3 = 4567891234567,
                EanCode4 = 123789456123,
                Vat = new Vat(){ Id = 1, Percentage = (decimal)7.7 },
                Cycle = new Cycle(){ Id = 1 },
                ECommerceGroup = new ArticleECommerceGroup(){ Id = 1 },
                ArticleType = 5,
                Category = new ArticleCategory(){ Id = 10 },
                SalesPrices = new List<SalesPrice>()
                {
                    new (){ Id = 1, Price = (decimal)17.50, PriceGroup = new PriceGroup(){Id = 3, Description = "Gastronomie"}, IsActive = true, ValidFrom = DateTime.Today},
                    new (){ Id = 1, Price = 20, PriceGroup = new PriceGroup(){Id = 2, Description = "Divers"}, IsActive = true, ValidFrom = DateTime.Today}
                },
                BulkPackage = new BulkPackage(){Id = 1, AbbreviationForWeb = "Ca"},
                Filling = new Filling(){Id = 1, AbbreviationForWeb = "BO", Abbreviation = "33cl"},
                Stocks = new List<Stock>()
                {
                    new ()
                    {
                        Id = 1,
                        Article = null,
                        Warehouse = new Warehouse(){ Id = 1, Description = "Testlager"},
                        OnStock = 5,
                        Reserved = 3,
                        Provision = 1,
                        Ordered = 1,
                        MininumStockWebshop = 7
                    }
                },
                Country = new Country() { Id = 1, IsoNumber = 380}
            },
            new()
            {
                Id = 2,
                ArticleNumber = 2,
                ProductTitle = "Red Bull",
                ProductTitleItalian = "Red Bull",
                ProductTitleFrench = "Red Bull",
                EanCode1 = 1122334455667,
                Vat = new Vat(){ Id = 1, Percentage = (decimal)8.0 },
                Cycle = new Cycle(){ Id = 2 },
                ECommerceGroup = new ArticleECommerceGroup(){ Id = 2 },
                ArticleType = 5,
                Category = new ArticleCategory(){ Id = 10 },
                SalesPrices = new List<SalesPrice>()
                {
                    new (){ Id = 1, Price = (decimal)3.50, PriceGroup = new PriceGroup(){Id = 3, Description = "Gastronomie"}, IsActive = true, ValidFrom = DateTime.Today},
                    new (){ Id = 1, Price = 5, PriceGroup = new PriceGroup(){Id = 2, Description = "Divers"}, IsActive = true, ValidFrom = DateTime.Today}
                },
                BulkPackage = new BulkPackage(){Id = 1, AbbreviationForWeb = "Ca"},
                Filling = new Filling(){Id = 1, AbbreviationForWeb = "PK"},
                Stocks = new List<Stock>()
                {
                    new ()
                    {
                        Id = 1,
                        Article = null,
                        Warehouse = new Warehouse(){ Id = 1, Description = "Testlager"},
                        OnStock = 1,
                        Reserved = 3,
                        Provision = 1,
                        Ordered = 1,
                        MininumStockWebshop = 7
                    }
                },
                Country = new Country() { Id = 1, IsoNumber = 0}
            },
            new()
            {
                Id = 3,
                ArticleNumber = 3,
                ProductTitle = "",
                ProductTitleItalian = "Succo d'arancia",
                ProductTitleFrench = "Jus d'orange",
                EanCode1 = 1122334455667,
                Vat = new Vat(){ Id = 1, Percentage = (decimal)8.0 },
                Cycle = new Cycle(){ Id = 1 },
                ECommerceGroup = new ArticleECommerceGroup(){ Id = 2 },
                ArticleType = 5,
                Category = new ArticleCategory(){ Id = 10 },
                SalesPrices = new List<SalesPrice>()
                {
                    new (){ Id = 1, Price = (decimal)2.50, PriceGroup = new PriceGroup(){Id = 3, Description = "Gastronomie"}, IsActive = true, ValidFrom = DateTime.Today},
                    new (){ Id = 1, Price = 4, PriceGroup = new PriceGroup(){Id = 2, Description = "Divers"}, IsActive = true, ValidFrom = DateTime.Today}
                },
                BulkPackage = new BulkPackage(){Id = 1, AbbreviationForWeb = "Ca"},
                Filling = new Filling(){Id = 1, AbbreviationForWeb = "LTR"},
                Stocks = new List<Stock>(),
                Country = new Country() { Id = 1}
            },
            new()
            {
                Id = 4,
                ArticleNumber = 4,
                ProductTitle = "Geschenkset",
                ProductTitleItalian = "",
                ProductTitleFrench = "Ensemble-cadeau",
                Vat = new Vat(){ Id = 1, Percentage = (decimal)8.0 },
                Cycle = new Cycle(){ Id = 3 },
                ECommerceGroup = new ArticleECommerceGroup(){ Id = 2 },
                SalesPrices = new List<SalesPrice>(),
                ArticleType = 30,
                Category = new ArticleCategory(){ Id = 10 },
                Filling = new Filling(){Id = 1, AbbreviationForWeb = "PCE"},
                Stocks = new List<Stock>()
                {
                    new ()
                    {
                        Id = 1,
                        Article = null,
                        Warehouse = new Warehouse(){ Id = 1, Description = "Testlager"},
                        OnStock = 15,
                        Reserved = 3,
                        Provision = 1,
                        Ordered = 1,
                        MininumStockWebshop = 7
                    }
                },
                Country = new Country() { Id = 1, IsoNumber = 380}
            },
            new()
            {
                Id = 5,
                ArticleNumber = 5,
                ProductTitle = "Wein",
                ProductTitleItalian = "Vino",
                ProductTitleFrench = "",
                Vat = new Vat(){ Id = 1, Percentage = (decimal)8.0 },
                Cycle = new Cycle(){ Id = 84 },
                ECommerceGroup = new ArticleECommerceGroup(){ Id = 2 },
                ArticleType = 2,
                Category = new ArticleCategory(){ Id = 10 },
                SalesPrices = new List<SalesPrice>()
                {
                    new (){ Id = 1, Price = (decimal)12.50, PriceGroup = new PriceGroup(){Id = 3, Description = "Gastronomie"}, IsActive = true, ValidFrom = DateTime.Today},
                },
                BulkPackage = new BulkPackage(){Id = 1, AbbreviationForWeb = "Ca"},
                Filling = new Filling(){Id = 1, AbbreviationForWeb = "BO"},
                Stocks = new List<Stock>()
                {
                    new ()
                    {
                        Id = 1,
                        Article = null,
                        Warehouse = new Warehouse(){ Id = 1, Description = "Testlager"},
                        OnStock = 35,
                        Reserved = 3,
                        Provision = 1,
                        Ordered = 1,
                        MininumStockWebshop = 7
                    }
                }
            },
            new()
            {
                Id = 6,
                ArticleNumber = 6,
                ProductTitle = "Cola",
                ProductTitleItalian = "Cola",
                ProductTitleFrench = "Cola",
                EanCode1 = 1234567899876,
                EanCode2 = 9876543211234,
                EanCode3 = 4567891234567,
                EanCode4 = 123789456123,
                Vat = new Vat(){ Id = 1, Percentage = (decimal)7.7 },
                Cycle = new Cycle(){ Id = 1 },
                ECommerceGroup = new ArticleECommerceGroup(){ Id = 1 },
                ArticleType = 5,
                Category = new ArticleCategory(){ Id = 10 },
                SalesPrices = new List<SalesPrice>()
                {
                    new (){ Id = 1, Price = (decimal)17.50, PriceGroup = new PriceGroup(){Id = 3, Description = "Gastronomie"}, IsActive = true, ValidFrom = DateTime.Today},
                    new (){ Id = 1, Price = 20, PriceGroup = new PriceGroup(){Id = 2, Description = "Divers"}, IsActive = true, ValidFrom = DateTime.Today}
                },
                BulkPackage = new BulkPackage(){Id = 1, AbbreviationForWeb = "BO"},
                Stocks = new List<Stock>()
                {
                    new ()
                    {
                        Id = 1,
                        Article = null,
                        Warehouse = new Warehouse(){ Id = 1, Description = "Testlager"},
                        OnStock = 5,
                        Reserved = 3,
                        Provision = 1,
                        Ordered = 1,
                        MininumStockWebshop = 7
                    }
                },
                Country = new Country() { Id = 1, IsoNumber = 380}
            },
            new()
            {
                Id = 7,
                ArticleNumber = 7,
                ProductTitle = "Cola",
                ProductTitleItalian = "Cola",
                ProductTitleFrench = "Cola",
                EanCode1 = 1234567899876,
                EanCode2 = 9876543211234,
                EanCode3 = 4567891234567,
                EanCode4 = 123789456123,
                Vat = new Vat(){ Id = 1, Percentage = (decimal)7.7 },
                Cycle = new Cycle(){ Id = 1 },
                ECommerceGroup = new ArticleECommerceGroup(){ Id = 1 },
                ArticleType = 6,
                Category = new ArticleCategory(){ Id = 10 },
                SalesPrices = new List<SalesPrice>()
                {
                    new (){ Id = 1, Price = (decimal)17.50, PriceGroup = new PriceGroup(){Id = 3, Description = "Gastronomie"}, IsActive = true, ValidFrom = DateTime.Today},
                    new (){ Id = 1, Price = 20, PriceGroup = new PriceGroup(){Id = 2, Description = "Divers"}, IsActive = true, ValidFrom = DateTime.Today}
                },
                BulkPackage = new BulkPackage(){Id = 1, AbbreviationForWeb = "Ca"},
                Filling = new Filling(){Id = 1, AbbreviationForWeb = "BO"},
                Stocks = new List<Stock>()
                {
                    new ()
                    {
                        Id = 1,
                        Article = null,
                        Warehouse = new Warehouse(){ Id = 1, Description = "Testlager"},
                        OnStock = 5,
                        Reserved = 3,
                        Provision = 1,
                        Ordered = 1,
                        MininumStockWebshop = 7
                    }
                },
                Country = new Country() { Id = 1, IsoNumber = 380}
            },
            new()
            {
                Id = 8,
                ArticleNumber = 8,
                ProductTitle = "",
                ProductTitleItalian = "",
                ProductTitleFrench = "",
                EanCode1 = 1234567899876,
                EanCode2 = 9876543211234,
                EanCode3 = 4567891234567,
                EanCode4 = 123789456123,
                Vat = new Vat(){ Id = 1, Percentage = (decimal)7.7 },
                Cycle = new Cycle(){ Id = 1 },
                ECommerceGroup = new ArticleECommerceGroup(){ Id = 1 },
                ArticleType = 5,
                Category = new ArticleCategory(){ Id = 10 },
                SalesPrices = new List<SalesPrice>()
                {
                    new (){ Id = 1, Price = (decimal)17.50, PriceGroup = new PriceGroup(){Id = 3, Description = "Gastronomie"}, IsActive = true, ValidFrom = DateTime.Today},
                    new (){ Id = 1, Price = 20, PriceGroup = new PriceGroup(){Id = 2, Description = "Divers"}, IsActive = true, ValidFrom = DateTime.Today}
                },
                Filling = new Filling(){Id = 1, AbbreviationForWeb = "KGM"},
                Stocks = new List<Stock>()
                {
                    new ()
                    {
                        Id = 1,
                        Article = null,
                        Warehouse = new Warehouse(){ Id = 1, Description = "Testlager"},
                        OnStock = 5,
                        Reserved = 3,
                        Provision = 1,
                        Ordered = 1,
                        MininumStockWebshop = 7
                    }
                },
                Country = new Country() { Id = 1, IsoNumber = 380}
            }
        };
    }
}