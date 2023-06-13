using System;
using System.Linq;
using System.Reflection;

using IAG.Infrastructure.BarcodeScanner;

using SkiaSharp;

using Xunit;

using ZXing;

namespace IAG.Infrastructure.Test.BarcodeScanner;

public class BarcodeScannerZXingTest
{
    private static readonly BarcodeScanOptions ScanOptionsCode128Only = new()
    {
        PossibleBarcodeTypes = new[] { BarcodeType.Code128 },
        ContainsBarcodeOnly = false
    };
    private static readonly BarcodeScanOptions ScanOptionsQrCodeOnly = new()
    {
        PossibleBarcodeTypes = new[] { BarcodeType.QrCode },
        ContainsBarcodeOnly = false
    };

    [Fact]
    public void ExtractBarcodesCode128Test()
    {
        var barcodeScanner = new BarcodeScannerZXing();

        var barcodeImage = GetBitmapFromResources("code128.png");

        var resultsWithoutOptions = barcodeScanner.ExtractBarcodes(barcodeImage)?.ToList();
        var resultsWithLimitationToCode128 = barcodeScanner.ExtractBarcodes(barcodeImage, ScanOptionsCode128Only)?.ToList();
        var resultsWithLimitationToQrCode = barcodeScanner.ExtractBarcodes(barcodeImage, ScanOptionsQrCodeOnly)?.ToList();

        Assert.NotNull(resultsWithoutOptions);
        Assert.NotNull(resultsWithLimitationToCode128);
        Assert.NotNull(resultsWithLimitationToQrCode);
        Assert.Empty(resultsWithLimitationToQrCode);
        Assert.Equal("ABC-abc-1234", Assert.Single(resultsWithoutOptions)?.Value);
        Assert.Equal("ABC-abc-1234", Assert.Single(resultsWithLimitationToCode128)?.Value);
    }

    [Fact]
    public void ExtractBarcodesQrCodeTest()
    {
        var barcodeScanner = new BarcodeScannerZXing();

        var barcodeImage = GetBitmapFromResources("qrcode.png");

        var resultsWithoutOptions = barcodeScanner.ExtractBarcodes(barcodeImage)?.ToList();
        var resultsWithLimitationToCode128 = barcodeScanner.ExtractBarcodes(barcodeImage, ScanOptionsCode128Only)?.ToList();
        var resultsWithLimitationToQrCode = barcodeScanner.ExtractBarcodes(barcodeImage, ScanOptionsQrCodeOnly)?.ToList();

        Assert.NotNull(resultsWithoutOptions);
        Assert.NotNull(resultsWithLimitationToCode128);
        Assert.NotNull(resultsWithLimitationToQrCode);
        Assert.Empty(resultsWithLimitationToCode128);
        Assert.Equal("https://i-ag.ch/de/", Assert.Single(resultsWithoutOptions)?.Value);
        Assert.Equal("https://i-ag.ch/de/", Assert.Single(resultsWithLimitationToQrCode)?.Value);
    }

    [Fact]
    public void ExtractBarcodesBothTest()
    {
        var barcodeScanner = new BarcodeScannerZXing();

        var barcodeImage = GetBitmapFromResources("both.png");

        var resultsWithoutOptions = barcodeScanner.ExtractBarcodes(barcodeImage)?.ToList();
        var resultsWithLimitationToCode128 = barcodeScanner.ExtractBarcodes(barcodeImage, ScanOptionsCode128Only)?.ToList();
        var resultsWithLimitationToQrCode = barcodeScanner.ExtractBarcodes(barcodeImage, ScanOptionsQrCodeOnly)?.ToList();

        Assert.NotNull(resultsWithoutOptions);
        Assert.NotNull(resultsWithLimitationToCode128);
        Assert.NotNull(resultsWithLimitationToQrCode);
        Assert.Equal(2, resultsWithoutOptions.Count);
        Assert.Equal("ABC-abc-1234", Assert.Single(resultsWithLimitationToCode128)?.Value);
        Assert.Equal("https://i-ag.ch/de/", Assert.Single(resultsWithLimitationToQrCode)?.Value);
        Assert.Contains(resultsWithoutOptions, barcode => barcode.Value == "ABC-abc-1234" && barcode.Type == BarcodeType.Code128);
        Assert.Contains(resultsWithoutOptions, barcode => barcode.Value == "https://i-ag.ch/de/" && barcode.Type == BarcodeType.QrCode);
    }

