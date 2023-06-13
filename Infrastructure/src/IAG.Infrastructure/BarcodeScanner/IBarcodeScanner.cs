using System.Collections.Generic;

using SkiaSharp;

namespace IAG.Infrastructure.BarcodeScanner;

public interface IBarcodeScanner
{
    IEnumerable<Barcode> ExtractBarcodes(SKBitmap image, BarcodeScanOptions options = default);
}