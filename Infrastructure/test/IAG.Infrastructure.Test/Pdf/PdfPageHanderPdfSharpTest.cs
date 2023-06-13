using System;

using IAG.Infrastructure.Pdf;
using System.IO;
using Xunit;

namespace IAG.Infrastructure.Test.Pdf;

public class PdfPageHandlerPdfSharpTest
{
    [Fact]
    public void GetPageCountTest()
    {
        using var pageHandler = new PdfPageHandlerPdfSharp();

        using var pdfStreamSingleJpg = GetStreamFromResources("singleJpg.pdf");
        using var pdfStreamEmpty = GetStreamFromResources("empty.pdf");
        using var pdfStreamSinglePng = GetStreamFromResources("singlePng.pdf");
        using var pdfStreamMultiPage = GetStreamFromResources("multiPage.pdf");

        pageHandler.Load(pdfStreamSingleJpg);
        var singleJpgPageCount = pageHandler.GetPageCount();
        pageHandler.Load(pdfStreamEmpty);
        var emptyPageCount = pageHandler.GetPageCount();
        pageHandler.Load(pdfStreamSinglePng);
        var singlePngPageCount = pageHandler.GetPageCount();
        pageHandler.Load(pdfStreamMultiPage);
        var multiPageCount = pageHandler.GetPageCount();
        pageHandler.Close();

        Assert.Equal(1, singleJpgPageCount);
        Assert.Equal(1, emptyPageCount);
        Assert.Equal(1, singlePngPageCount);
        Assert.Equal(2, multiPageCount);
    }

    [Fact]
    public void ExtractPdfPageTest()
    {
        using var pageHandler = new PdfPageHandlerPdfSharp();

        using var pdfStreamInput = GetStreamFromResources("multiPage.pdf");
        using var pageOnePdfStream = new MemoryStream();
        using var pageTwoPdfStream = new MemoryStream();

        pageHandler.Load(pdfStreamInput);
        pageHandler.ExtractPdfPage(0, pageOnePdfStream);
        pageHandler.ExtractPdfPage(1, pageTwoPdfStream);

        Assert.True(pageOnePdfStream.Length > 0);
        Assert.True(pageTwoPdfStream.Length > 0);
        Assert.Throws<IndexOutOfRangeException>(() => pageHandler.ExtractPdfPage(2, new MemoryStream()));
    }

    private Stream GetStreamFromResources(string fileName)
        => GetType().Assembly.GetManifestResourceStream($"{GetType().Namespace}.Examples.{fileName}");

}