    [Fact]
    public void ExtractBarcodesBothRotatedTest()
    {
        var barcodeScanner = new BarcodeScannerZXing();

        var barcodeImage = GetBitmapFromResources("bothRotated.png");

        var resultsWithoutOptions = barcodeScanner.ExtractBarcodes(barcodeImage)?.ToList();
        var resultsWithLimitationToCode128 = barcodeScanner.ExtractBarcodes(barcodeImage, ScanOptionsCode128Only)?.ToList();
        var resultsWithLimitationToQrCode = barcodeScanner.ExtractBarcodes(barcodeImage, ScanOptionsQrCodeOnly)?.ToList();

        Assert.NotNull(resultsWithoutOptions);
        Assert.NotNull(resultsWithLimitationToCode128);
        Assert.NotNull(resultsWithLimitationToQrCode);
        Assert.Equal(2, resultsWithoutOptions.Count);
        Assert.Equal("ABC-abc-1234", Assert.Single(resultsWithLimitationToCode128)?.Value);
        Assert.Equal("https://i-ag.ch/de/", Assert.Single(resultsWithLimitationToQrCode)?.Value);
        Assert.Contains(resultsWithoutOptions, barcode => barcode.Value == "ABC-abc-1234");
        Assert.Contains(resultsWithoutOptions, barcode => barcode.Value == "https://i-ag.ch/de/");
    }

    [Fact]
    public void ExtractBarcodesBothRotatedAndDirtyTest()
    {
        var barcodeScanner = new BarcodeScannerZXing();

        var barcodeImage = GetBitmapFromResources("bothRotatedAndDirty.png");

        var resultsWithoutOptions = barcodeScanner.ExtractBarcodes(barcodeImage)?.ToList();
        var resultsWithLimitationToCode128 = barcodeScanner.ExtractBarcodes(barcodeImage, ScanOptionsCode128Only)?.ToList();
        var resultsWithLimitationToQrCode = barcodeScanner.ExtractBarcodes(barcodeImage, ScanOptionsQrCodeOnly)?.ToList();

        Assert.NotNull(resultsWithoutOptions);
        Assert.NotNull(resultsWithLimitationToCode128);
        Assert.NotNull(resultsWithLimitationToQrCode);
        Assert.Equal(2, resultsWithoutOptions.Count);
        Assert.Equal("ABC-abc-1234", Assert.Single(resultsWithLimitationToCode128)?.Value);
        Assert.Equal("https://i-ag.ch/de/", Assert.Single(resultsWithLimitationToQrCode)?.Value);
        Assert.Contains(resultsWithoutOptions, barcode => barcode.Value == "ABC-abc-1234" && barcode.Type == BarcodeType.Code128);
        Assert.Contains(resultsWithoutOptions, barcode => barcode.Value == "https://i-ag.ch/de/" && barcode.Type == BarcodeType.QrCode);
    }

    [Fact]
    public void ExtractBarcodesNoBarcodeTest()
    {
        var barcodeScanner = new BarcodeScannerZXing();

        var barcodeImage = GetBitmapFromResources("nobarcode.png");

        var resultsWithoutOptions = barcodeScanner.ExtractBarcodes(barcodeImage)?.ToList();
        var resultsWithLimitationToCode128 = barcodeScanner.ExtractBarcodes(barcodeImage, ScanOptionsCode128Only)?.ToList();
        var resultsWithLimitationToQrCode = barcodeScanner.ExtractBarcodes(barcodeImage, ScanOptionsQrCodeOnly)?.ToList();

        Assert.NotNull(resultsWithoutOptions);
        Assert.NotNull(resultsWithLimitationToCode128);
        Assert.NotNull(resultsWithLimitationToQrCode);
        Assert.Empty(resultsWithoutOptions);
        Assert.Empty(resultsWithLimitationToCode128);
        Assert.Empty(resultsWithLimitationToQrCode);
    }

