using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Startup;

namespace IAG.ControlCenter.HostAzure;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args);
    }

    private static void CreateHostBuilder(string[] args) =>
        new ConsoleHostMain().DoStart(args);
}