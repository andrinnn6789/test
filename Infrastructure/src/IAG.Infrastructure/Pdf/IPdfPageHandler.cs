using System.IO;

namespace IAG.Infrastructure.Pdf;

public interface IPdfPageHandler
{
    public void Load(Stream inputPfdStream);

    public int GetPageCount();

    public void ExtractPdfPage(int page, Stream outputPdfPageStream);

    public void Close();
}