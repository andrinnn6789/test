using System.Collections.Generic;
using System.IO;

using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace IAG.Infrastructure.Pdf;

public class PdfMergerPdfSharp : IPdfMerger
{
    public void MergePdfs(IEnumerable<Stream> inputPfdStreams, Stream outputPdfStream)
    {
        using var outputPdf = new PdfDocument();
        foreach (var inputPfdStream in inputPfdStreams)
        {
            using var pdfDocument = PdfReader.Open(inputPfdStream, PdfDocumentOpenMode.Import);
            foreach (PdfPage page in pdfDocument.Pages)
            {
                outputPdf.AddPage(page);
            }
        }

        outputPdf.Save(outputPdfStream);
    }
}