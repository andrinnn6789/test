using IAG.Common.DataLayerNHibernate;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.CDV.Gastivo.Common.Config;
using IAG.VinX.CDV.Gastivo.Common.Ftp;
using IAG.VinX.CDV.Gastivo.CustomerExport.BusinessLogic;
using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using Moq;

using NHibernate;

using Xunit;

namespace IAG.VinX.CDV.Test.Gastivo.CustomerExport.BusinessLogic;

public class CustomerExporterTest
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
        var customerExporter = new CustomerExporter(fakeSessionFactory.Object, fakeFtpConnector.Object);

        // Act
        customerExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Assert
        fakeSessionFactory.Verify(
            con => con.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public void ExportCustomers_WhenNoException_ShouldReturnJobResultSuccess_ShouldExportFile()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var ftpConfig = new GastivoFtpConfig();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXSession = new Mock<ISession>();
        fakeVinXSession.Setup(vc => vc.Query<Address>())
            .Returns(CustomerExporterTestData.GetAddresses().AsQueryable());
        var fakeVinXSessionContext = new Mock<ISessionContext>();
        fakeVinXSessionContext.Setup(sc => sc.Session).Returns(fakeVinXSession.Object);
        var fakeSessionFactory = new Mock<ISessionContextFactory>();
        fakeSessionFactory
            .Setup(factory => factory.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(fakeVinXSessionContext.Object);
        var customerExporter = new CustomerExporter(fakeSessionFactory.Object, fakeFtpConnector.Object);
        customerExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = customerExporter.ExportCustomers();

        // Assert
        Assert.Equal(JobResultEnum.Success, jobResult.Result);
        Assert.Equal(2, jobResult.ExportedCount);
        Assert.Equal(0, jobResult.ErrorCount);
        fakeFtpConnector.Verify(ftp => ftp.SetConfig(ftpConfig), Times.Once);
        fakeFtpConnector.Verify(
            ftp => ftp.UploadFile(It.IsAny<byte[]>(), It.Is<string>(s => s.Contains("customers"))), Times.Once);
    }

    [Fact]
    public void ExportCustomers_WhenException_ShouldReturnJobResultFailed_ShouldLogErrorMessage()
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
        var customerExporter = new CustomerExporter(fakeSessionFactory.Object, fakeFtpConnector.Object);
        customerExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = customerExporter.ExportCustomers();

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ExportedCount);
        Assert.Equal(1, jobResult.ErrorCount);
        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Gastivo.Fehler beim Exportieren von Kundenadressen zu Gastivo, {0}", fakeException),
            Times.Once);
    }
}

public static class CustomerExporterTestData
{
    public static IEnumerable<Address> GetAddresses()
    {
        return new List<Address>
        {
            new()
            {
                Id = 1,
                AddressNumber = 1,
                FirstName = "John",
                Name = "Doe",
                IsActive = true,
                PriceGroup = new PriceGroup(){Id = 3, Description = "Gastronomie"},
                TransmitToGastivo = true
            },
            new()
            {
                Id = 2,
                AddressNumber = 2,
                FirstName = "Jane",
                Name = "Doe",
                IsActive = true,
                PriceGroup = new PriceGroup(){Id = 3, Description = "Gastronomie"},
                TransmitToGastivo = true
            },
            new()
            {
                Id = 3,
                AddressNumber = 3,
                FirstName = "Jill",
                Name = "Doe",
                IsActive = false,
                PriceGroup = new PriceGroup(){Id = 3, Description = "Gastronomie"},
                TransmitToGastivo = true
            },
            new()
            {
                Id = 4,
                AddressNumber = 4,
                FirstName = "John",
                Name = "Doe",
                IsActive = true,
                PriceGroup = new PriceGroup(){Id = 3, Description = "Gastronomie"},
                TransmitToGastivo = false
            },
        };
    }
}