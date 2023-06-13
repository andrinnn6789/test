using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IAG.Infrastructure.DI;

public interface IPluginConfigureServices
{
    void PluginConfigureServices(IServiceCollection services, IHostEnvironment env);
}