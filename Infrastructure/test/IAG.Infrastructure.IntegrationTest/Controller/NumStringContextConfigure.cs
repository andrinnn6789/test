using System.Composition;

using IAG.Infrastructure.DI;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IAG.Infrastructure.IntegrationTest.Controller;

[UsedImplicitly]
[Export(typeof(IPluginConfigureServices))]
public class NumStringContextConfigure : IPluginConfigureServices
{
    public void PluginConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        services.AddDbContext<NumStringContext>(opt => opt.UseInMemoryDatabase("numstring"));
    }
}