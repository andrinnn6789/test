using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

using IAG.HostWindows;

namespace IAG.VinX.Kouski.Test;

[ExcludeFromCodeCoverage]
[SupportedOSPlatform("windows")]
public static class Program
{
    public static void Main(string[] args)
    {
        new WindowsHostMain().DoStart(args);
    }
}