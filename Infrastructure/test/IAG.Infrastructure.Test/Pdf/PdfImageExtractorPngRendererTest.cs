using IAG.Infrastructure.Pdf;
using System.IO;
using System.Linq;

using Xunit;

namespace IAG.Infrastructure.Test.Pdf;

public class PdfImageExtractorPngRendererTest
{
    private readonly PdfImageExtractorPngRenderer _converter = new();

    [Fact]
    public void ExtractImageFromPdfWithSinglePageTest()
    {
        using var pdfStream = GetStreamFromResources("singleJpg.pdf");
        var images = _converter.GetImages(pdfStream);

        var singleJpg = Assert.Single(images);
        Assert.True(singleJpg.Height > 0);
        Assert.True(singleJpg.Width > 0);
    }

    [Fact]
    public void ExtractImageFromPdfWithMultiplePagesTest()
    {
        using var pdfStream = GetStreamFromResources("multiPage.pdf");
        var images = _converter.GetImages(pdfStream)?.ToList();

        Assert.NotNull(images);
        Assert.True(images.Count == 2);
        Assert.All(images, image => Assert.True(image.Height > 0));
        Assert.All(images, image => Assert.True(image.Width > 0));
    }

    private Stream GetStreamFromResources(string fileName)
        => GetType().Assembly.GetManifestResourceStream($"{GetType().Namespace}.Examples.{fileName}");
}