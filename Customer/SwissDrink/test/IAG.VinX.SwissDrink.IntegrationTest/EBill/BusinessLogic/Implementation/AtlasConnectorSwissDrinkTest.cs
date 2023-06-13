using System.IO;
using System.Linq;

using IAG.Common.ArchiveProvider;
using IAG.Common.EBill.BusinessLogic.Implementation;
using IAG.Common.IntegrationTest.Pdf;
using IAG.Common.Pdf.Implementation;
using IAG.Common.TestHelper.Arrange;
using IAG.Common.WoD.Connectors;
using IAG.Common.WoD.Interfaces;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.SwissDrink.Dto;
using IAG.VinX.SwissDrink.EBill.BusinessLogic.Implementation;

using Moq;

using Xunit;

namespace IAG.VinX.SwissDrink.IntegrationTest.EBill.BusinessLogic.Implementation;

public class AtlasConnectorSwissDrinkTest
{
    private readonly AtlasConnectorEbillSwissDrink _connectorEbill;
    private readonly OpData _opData;

    public AtlasConnectorSwissDrinkTest()
    {             
        var factory = SybaseConnectionFactoryHelper.CreateFactory();
        var archiveProviderFactory = new ArchiveProviderFactory(new PluginLoader());
        _connectorEbill = new AtlasConnectorEbillSwissDrink(factory, archiveProviderFactory);
        _opData = factory.CreateConnection()
            .Query<OpData>()
            .Where(o => o.HasZugferd)
            .OrderByDescending(o => o.Id)
            .First();
    }

    [Fact]
    public void LoadOpFromEbillControl()
    {
        var op = _connectorEbill.LoadOp(_opData.Id);
        Assert.True(op.TotalAmount > 0);
    }

    [Fact]
    public void EmbeddPdf()
    {
        var archiveLink = _connectorEbill.GetArchivelinkQuery()
            .Where(a => a.ForeignId == _opData.ReceiptId)
            .Where(a => a.Tablename == "Beleg")
            .OrderByDescending(a => a.Id)
            .FirstOrDefault();

        var mockIWodConfigLoader = new Mock<IWodConfigLoader>();
        mockIWodConfigLoader.Setup(m => m.ProviderSetting()).Returns(ConfigHelper.ProviderSettingWodTestInstance);
        var wodConnector = new WodConnector(new MockILogger<WodConnector>(), mockIWodConfigLoader.Object);
        var pdfProcessor = new PdfProcessorWod(wodConnector);

        using var zugferdPdfStream = new MemoryStream();
        var dataName = new ZugferdEmbedder(_connectorEbill, new ZugferdBuilder(), pdfProcessor)
            .EmbedZugferd(zugferdPdfStream, archiveLink, _opData.Id);
        File.WriteAllBytes(dataName + ".pdf", zugferdPdfStream.ToArray());
    }
}