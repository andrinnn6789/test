using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

using IAG.Infrastructure.Globalisation.ResourceProvider;
using IAG.Infrastructure.Logging;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace IAG.Infrastructure.Startup;

[ExcludeFromCodeCoverage]
public static class ServerHost
{
    public static IWebHost BuildHost(string[] args, bool isService)
    {
        var pathToContentRoot = Directory.GetCurrentDirectory();
        if (isService)
        {
            var processModule = Process.GetCurrentProcess().MainModule;
            if (processModule != null)
            {
                var pathToExe = processModule.FileName;
                pathToContentRoot = Path.GetDirectoryName(pathToExe) ?? string.Empty;
            }

            Directory.SetCurrentDirectory(pathToContentRoot);
        }

        var config = Startup.BuildConfig(args);

        var host = WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging((_, logging) => { LogConfigureServices.ConfigLogging(logging, config); })
            .UseStartup<Startup>()
            .UseContentRoot(pathToContentRoot)
            .UseSetting(WebHostDefaults.ApplicationKey, Assembly.GetEntryAssembly()?.FullName)
            .UseConfiguration(config)
            .Build();

        host.Services.GetService<IResourceCollector>()?.CollectAndUpdate();

        return host;
    }
}