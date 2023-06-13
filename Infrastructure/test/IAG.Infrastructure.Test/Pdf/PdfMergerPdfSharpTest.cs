using IAG.Infrastructure.Pdf;
using System.IO;
using Xunit;

namespace IAG.Infrastructure.Test.Pdf;

public class PdfMergerPdfSharpTest
{
    [Fact]
    public void ConvertImageToPdfTest()
    {
        var merger = new PdfMergerPdfSharp();

        using var pdfStreamSingleJpg = GetStreamFromResources("singleJpg.pdf");
        using var pdfStreamEmpty = GetStreamFromResources("empty.pdf");
        using var pdfStreamSinglePng = GetStreamFromResources("singlePng.pdf");
        using var outputPdfStream = new MemoryStream();

        merger.MergePdfs(new []
        {
            pdfStreamSingleJpg, pdfStreamEmpty,pdfStreamSinglePng
        }, outputPdfStream);


        Assert.True(outputPdfStream.Length > 0);
    }

    private Stream GetStreamFromResources(string fileName)
        => GetType().Assembly.GetManifestResourceStream($"{GetType().Namespace}.Examples.{fileName}");

}