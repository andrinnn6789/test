using System;

using JetBrains.Annotations;

namespace IAG.ProcessEngine.Startup;

[UsedImplicitly]
public static class ServiceProviderHolder
{
    public static IServiceProvider ServiceProvider { get; set; }
}