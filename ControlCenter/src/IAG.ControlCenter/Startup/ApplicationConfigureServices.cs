using System.Composition;
using System.Diagnostics.CodeAnalysis;

using IAG.ControlCenter.Distribution.BusinessLayer;
using IAG.ControlCenter.Distribution.DataLayer.Context;
using IAG.ControlCenter.Mobile.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.DI;

using JetBrains.Annotations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IAG.ControlCenter.Startup;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Export(typeof(IConfigureServices))]
public class ApplicationConfigureServices : IConfigureServices
{
    private const string ControlCenter = "ControlCenter";

    public void ConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        var config = services.BuildServiceProvider().GetRequiredService<IConfigurationRoot>();

        if (bool.TryParse(config[$"{ControlCenter}:Disabled"], out var disabled) && disabled)
            return;
            
        services.AddDbContext<MobileDbContext>(services.GetDbOption(ControlCenter, $"{ControlCenter}.db"));
        services.AddDbContext<DistributionDbContext>(services.GetDbOption(ControlCenter, $"{ControlCenter}.db"));

        services.AddScoped<IProductAdminHandler, ProductAdminHandler>();
        services.AddScoped<ICustomerAdminHandler, CustomerAdminHandler>();
        services.AddScoped<ICustomerHandler, CustomerHandler>();
        services.AddScoped<ILinkAdminHandler, LinkAdminHandler>();
            
        using var scope = services.BuildServiceProviderForExplicitUser(SchemaMigrator.MigratorUser).CreateScope();
        var migrator = new SchemaMigrator(scope.ServiceProvider);
        migrator.DoMigration(scope.ServiceProvider.GetRequiredService<MobileDbContext>());
        migrator.DoMigration(scope.ServiceProvider.GetRequiredService<DistributionDbContext>());
    }
}