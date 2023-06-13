using IAG.Infrastructure.Pdf;
using System.IO;
using Xunit;

namespace IAG.Infrastructure.Test.Pdf;

public class ImageToPdfConverterPdfSharpTest
{
    [Fact]
    public void ConvertImageToPdfTest()
    {
        var converter = new ImageToPdfConverterPdfSharp();

        using var imageStreamPortraitJpg = GetStreamFromResources("fullPagePortrait.jpg");
        using var imageStreamLandscapeJpg = GetStreamFromResources("fullPageLandscape.jpg");
        using var imageStreamPortraitPng = GetStreamFromResources("fullPagePortrait.png");
        using var imageStreamLandscapePng = GetStreamFromResources("fullPageLandscape.png");
        using var pdfStreamPortraitJpg = new MemoryStream();
        using var pdfStreamLandscapeJpg = new MemoryStream();
        using var pdfStreamPortraitPng = new MemoryStream();
        using var pdfStreamLandscapePng = new MemoryStream();

        converter.ConvertImageToPdf(imageStreamPortraitJpg, pdfStreamPortraitJpg);
        converter.ConvertImageToPdf(imageStreamLandscapeJpg, pdfStreamLandscapeJpg);
        converter.ConvertImageToPdf(imageStreamPortraitPng, pdfStreamPortraitPng);
        converter.ConvertImageToPdf(imageStreamLandscapePng, pdfStreamLandscapePng);

        Assert.True(pdfStreamPortraitJpg.Length > 0);
        Assert.True(pdfStreamLandscapeJpg.Length > 0);
        Assert.True(pdfStreamPortraitPng.Length > 0);
        Assert.True(pdfStreamLandscapePng.Length > 0);
    }


    private Stream GetStreamFromResources(string fileName)
        => GetType().Assembly.GetManifestResourceStream($"{GetType().Namespace}.Examples.{fileName}");

}