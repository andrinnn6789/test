using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IAG.Infrastructure.DI;

public interface IConfigureServices
{
    void ConfigureServices(IServiceCollection services, IHostEnvironment env);
}