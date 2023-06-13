using System;
using System.Composition;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DI;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IAG.Infrastructure.Logging;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Export(typeof(IConfigure))]
public class LogConfigure : IConfigure
{
    private static ILoggerFactory _factory;

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        _factory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
        AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = Debugger.IsAttached;
    }

    private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var colorOri = Console.ForegroundColor;
        try
        {
            var exceptionThrown = (System.Exception)e.ExceptionObject;
            _factory.CreateLogger("Global").LogError(exceptionThrown, "Global error");
        }
        catch (System.Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            while (ex != null)
            {
                Console.WriteLine(ex.Message);
                ex = ex.InnerException;
            }
        }
        finally
        {
            Console.ForegroundColor = colorOri;
        }
    }
}