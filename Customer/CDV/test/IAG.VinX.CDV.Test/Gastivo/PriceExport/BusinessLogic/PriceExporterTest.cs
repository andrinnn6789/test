using IAG.Common.DataLayerNHibernate;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.CDV.Gastivo.Common.Config;
using IAG.VinX.CDV.Gastivo.Common.Ftp;
using IAG.VinX.CDV.Gastivo.PriceExport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using Moq;

using NHibernate;

using Xunit;

namespace IAG.VinX.CDV.Test.Gastivo.PriceExport.BusinessLogic;

public class PriceExporterTest
{
    [Fact]
    public void SetConfig_ShouldCreateSession()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var ftpConfig = new GastivoFtpConfig();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXSession = new Mock<ISession>();
        var fakeVinXSessionContext = new Mock<ISessionContext>();
        fakeVinXSessionContext.Setup(sc => sc.Session).Returns(fakeVinXSession.Object);
        var fakeSessionFactory = new Mock<ISessionContextFactory>();
        fakeSessionFactory
            .Setup(factory => factory.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(fakeVinXSessionContext.Object);
        var priceExporter = new PriceExporter(fakeSessionFactory.Object, fakeFtpConnector.Object);

        // Act
        priceExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Assert
        fakeSessionFactory.Verify(
            con => con.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public void ExportPrices_WhenNoException_ShouldReturnJobResultSuccess_ShouldExportFile()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var ftpConfig = new GastivoFtpConfig();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXSession = new Mock<ISession>();
        fakeVinXSession.Setup(vc => vc.Query<Address>())
            .Returns(PriceExporterTestData.GetAddresses().AsQueryable());
        var fakeVinXSessionContext = new Mock<ISessionContext>();
        fakeVinXSessionContext.Setup(sc => sc.Session).Returns(fakeVinXSession.Object);
        var fakeSessionFactory = new Mock<ISessionContextFactory>();
        fakeSessionFactory
            .Setup(factory => factory.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(fakeVinXSessionContext.Object);
        var priceExporter = new PriceExporter(fakeSessionFactory.Object, fakeFtpConnector.Object);
        priceExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = priceExporter.ExportPrices();

        // Assert
        Assert.Equal(JobResultEnum.Success, jobResult.Result);
        Assert.Equal(3, jobResult.ExportedCount);
        Assert.Equal(0, jobResult.ErrorCount);
        fakeFtpConnector.Verify(ftp => ftp.SetConfig(ftpConfig), Times.Once);
        fakeFtpConnector.Verify(
            ftp => ftp.UploadFile(It.IsAny<byte[]>(), It.Is<string>(s => s.Contains("prices"))), Times.Once);
    }

    [Fact]
    public void ExportPrices_WhenException_ShouldReturnJobResultFailed_ShouldLogErrorMessage()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var ftpConfig = new GastivoFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeException = new Exception("This is a fake exception");
        var fakeVinXSession = new Mock<ISession>();
        fakeVinXSession.Setup(vc => vc.Query<Address>()).Throws(fakeException);
        var fakeVinXSessionContext = new Mock<ISessionContext>();
        fakeVinXSessionContext.Setup(sc => sc.Session).Returns(fakeVinXSession.Object);
        var fakeSessionFactory = new Mock<ISessionContextFactory>();
        fakeSessionFactory
            .Setup(factory => factory.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(fakeVinXSessionContext.Object);
        var priceExporter = new PriceExporter(fakeSessionFactory.Object, fakeFtpConnector.Object);
        priceExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = priceExporter.ExportPrices();

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ExportedCount);
        Assert.Equal(1, jobResult.ErrorCount);
        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Gastivo.Fehler beim Exportieren von Preisen zu Gastivo, {0}", fakeException),
            Times.Once);
    }
}

public static class PriceExporterTestData
{
    public static IList<SpecialPrice> GetPrices()
    {
        return new List<SpecialPrice>
        {
            new()
            {
                Id = 1,
                Price = (decimal)17.50,
                ValidFrom = DateTime.MinValue,
                ValidTo = null,
                Article = new Article()
                {
                    Id = 1,
                    ArticleNumber = 1,
                    StockMovements = new List<StockMovement>()
                    {
                        new()
                        {
                            Id = 1,
                            Date = DateTime.Now
                        }
                    },
                    Cycle = new Cycle() { Id = 1 },
                    ArticleType = 2,
                    Filling = new Filling() { Id = 1, AbbreviationForWeb = "Fl" },
                    ECommerceGroup = new ArticleECommerceGroup() { Id = 1 },
                }
            },
            new()
            {
                Id = 2,
                Price = (decimal)12.15,
                ValidFrom = DateTime.MinValue,
                ValidTo = null,
                Article = new Article()
                {
                    Id = 2,
                    ArticleNumber = 2,
                    StockMovements = new List<StockMovement>(),
                    Cycle = new Cycle() { Id = 1 },
                    ArticleType = 2,
                    Filling = new Filling() { Id = 1, AbbreviationForWeb = "Fl" },
                    ECommerceGroup = new ArticleECommerceGroup() { Id = 1 },
                }
            },
            new()
            {
                Id = 4,
                Price = (decimal)17.50,
                ValidFrom = DateTime.MinValue,
                ValidTo = null,
                Article = new Article()
                {
                    Id = 3,
                    ArticleNumber = 3,
                    StockMovements = new List<StockMovement>(),
                    Cycle = new Cycle() { Id = 2 },
                    ArticleType = 2,
                    Filling = new Filling() { Id = 1, AbbreviationForWeb = "Fl" },
                    ECommerceGroup = new ArticleECommerceGroup() { Id = 1 },
                }
            },
            new()
            {
                Id = 5,
                Price = (decimal)17.50,
                ValidFrom = DateTime.MinValue,
                ValidTo = null,
                Article = new Article()
                {
                    Id = 1,
                    ArticleNumber = 1,
                    StockMovements = new List<StockMovement>(),
                    Cycle = new Cycle() { Id = 1 },
                    ArticleType = 2,
                    Filling = new Filling() { Id = 1, AbbreviationForWeb = "Fl" },
                    ECommerceGroup = new ArticleECommerceGroup() { Id = 1 },
                }
            },
            new()
            {
                Id = 6,
                Price = (decimal)17.50,
                ValidFrom = DateTime.MinValue,
                ValidTo = DateTime.MinValue,
                Article = new Article()
                {
                    Id = 1,
                    ArticleNumber = 1,
                    StockMovements = new List<StockMovement>(),
                    Cycle = new Cycle() { Id = 1 },
                    ArticleType = 2,
                    Filling = new Filling() { Id = 1, AbbreviationForWeb = "Fl" },
                    ECommerceGroup = new ArticleECommerceGroup() { Id = 1 },
                }
            }
        };
    }

    public static IEnumerable<Address> GetAddresses()
    {
        return new List<Address>
        {
            new Address
            {
                PriceCondition = 10,
                SpecialPrices = GetPrices(),
                IsActive = true,
                AddressNumber = 123,
                TransmitToGastivo = true
            }
        };
    }
}