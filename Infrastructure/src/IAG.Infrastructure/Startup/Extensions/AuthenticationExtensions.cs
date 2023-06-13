using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;
using IAG.Infrastructure.Swagger;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;

namespace IAG.Infrastructure.Startup.Extensions;

public enum AuthenticationMode
{
    All,
    Remote,
    Off
}

[ExcludeFromCodeCoverage]
public static class AuthenticationExtensions
{
    public static string Issuer { get; private set; }

    public static IServiceCollection AddIagServerAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        if (IsAuthenticationDisabled(configuration))
            return services;

        Issuer = (configuration.GetChildren().Any(c => c.Key == "urls")
                ? configuration["urls"].Split(';')[0].Replace("*", Dns.GetHostName())
                : Dns.GetHostName())
            .ToLower();
        TokenValidationParameters tokenValidationParameters;
        var authority = configuration["Authentication:IdentityServer"];
        bool requireHttpsMetadata = !Debugger.IsAttached;
        if (string.IsNullOrWhiteSpace(authority))
        {
            var tokenHandler = services.BuildServiceProvider().GetService<ITokenHandler>();
            if (tokenHandler == null)
                throw new System.Exception("Cannot initialize authentication without a tokenhandler");
            tokenValidationParameters = tokenHandler.GetTokenValidationParameters(Issuer);
            requireHttpsMetadata = tokenHandler.RequireHttpsMetadata ?? requireHttpsMetadata;
        }
        else
        {
            tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidIssuer = authority,
                ValidIssuers = new[] { authority },
                ValidateAudience = false,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };
        }
        tokenValidationParameters.ValidIssuer = Issuer;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                x.Authority = authority;
                x.SaveToken = true;
                x.RequireHttpsMetadata = requireHttpsMetadata;
                x.TokenValidationParameters = tokenValidationParameters;
            });

        return services;
    }

    public static IServiceCollection AddIagServerAuthorization(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IAuthorizationPolicyProvider, ClaimAuthorizationPolicyProvider>();

        switch (GetAuthenticationMode(configuration))
        {
            case AuthenticationMode.Off:
                services.AddSingleton<IAuthorizationHandler, NoAuthorizationHandler>();
                break;
            case AuthenticationMode.Remote:
                services.AddSingleton<IAuthorizationHandler, RemoteOnlyClaimAuthorizationHandler>();
                break;
            default:
                services.AddSingleton<IAuthorizationHandler, ClaimAuthorizationHandler>();
                break;
        }

        return services;
    }

    public static IMvcBuilder AddIagServerAuthorization(this IMvcBuilder mvcBuilder, IConfiguration configuration)
    {
        if (GetAuthenticationMode(configuration) != AuthenticationMode.All) 
            return mvcBuilder;

        // in mode "AuthenticationMode.Remote" the authenticated user is checked in the RemoteOnlyClaimAuthorizationHandler
        mvcBuilder.AddMvcOptions(options =>
        {
            var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });

        return mvcBuilder;
    }

    public static IApplicationBuilder UseIagServerAuthorization(this IApplicationBuilder appBuilder,
        IConfiguration configuration)
    {
        if (IsAuthenticationDisabled(configuration))
            return appBuilder;

        appBuilder.UseAuthentication();

        if (!IsSwaggerAuthenticationDisabled(configuration))
            appBuilder.UseMiddleware<SwaggerAuthMiddleware>();

        return appBuilder;
    }

    public static bool IsAuthenticationDisabled(IConfiguration configuration)
    {
        return GetAuthenticationMode(configuration) == AuthenticationMode.Off;
    }

    private static AuthenticationMode GetAuthenticationMode(IConfiguration configuration)
    {
        var modeConfig = configuration["Authentication:Mode"];
        if (string.IsNullOrEmpty(modeConfig))
        {
            return AuthenticationMode.All;
        }

        return Enum.TryParse<AuthenticationMode>(modeConfig, out var mode) ? mode : AuthenticationMode.All;
    }

    private static bool IsSwaggerAuthenticationDisabled(IConfiguration configuration)
    {
        return bool.TrueString.Equals(configuration["Authentication:DisabledForSwagger"],
            StringComparison.InvariantCultureIgnoreCase);
    }
}