using System;
using System.Composition;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.BarcodeScanner;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Ftp;
using IAG.Infrastructure.Influx;
using IAG.Infrastructure.Pdf;
using IAG.Infrastructure.Settings;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IAG.Infrastructure.Startup;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Export(typeof(IConfigureServices))]
public class ApplicationConfigureServices : IConfigureServices
{
    public void ConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        services.AddScoped<IBarcodeScanner, BarcodeScannerZXing>();
        services.AddScoped<IImageToPdfConverter, ImageToPdfConverterPdfSharp>();
        services.AddScoped<IPdfImageExtractor, PdfImageExtractorPngRenderer>();
        services.AddScoped<IPdfMerger, PdfMergerPdfSharp>();
        services.AddScoped<IPdfPageHandler, PdfPageHandlerPdfSharp>();
        services.AddScoped<IInfluxClient, InfluxClient>();
        services.AddScoped(_ => new InfluxConfig
        {
            Url = Environment.GetEnvironmentVariable(SettingsConst.InfluxServer),
            Token = Environment.GetEnvironmentVariable(SettingsConst.InfluxToken)
        });
        services.AddScoped<ISecureFtpClient, SecureFtpClient>();
    }
}