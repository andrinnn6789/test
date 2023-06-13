using IAG.Common.DataLayerNHibernate;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.CDV.Gastivo.Common.Config;
using IAG.VinX.CDV.Gastivo.Common.Ftp;
using IAG.VinX.CDV.Gastivo.OrderImport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using Moq;

using NHibernate;

using Xunit;

namespace IAG.VinX.CDV.Test.Gastivo.OrderImport.BusinessLogic;

public class OrderImporterTest
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
        var orderImporter = new OrderImporter(fakeSessionFactory.Object, fakeFtpConnector.Object);

        // Act
        orderImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Assert
        fakeSessionFactory.Verify(
            con => con.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
    }

    [Theory]
    [InlineData(@"Gastivo\OrderImport\TestData\order-successfull.txt")]
    public void ImportOrders_WhenNoException_ShouldReturnJobResultSuccess_ShouldImportOrder(string fileName)
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fileList = new List<string> { fileName };

        var ftpConfig = new GastivoFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        fakeFtpConnector.Setup(f => f.GetFiles("order")).Returns(fileList);
        
        using var stream = new MemoryStream();
        using var fileStream = File.OpenRead(fileName);
        fileStream.CopyTo(stream);
        fakeFtpConnector.Setup(f => f.DownloadFile(fileName)).Returns(stream.ToArray);
        
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeTransaction = new Mock<ITransaction>();
        fakeTransaction.Setup(t => t.Commit());
        fakeTransaction.Setup(t => t.Rollback());
        
        var fakeVinXSession = new Mock<ISession>();
        fakeVinXSession.Setup(vc => vc.Query<Article>())
            .Returns(OrderImporterTestData.GetArticles().AsQueryable());
        fakeVinXSession.Setup(vc => vc.Query<Address>())
            .Returns(OrderImporterTestData.GetAddresses().AsQueryable());
        fakeVinXSession.Setup(vc => vc.Query<ServiceProvider>())
            .Returns(OrderImporterTestData.GetServiceProviders().AsQueryable);
        fakeVinXSession.Setup(vc => vc.BeginTransaction()).Returns(fakeTransaction.Object);
        var fakeVinXSessionContext = new Mock<ISessionContext>();
        fakeVinXSessionContext.Setup(sc => sc.Session).Returns(fakeVinXSession.Object);
        var fakeSessionFactory = new Mock<ISessionContextFactory>();
        fakeSessionFactory
            .Setup(factory => factory.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(fakeVinXSessionContext.Object);
        var orderImporter = new OrderImporter(fakeSessionFactory.Object, fakeFtpConnector.Object);
        orderImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = orderImporter.ImportOrders();

        // Assert
        Assert.Equal(JobResultEnum.Success, jobResult.Result);
        Assert.Equal(1, jobResult.ImportedCount);
        Assert.Equal(0, jobResult.ErrorCount);
        fakeFtpConnector.Verify(ftp => ftp.SetConfig(ftpConfig), Times.Once);
        fakeFtpConnector.Verify(
            ftp => ftp.DownloadFile(It.Is<string>(s => s.Contains("order"))), Times.Once);
        fakeFtpConnector.Verify(ftp => ftp.DeleteFile(It.Is<string>(s => s.Contains("order"))), Times.Once);
    }

    [Theory]
    [InlineData(@"Gastivo\OrderImport\TestData\order-wrongarticle.txt")]
    public void ImportOrders_WhenException_ShouldReturnJobResultFailed_ShouldLogErrorMessage(string fileName)
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fileList = new List<string> { fileName };

        var ftpConfig = new GastivoFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        fakeFtpConnector.Setup(f => f.GetFiles("order")).Returns(fileList);
        
        using var stream = new MemoryStream();
        using var fileStream = File.OpenRead(fileName);
        fileStream.CopyTo(stream);
        fakeFtpConnector.Setup(f => f.DownloadFile(fileName)).Returns(stream.ToArray);
        
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXSession = new Mock<ISession>();
        fakeVinXSession.Setup(vc => vc.Query<Article>())
            .Returns(OrderImporterTestData.GetArticles().AsQueryable);
        fakeVinXSession.Setup(vc => vc.Query<Address>())
            .Returns(OrderImporterTestData.GetAddresses().AsQueryable);
        fakeVinXSession.Setup(vc => vc.Query<ServiceProvider>())
            .Returns(OrderImporterTestData.GetServiceProviders().AsQueryable);
        var fakeVinXSessionContext = new Mock<ISessionContext>();
        fakeVinXSessionContext.Setup(sc => sc.Session).Returns(fakeVinXSession.Object);
        var fakeSessionFactory = new Mock<ISessionContextFactory>();
        fakeSessionFactory
            .Setup(factory => factory.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(fakeVinXSessionContext.Object);
        var orderImporter = new OrderImporter(fakeSessionFactory.Object, fakeFtpConnector.Object);
        orderImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = orderImporter.ImportOrders();

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ImportedCount);
        Assert.Equal(1, jobResult.ErrorCount);
        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Gastivo.Fehler beim Importieren der Bestellungs-Datei {0} aus Gastivo, {1}",
                It.IsAny<object[]>()),
            Times.Once);
        fakeFtpConnector.Verify(ftp => ftp.DeleteFile(It.Is<string>(s => s.Contains("order"))), Times.Never);
    }
    
    [Fact]
    public void ImportOrders_WhenExceptionReadingFile_ShouldReturnJobResultFailed_ShouldLogErrorMessage()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fakeException = new Exception("This is a fake exception");
        var ftpConfig = new GastivoFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        fakeFtpConnector.Setup(f => f.GetFiles(It.IsAny<string>())).Throws(fakeException);
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXSession = new Mock<ISession>();
        var fakeVinXSessionContext = new Mock<ISessionContext>();
        fakeVinXSessionContext.Setup(sc => sc.Session).Returns(fakeVinXSession.Object);
        var fakeSessionFactory = new Mock<ISessionContextFactory>();
        fakeSessionFactory
            .Setup(factory => factory.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(fakeVinXSessionContext.Object);
        var orderImporter = new OrderImporter(fakeSessionFactory.Object, fakeFtpConnector.Object);
        orderImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = orderImporter.ImportOrders();

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ImportedCount);
        Assert.Equal(1, jobResult.ErrorCount);
        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Gastivo.Fehler beim Importieren von Bestellungen aus Gastivo, {0}", fakeException),
            Times.Once);
        fakeFtpConnector.Verify(ftp => ftp.DeleteFile(It.Is<string>(s => s.Contains("order"))), Times.Never);
    }

    [Theory]
    [InlineData(@"Gastivo\OrderImport\TestData\order-successfull.txt")]
    public void ImportOrders_WhenExceptionServiceProviderNotFound_ShouldReturnJobResultFailed_ShouldLogErrorMessage(string fileName)
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fileList = new List<string> { fileName };

        var ftpConfig = new GastivoFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        fakeFtpConnector.Setup(f => f.GetFiles("order")).Returns(fileList);

        using var stream = new MemoryStream();
        using var fileStream = File.OpenRead(fileName);
        fileStream.CopyTo(stream);
        fakeFtpConnector.Setup(f => f.DownloadFile(fileName)).Returns(stream.ToArray);

        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeTransaction = new Mock<ITransaction>();
        fakeTransaction.Setup(t => t.Commit());
        fakeTransaction.Setup(t => t.Rollback());

        var fakeVinXSession = new Mock<ISession>();
        fakeVinXSession.Setup(vc => vc.Query<Article>())
            .Returns(OrderImporterTestData.GetArticles().AsQueryable());
        fakeVinXSession.Setup(vc => vc.Query<Address>())
            .Returns(OrderImporterTestData.GetAddresses().AsQueryable());
        fakeVinXSession.Setup(vc => vc.Query<ServiceProvider>())
            .Returns(OrderImporterTestData.GetWrongServiceProviders().AsQueryable);
        fakeVinXSession.Setup(vc => vc.BeginTransaction()).Returns(fakeTransaction.Object);
        var fakeVinXSessionContext = new Mock<ISessionContext>();
        fakeVinXSessionContext.Setup(sc => sc.Session).Returns(fakeVinXSession.Object);
        var fakeSessionFactory = new Mock<ISessionContextFactory>();
        fakeSessionFactory
            .Setup(factory => factory.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(fakeVinXSessionContext.Object);
        var orderImporter = new OrderImporter(fakeSessionFactory.Object, fakeFtpConnector.Object);
        orderImporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = orderImporter.ImportOrders();

        

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ImportedCount);
        Assert.Equal(1, jobResult.ErrorCount);
        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Gastivo.Fehler beim Importieren der Bestellungs-Datei {0} aus Gastivo, {1}",
                It.IsAny<object[]>()),
            Times.Once);
        fakeFtpConnector.Verify(ftp => ftp.DeleteFile(It.Is<string>(s => s.Contains("order"))), Times.Never);
    }
}

