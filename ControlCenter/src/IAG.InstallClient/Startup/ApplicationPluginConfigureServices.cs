using IAG.Infrastructure.DI;
using IAG.InstallClient.Authorization;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Composition;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace IAG.InstallClient.Startup;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Export(typeof(IPluginConfigureServices))]
public class ApplicationPluginConfigureServices : IPluginConfigureServices
{
    public void PluginConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        // Needs to be overwritten after AddIagServerAuthentication in Startup
        services.Replace(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, CookieJwtBearerPostConfigureOptions>());
    }
}