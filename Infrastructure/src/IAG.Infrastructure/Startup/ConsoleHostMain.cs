using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace IAG.Infrastructure.Startup;

[ExcludeFromCodeCoverage]
public class ConsoleHostMain
{
    public virtual void DoStart(string[] args)
    {
        var host = ServerHost.BuildHost(args, false);
        RunHost(host);
    }

    protected static void RunHost(IWebHost host)
    {
        var serviceLifetimes = host.Services.GetServices<IServiceLifetime>().ToArray();
        foreach (var serviceLifetime in serviceLifetimes)
        {
            serviceLifetime.OnStart();
        }

        host.Run();
        foreach (var serviceLifetime in serviceLifetimes)
        {
            serviceLifetime.OnStop();
        }
    }
}