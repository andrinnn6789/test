using System.Composition;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DI;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IAG.Infrastructure.Test.DI.TestData;

[Export(typeof(IConfigure))]
[Export(typeof(IPluginConfigureServices))]
[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class TestPlugin : IPluginConfigureServices, IConfigure
{
    public int CallbackFromConfigure { get; private set; }

    public int CallbackFromStartup { get; private set; }

    public void PluginConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        CallbackFromConfigure++;
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        CallbackFromStartup++;
    }
}