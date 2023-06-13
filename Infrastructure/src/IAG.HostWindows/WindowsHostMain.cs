using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Versioning;
using System.ServiceProcess;

using IAG.Infrastructure.Startup;

namespace IAG.HostWindows;

[ExcludeFromCodeCoverage]
[SupportedOSPlatform("windows")]
public class WindowsHostMain: ConsoleHostMain
{
    public override void DoStart(string[] args)
    {
        var isService = true;
        if (args.Contains("--console"))
        {
            isService = false;
            args = args.Where(arg => arg != "--console").ToArray();
        }
        else if (Debugger.IsAttached)
        {
            isService = false;
        }

        var host = ServerHost.BuildHost(args, isService);

        if (isService)
        {
            var webHostService = new ServiceHost(host);
            ServiceBase.Run(webHostService);
        }
        else
        {
            RunHost(host);
        }
    }
}