public static class OrderImporterTestData
{
    public static IEnumerable<Article> GetArticles()
    {
        return new List<Article>
        {
            new()
            {
                Id = 1,
                ArticleNumber = 30002891,
                Description = "Test Description of 30002891",
                Vat = new Vat(){ Id = 1, Percentage = (decimal)7.7 }
            },
            
            new()
            {
                Id = 2,
                ArticleNumber = 30004028,
                Description = "Test Description of 30002891",
                Vat = new Vat(){ Id = 1, Percentage = (decimal)7.7 }
            },
        };
    }
    
    public static IEnumerable<Address> GetAddresses()
    {
        return new List<Address>
        {
            new()
            {
                Id = 1,
                AddressNumber = 206160389,
                DeliveryConditionId = 1,
                PaymentConditionId = 1
            }
        };
    }

    public static IEnumerable<ServiceProvider> GetServiceProviders()
    {
        return new List<ServiceProvider>
        {
            new()
            {
                Id = 3,
                Description = "Intern"
            },
            new()
            {
                Id = 8,
                Description = "Gastivo"
            }
        };
    }

    public static IEnumerable<ServiceProvider> GetWrongServiceProviders()
    {
        return new List<ServiceProvider>
        {
            new()
            {
                Id = 1,
                Description = "Intern"
            },
            new()
            {
                Id = 3,
                Description = "Gooschtive"
            }
        };
    }
}