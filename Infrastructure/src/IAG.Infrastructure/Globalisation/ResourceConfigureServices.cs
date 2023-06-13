using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Globalisation.ResourceProvider;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;

namespace IAG.Infrastructure.Globalisation;

[ExcludeFromCodeCoverage]   // is covered in the integration tests
[UsedImplicitly]
[Export(typeof(IConfigureServices))]
public class ResourceConfigureServices : IConfigureServices
{
    private const string Resources = "Resources";

    public void ConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        var dbOption = services.GetDbOption(Resources, $"{Resources}.db");
        services.AddSingleton<IStringLocalizerFactoryReloadable, DbStringLocalizerFactory>();
        services.AddSingleton<IStringLocalizerFactory, DbStringLocalizerFactory>();
        services.AddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
        services.AddDbContext<ResourceContext>(dbOption);
        services.AddScoped<IResourceCollector, ResourceCollector>();
        services.AddLocalization();
        var builder = services.BuildServiceProviderForExplicitUser(SchemaMigrator.MigratorUser);
        using var scope = builder.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ResourceContext>();

        new SchemaMigrator(scope.ServiceProvider).DoMigration(context);

        if (!context.Cultures.Any())
        {
            context.Cultures.AddRange(
                new Culture { Name = "de" },
                new Culture { Name = "de-CH" },
                new Culture { Name = "fr" },
                new Culture { Name = "fr-CH" },
                new Culture { Name = "en" });
            context.SaveChanges();
        }
        var supportedCultures = context.Cultures.Select(culture => new CultureInfo(culture.Name)).ToList();
        if (supportedCultures.Count > 0)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
        }
    }
}