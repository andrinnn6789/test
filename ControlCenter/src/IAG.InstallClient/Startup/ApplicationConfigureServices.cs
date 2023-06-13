using System;
using System.Composition;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DI;
using IAG.InstallClient.BusinessLogic;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IAG.InstallClient.Startup;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Export(typeof(IConfigureServices))]
public class ApplicationConfigureServices : IConfigureServices
{
    public void ConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        services.AddDistributedMemoryCache();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
        });

        services.AddControllersWithViews();
        services.AddScoped<ICustomerManager, CustomerManager>();
        services.AddScoped<IInstallationManager, InstallationManager>();
        services.AddScoped<IReleaseManager, ReleaseManager>();
        services.AddScoped<IServiceManager, ServiceManager>();
        services.AddScoped<IInventoryManager, InventoryManager>();
        services.AddScoped<ILinkListManager, LinkListManager>();
        services.AddScoped<ILoginManager, LoginManager>();
    }
}