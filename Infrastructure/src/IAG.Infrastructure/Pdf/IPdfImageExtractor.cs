using System.Collections.Generic;
using System.IO;

using SkiaSharp;

namespace IAG.Infrastructure.Pdf;

public interface IPdfImageExtractor
{
    IEnumerable<SKBitmap> GetImages(Stream pdfStream);
}