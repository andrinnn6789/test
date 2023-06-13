using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;

using SkiaSharp;

namespace IAG.Infrastructure.Pdf;

public class PdfImageExtractorPngRenderer : IPdfImageExtractor
{
    private static readonly string WorkingFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    private static readonly string PdfToPngPathWindows = Path.Combine(AppContext.BaseDirectory, "pdftopng.exe");
    private static readonly string PdfToPngPathLinux = Path.Combine(AppContext.BaseDirectory, "pdftopng");

    static PdfImageExtractorPngRenderer()
    {
        AppDomain.CurrentDomain.ProcessExit += Cleanup;
    }

    // Will return one PNG per page with the help of pdftopng.exe
    // A tool from https://www.xpdfreader.com/download.html 
    public IEnumerable<SKBitmap> GetImages(Stream pdfStream)
    {
        var toolPath = GetToolPath();
        Directory.CreateDirectory(WorkingFolder);

        var conversionId = Guid.NewGuid();
        var pdfFilePath = Path.Combine(WorkingFolder, $"{conversionId}.pdf");

        try
        {
            using var fileStream = File.Create(pdfFilePath);
            pdfStream.Seek(0, SeekOrigin.Begin);
            pdfStream.CopyTo(fileStream);
            fileStream.Close();

            Process cmd = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = toolPath,
                    Arguments = $"-alpha \"{pdfFilePath}\" \"{Path.Combine(WorkingFolder, conversionId.ToString())}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false

                }
            };
            cmd.Start();
            cmd.WaitForExit();

            var result = new List<SKBitmap>();
            foreach (var pngFilePath in Directory.GetFiles(WorkingFolder, $"{conversionId}*.png"))
            {
                using var pngStream = File.OpenRead(pngFilePath);
                result.Add(SKBitmap.Decode(pngStream));
                pngStream.Close();
                File.Delete(pngFilePath);
            }

            return result;
        }
        finally
        {
            if (File.Exists(pdfFilePath))
            {
                File.Delete(pdfFilePath);
            }
        }
    }

    [ExcludeFromCodeCoverage(Justification = "RuntimeInformation cannot be mocked...")]
    private static string GetToolPath()
    {
        string toolPath;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            toolPath = PdfToPngPathWindows;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            toolPath = PdfToPngPathLinux;
        }
        else
        {
            throw new NotSupportedException($"The platform {RuntimeInformation.OSDescription} is not supported");
        }

        return toolPath;
    }

    [ExcludeFromCodeCoverage(Justification = "Cannot be tested...")]
    private static void Cleanup(object _, EventArgs _1)
    {
        try
        {
            if (Directory.Exists(WorkingFolder))
            {
                Directory.Delete(WorkingFolder);
            }
        }
        catch (System.Exception)
        {
            // ignored, just don't bother the application shutdown with an exception
        }
    }
}