using System.IO;

namespace IAG.Infrastructure.Pdf;

public interface IImageToPdfConverter
{
    void ConvertImageToPdf(Stream imageStream, Stream outputPdfStream);
}