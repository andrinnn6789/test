using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

using IAG.HostWindows;

namespace IAG.VinX.Schüwo.IntegrationTest;

[ExcludeFromCodeCoverage]
[SupportedOSPlatform("windows")]
public static class Program
{
    public static void Main(string[] args)
    {
        new WindowsHostMain().DoStart(args);
    }
}