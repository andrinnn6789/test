using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Startup;
using IAG.ProcessEngine.Execution;

using Microsoft.Extensions.Logging;

namespace IAG.ProcessEngine.Startup;

[ExcludeFromCodeCoverage]
internal class ServiceLifetime : IServiceLifetime
{
    private readonly IJobExecuter _executer;
    private readonly IScheduler _scheduler;
    private readonly IMonitor _monitor;
    private readonly ILogger _logger;

    public ServiceLifetime(IJobExecuter executer, IScheduler scheduler, IMonitor monitor, ILogger<ServiceLifetime> logger)
    {
        _executer = executer;
        _scheduler = scheduler;
        _monitor = monitor;
        _logger = logger;
    }

    public void OnStart()
    {
        _executer.Start();
        _scheduler.Start();
        _monitor.Start();
        _logger.LogDebug("PE-services started");
    }

    public void OnStop()
    {
        _monitor.Stop();
        _scheduler.Stop();
        _executer.Stop();
        _logger.LogDebug("PE-Services stopped");
    }
}