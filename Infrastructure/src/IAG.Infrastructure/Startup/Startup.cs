using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using IAG.Infrastructure.DI;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Authorization.ClaimProvider;
using IAG.Infrastructure.Middleware;
using IAG.Infrastructure.Settings;
using IAG.Infrastructure.Startup.Extensions;
using IAG.Infrastructure.Startup.Extensions.Swagger;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace IAG.Infrastructure.Startup;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class Startup
{
    private readonly PluginLoader _pluginLoader;
    private static IConfigurationRoot _configurationRoot;

    public Startup(IHostEnvironment env)
    {
        _pluginLoader = new PluginLoader();
        HostingEnvironment = env;
    }

    private IHostEnvironment HostingEnvironment { get; }

    /// <summary>
    /// only for unit- and integration tests
    /// </summary>
    /// <returns>new configuration or static member from previous call</returns>
    public static IConfigurationRoot BuildConfig()
    {
        return BuildConfig(Array.Empty<string>());
    }

    /// <summary>
    /// only for unit- and integration tests
    /// </summary>
    public static void CleanConfig()
    {
        _configurationRoot = null;
    }

    public static IConfigurationRoot BuildConfig(string[] args)
    {
        if (_configurationRoot != null)
        {
            return _configurationRoot;
        }

        var config = new ConfigurationBuilder()
            .AddCommandLine(args)
            .AddEnvironmentVariables().Build();

        var appSettings = config.AsEnumerable().FirstOrDefault(s => s.Key == "appsettings").Value;
        if (string.IsNullOrWhiteSpace(appSettings))
        {
            appSettings = new SettingsFinder().GetSettingsFilePath();
        }

        var builder = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile(appSettings)
            .AddEnvironmentVariables();

        var logSettings = new SettingsFinder().GetSettingsFilePath("logsettings.json");
        if (File.Exists(logSettings))
        {
            builder.AddJsonFile(logSettings);
        }

        _configurationRoot = builder.Build();
        return _configurationRoot;
    }

    [UsedImplicitly]
    public void ConfigureServices(IServiceCollection services)
    {
        if (_configurationRoot == null)
        {
            BuildConfig();
        }

        // ReSharper disable once AssignNullToNotNullAttribute
        services.AddSingleton(_configurationRoot);
        services.AddHttpContextAccessor();
        services.AddScoped<IPluginLoader, PluginLoader>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IClaimCollector, ClaimCollector>();
        services.AddLogging();
        services.AddIagSwaggerDocs(_pluginLoader);

        foreach (var plugin in _pluginLoader.GetExports<IConfigureServices>())
            plugin.ConfigureServices(services, HostingEnvironment);

        services.AddIagServerAuthentication(_configurationRoot);
        services.AddIagServerAuthorization(_configurationRoot);

        foreach (var plugin in _pluginLoader.GetExports<IPluginConfigureServices>())
            plugin.PluginConfigureServices(services, HostingEnvironment);

        // Needs to be called after plugin configuration
        // --> If IdentityServer is on same instance, there is no need for remote claims publishing
        services.AddClaimsPublisher(_configurationRoot);

        services.AddIagMvc(_configurationRoot, _pluginLoader);
        
        services.AddControllersWithViews();
    }

    [UsedImplicitly]
    public void Configure(IApplicationBuilder app)
    {
        app.UseMiddleware<LoggingMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseIagServerAuthorization(_configurationRoot);
        app.UseIagSwagger();

        app.UseRouting();
        foreach (var plugin in _pluginLoader.GetExports<IConfigure>())
        {
            plugin.Configure(app, HostingEnvironment);
        }

        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
        );

        app.UseIagMvc();

        app.PublishClaims(_configurationRoot);
        app.DoSeedImport();
        
        var assembly = GetType().Assembly;
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new EmbeddedFileProvider(assembly, $"{assembly.GetName().Name}.wwwroot")
        });
    }
}