using System.Diagnostics.CodeAnalysis;
using System.Linq;

using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IAG.Infrastructure.Startup.Extensions;

[ExcludeFromCodeCoverage]
public static class ClaimsPublishExtensions
{
    public static IServiceCollection AddClaimsPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        if (AuthenticationExtensions.IsAuthenticationDisabled(configuration))
        {
            // don't publish if authentication is disabled
            return services;
        }

        if (!services.Any(s => s.ServiceType == typeof(IClaimPublisher)))
        {
            services.AddScoped<IClaimPublisher, RemoteClaimPublisher>();
        }
                
        return services;
    }

    public static IApplicationBuilder PublishClaims(this IApplicationBuilder appBuilder, IConfiguration configuration)
    {
        if (AuthenticationExtensions.IsAuthenticationDisabled(configuration))
        {
            // don't publish if authentication is disabled
            return appBuilder;
        }

        var claimCollector = appBuilder.ApplicationServices.GetRequiredService<IClaimCollector>();
        claimCollector.CollectAndUpdateAsync().Wait();

        return appBuilder;
    }
}