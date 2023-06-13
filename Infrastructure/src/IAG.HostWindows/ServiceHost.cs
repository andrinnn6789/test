using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Versioning;

using IAG.Infrastructure.Startup;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IAG.HostWindows;

[ExcludeFromCodeCoverage]
[SupportedOSPlatform("windows")]
internal class ServiceHost : WebHostService
{
    private readonly ILogger _logger;
    private readonly IEnumerable<IServiceLifetime> _serviceLifetimes;

    public ServiceHost(IWebHost host) : base(host)
    {
        _logger = host.Services.GetService<ILogger<ServiceHost>>();
        _serviceLifetimes = host.Services.GetServices<IServiceLifetime>().ToArray();
    }

    protected override void OnStarting(string[] args)
    {
        _logger.LogDebug("Service is starting...");
        foreach (var serviceLifetime in _serviceLifetimes)
        {
            serviceLifetime.OnStart();
        }

        base.OnStarting(args);
    }

    protected override void OnStarted()
    {
        _logger.LogDebug("Service started");
        base.OnStarted();
    }

    protected override void OnStopping()
    {
        _logger.LogDebug("Service is stopping...");
        foreach (var serviceLifetime in _serviceLifetimes)
        {
            serviceLifetime.OnStop();
        }

        base.OnStopping();
    }

    protected override void OnStopped()
    {
        _logger.LogDebug("Service stopped");
        base.OnStarted();
    }
}