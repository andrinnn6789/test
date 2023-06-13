using System;
using System.IO;
using System.Linq;
using System.Net.Http;

using IAG.Infrastructure.Settings;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IAG.Infrastructure.TestHelper.Startup;

public class TestServerEnvironment : IDisposable
{
    private static readonly string TempDirPlaceholder = "{{TempTestDir}}";

    private Guid TestRunId { get; }
    private string TemporaryDirectory { get; }

    private IHost Server { get; }

    public TestServerEnvironment()
    {
        TestRunId = Guid.NewGuid();
        TemporaryDirectory = $".\\TempDir_{TestRunId}";
        Directory.CreateDirectory(TemporaryDirectory);

        var hostBuilder = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost
                    .UseTestServer()
                    .UseEnvironment("Test")
                    .ConfigureLogging(factory => { factory.AddDebug(); })
                    .UseStartup<Infrastructure.Startup.Startup>()
                    .UseConfiguration(Infrastructure.Startup.Startup.BuildConfig());
            });

        var configRoot = Infrastructure.Startup.Startup.BuildConfig();
        foreach (var config in configRoot.GetAllConfigValues().Where(c => c.Value.Contains(TempDirPlaceholder)))
        {
            configRoot[config.Key] = config.Value.Replace(TempDirPlaceholder, TemporaryDirectory);
        }

        // Delete "ref" directory which will confuse plugin loader...
        var refDir = Path.Combine(Directory.GetCurrentDirectory(), "ref");
        if (Directory.Exists(refDir))
        {
            Directory.Delete(refDir, true);
        }

        Server = hostBuilder.Start();
    }

    public HttpClient NewClient() => Server.GetTestClient();

    public IServiceProvider GetServices() => Server.Services;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Directory.Delete(TemporaryDirectory, true);
            Server.Dispose();
        }
    }
}