using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.ProcessEngine.Enum;
using IAG.VinX.CDV.Wamas.Common.Config;
using IAG.VinX.CDV.Wamas.Common.DataAccess;
using IAG.VinX.CDV.Wamas.PartnerExport.BusinessLogic;
using IAG.VinX.CDV.Wamas.PartnerExport.Dto;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Wamas.PartnerExport.BusinessLogic;

public class PartnerExportTest
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
        var partnerExporter = new PartnerExporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);

        // Act
        partnerExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Assert
        fakeConnectionFactory.Verify(
            con => con.CreateConnection(connectionString, It.IsAny<IUserContext>(), It.IsAny<ILogger>()), Times.Once);
    }

    [Fact]
    public void ExportPartners_WhenNoException_ShouldReturnJobResultSuccess_ShouldExportFile()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var ftpConfig = new WamasFtpConfig();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Partner>())
            .Returns(PartnerExporterTestData.GetPartner().AsQueryable);
        fakeVinXConnector.Setup(vc => vc.GetQueryable<PartnerAddress>())
            .Returns(PartnerExporterTestData.GetAddresses().AsQueryable);
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var partnerExporter = new PartnerExporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        partnerExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = partnerExporter.ExportPartner(DateTime.MinValue);

        // Assert
        Assert.Equal(JobResultEnum.Success, jobResult.Result);
        Assert.Equal(4, jobResult.ExportedCount);
        Assert.Equal(0, jobResult.ErrorCount);
        fakeFtpConnector.Verify(ftp => ftp.SetConfig(ftpConfig), Times.Once);
        fakeFtpConnector.Verify(
            ftp => ftp.UploadFile(It.IsAny<byte[]>(), It.Is<string>(s => s.Contains("PARTNER00008"))), Times.Once);
    }

    [Fact]
    public void ExportPartners_WhenException_ShouldReturnJobResultFailed_ShouldLogErrorMessage()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        var ftpConfig = new WamasFtpConfig();
        var fakeFtpConnector = new Mock<IFtpConnector>();
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeException = new Exception("This is a fake exception");
        var fakeVinXConnector = new Mock<ISybaseConnection>();
        fakeVinXConnector.Setup(vc => vc.GetQueryable<Partner>()).Throws(fakeException);
        var fakeConnectionFactory = new Mock<ISybaseConnectionFactory>();
        fakeConnectionFactory
            .Setup(factory =>
                factory.CreateConnection(It.IsAny<string>(), It.IsAny<IUserContext>(), It.IsAny<ILogger>()))
            .Returns(fakeVinXConnector.Object);
        var partnerExporter = new PartnerExporter(fakeConnectionFactory.Object, fakeFtpConnector.Object);
        partnerExporter.SetConfig(ftpConfig, connectionString, fakeMessageLogger.Object);

        // Act
        var jobResult = partnerExporter.ExportPartner(DateTime.MinValue);

        // Assert
        Assert.Equal(JobResultEnum.Failed, jobResult.Result);
        Assert.Equal(0, jobResult.ExportedCount);
        Assert.Equal(1, jobResult.ErrorCount);
        fakeMessageLogger.Verify(
            logger => logger.AddMessage(MessageTypeEnum.Error,
                "CDV.Job.Wamas.Fehler beim Exportieren von Partnern & Adressen zu WAMAS, {0}", fakeException.Message),
            Times.Once);
    }
}

public static class PartnerExporterTestData
{
    public static List<Partner> GetPartner()
    {
        return new List<Partner>
        {
            new()
            {
                PartnerId = "1",
                IsSupplier = 0,
                IsFreightCarrier = 0,
                IsDeliveryBlocked = 0,
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 1,
                RecordDate = new DateTime(2022, 07, 21, 08, 00, 00),
                DatasetType = "PARTNER00008",
                ClientId = "CDV",
                IsCustomer = 0
            },
            new()
            {
                PartnerId = "2",
                IsSupplier = 0,
                IsFreightCarrier = 0,
                IsDeliveryBlocked = 0,
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 3,
                RecordDate = new DateTime(2022, 07, 21, 08, 00, 00),
                DatasetType = "PARTNER00008",
                ClientId = "CDV",
                IsCustomer = 0
            }
        };
    }

    public static List<PartnerAddress> GetAddresses()
    {
        return new List<PartnerAddress>
        {
            new()
            {
                PartnerId = "1",
                AddressKind = "DELIVERY",
                Name = "John Doe",
                Country = "CH",
                Language = "FR",
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 2,
                RecordDate = new DateTime(2022, 07, 21, 08, 00, 00),
                DatasetType = "PARTNERADDR00007",
                ClientId = "CDV"
            },
            new()
            {
                PartnerId = "2",
                AddressKind = "INVOICE",
                Name = "Jane Doe",
                Country = "DE",
                Language = "DE",
                Source = "VINX",
                Target = "WAMAS",
                SerialNumber = 4,
                RecordDate = new DateTime(2022, 07, 21, 08, 00, 00),
                DatasetType = "PARTNERADDR00007",
                ClientId = "CDV"
            }
        };
    }
}