    [Fact]
    public void ExtractBarcodesCode128EmbeddedTest()
    {
        var barcodeScanner = new BarcodeScannerZXing();

        var barcodeImage = GetBitmapFromResources("code128Embedded.png");

        var resultsWithoutOptions = barcodeScanner.ExtractBarcodes(barcodeImage)?.ToList();
        var resultsWithLimitationToCode128 = barcodeScanner.ExtractBarcodes(barcodeImage, ScanOptionsCode128Only)?.ToList();
        var resultsWithLimitationToQrCode = barcodeScanner.ExtractBarcodes(barcodeImage, ScanOptionsQrCodeOnly)?.ToList();

        Assert.NotNull(resultsWithoutOptions);
        Assert.NotNull(resultsWithLimitationToCode128);
        Assert.NotNull(resultsWithLimitationToQrCode);
        Assert.Empty(resultsWithLimitationToQrCode);
        Assert.Equal("ABC-abc-1234", Assert.Single(resultsWithoutOptions)?.Value);
        Assert.Equal("ABC-abc-1234", Assert.Single(resultsWithLimitationToCode128)?.Value);
    }

    [Fact]
    public void ExtractBarcodesQrCodeEmbeddedTest()
    {
        var barcodeScanner = new BarcodeScannerZXing();

        var barcodeImage = GetBitmapFromResources("qrcodeEmbedded.png");

        var resultsWithoutOptions = barcodeScanner.ExtractBarcodes(barcodeImage)?.ToList();
        var resultsWithLimitationToCode128 = barcodeScanner.ExtractBarcodes(barcodeImage, ScanOptionsCode128Only)?.ToList();
        var resultsWithLimitationToQrCode = barcodeScanner.ExtractBarcodes(barcodeImage, ScanOptionsQrCodeOnly)?.ToList();

        Assert.NotNull(resultsWithoutOptions);
        Assert.NotNull(resultsWithLimitationToCode128);
        Assert.NotNull(resultsWithLimitationToQrCode);
        Assert.Empty(resultsWithLimitationToCode128);
        Assert.Equal("Simple-Test-QR-Code", Assert.Single(resultsWithoutOptions)?.Value);
        Assert.Equal("Simple-Test-QR-Code", Assert.Single(resultsWithLimitationToQrCode)?.Value);
    }


    [Fact]
    public void ExtractBarcodesExceptionsTest()
    {
        var barcodeScanner = new BarcodeScannerZXing();

        var barcodeImage = GetBitmapFromResources("nobarcode.png");
        var invalidOptions = new BarcodeScanOptions
        {
            PossibleBarcodeTypes = new[] {(BarcodeType) 42}
        };

        Assert.Throws<NotSupportedException>(() => barcodeScanner.ExtractBarcodes(barcodeImage, invalidOptions));
    }

    [Fact]
    public void MapBarcodeFormatTest()
    {
        var barcodeScanner = new BarcodeScannerZXing();

        var mapMethod = barcodeScanner.GetType().GetMethod("MapBarcodeFormat", BindingFlags.NonPublic | BindingFlags.Instance);
        var resultCode128 = mapMethod?.Invoke(barcodeScanner, new object[] { BarcodeFormat.CODE_128 });
        var resultQrCode = mapMethod?.Invoke(barcodeScanner, new object[] { BarcodeFormat.QR_CODE });
        var resultUnknown = mapMethod?.Invoke(barcodeScanner, new object[] { BarcodeFormat.AZTEC });

        Assert.Equal(BarcodeType.Code128, resultCode128);
        Assert.Equal(BarcodeType.QrCode, resultQrCode);
        Assert.Equal(BarcodeType.Unknown, resultUnknown);
    }

    private SKBitmap GetBitmapFromResources(string fileName)
    {
        var bitmapStream = GetType().Assembly.GetManifestResourceStream($"{GetType().Namespace}.Examples.{fileName}");
        return SKBitmap.Decode(bitmapStream!);
    }
}