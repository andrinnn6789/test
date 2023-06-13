using System;
using System.IO;

using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace IAG.Infrastructure.Pdf;

public class PdfPageHandlerPdfSharp : IPdfPageHandler, IDisposable
{
    private PdfDocument _pdfDocument;

    public void Load(Stream inputPfdStream)
    {
        Close();
        _pdfDocument = PdfReader.Open(inputPfdStream, PdfDocumentOpenMode.Import);
    }

    public int GetPageCount()
    {
        return _pdfDocument.PageCount;
    }

    public void ExtractPdfPage(int pageIdx, Stream outputPdfPageStream)
    {
        if (pageIdx < 0 || pageIdx >= _pdfDocument.PageCount)
        {
            throw new IndexOutOfRangeException($"PDF has just {_pdfDocument.PageCount} page(s). Cannot read page {pageIdx}");
        }

        using var outputPdf = new PdfDocument();
        outputPdf.AddPage(_pdfDocument.Pages[pageIdx]);
        outputPdf.Save(outputPdfPageStream);
    }

    public void Close()
    {
        _pdfDocument?.Close();
        _pdfDocument?.Dispose();
        _pdfDocument = null;
    }

    public void Dispose() => Close();
}