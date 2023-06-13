using System;
using System.IO;

using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace IAG.Infrastructure.Pdf;

public class ImageToPdfConverterPdfSharp : IImageToPdfConverter
{
    public void ConvertImageToPdf(Stream imageStream, Stream outputPdfStream)
    {
        using var document = new PdfDocument();

        XImage image = XImage.FromStream(() => imageStream);
        PdfPage page = document.AddPage();
        page.Orientation = image.PixelWidth > image.PointHeight
            ? PageOrientation.Landscape
            : PageOrientation.Portrait;

        var heightScaleFactor = image.PointHeight / page.Height.Point;
        var widthScaleFactor = image.PointWidth / page.Width.Point;
        var scaleFactor = Math.Max(heightScaleFactor, widthScaleFactor);
        var height = image.PointHeight / scaleFactor;
        var width = image.PointWidth / scaleFactor;

        var xBorder = (page.Width.Point - width) / 2;
        var yBorder = (page.Height.Point - height) /2;

        XGraphics gfx = XGraphics.FromPdfPage(page);
        gfx.DrawImage(image, xBorder, yBorder, width, height);

        document.Save(outputPdfStream);
    }
}