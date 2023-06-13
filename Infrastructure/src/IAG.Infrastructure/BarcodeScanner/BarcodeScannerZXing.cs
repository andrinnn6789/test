using System;
using System.Collections.Generic;
using System.Linq;

using SkiaSharp;

using ZXing;
using ZXing.SkiaSharp;

namespace IAG.Infrastructure.BarcodeScanner;

public class BarcodeScannerZXing : IBarcodeScanner
{
    public IEnumerable<Barcode> ExtractBarcodes(SKBitmap image, BarcodeScanOptions options = default)
    {
        var possibleFormats = (options?.PossibleBarcodeTypes ?? Enum.GetValues<BarcodeType>())
            .Where(type => type != BarcodeType.Unknown)
            .Select(MapBarcodeType)
            .ToList();

        var reader = new BarcodeReader
        {
            Options =
            {
                TryHarder = true,
                PureBarcode = options?.ContainsBarcodeOnly ?? false,
                PossibleFormats = possibleFormats
            }
        };

        var result = reader.DecodeMultiple(image);

        return result?.Select(MapBarcodeResult) ?? Array.Empty<Barcode>();
    }

    private BarcodeFormat MapBarcodeType(BarcodeType barcodeType)
    {
        return barcodeType switch
        {
            BarcodeType.Code128 => BarcodeFormat.CODE_128,
            BarcodeType.QrCode => BarcodeFormat.QR_CODE,
            _ => throw new NotSupportedException($"The barcodeType {barcodeType} is not supported")
        };
    }

    private Barcode MapBarcodeResult(Result barcode)
    {
        return new()
        {
            Type = MapBarcodeFormat(barcode.BarcodeFormat),
            Value = barcode.Text
        };
    }

    private BarcodeType MapBarcodeFormat(BarcodeFormat barcodeFormat)
    {
        return barcodeFormat switch
        {
            BarcodeFormat.CODE_128 => BarcodeType.Code128,
            BarcodeFormat.QR_CODE => BarcodeType.QrCode,
            _ => BarcodeType.Unknown
        };
    }
}