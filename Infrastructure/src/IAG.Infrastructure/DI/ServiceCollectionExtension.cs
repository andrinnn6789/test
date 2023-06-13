using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.Settings;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IAG.Infrastructure.DI;

[ExcludeFromCodeCoverage]   // is covered in the integration tests
[UsedImplicitly]
public static class ServiceCollectionExtension
{
    private static readonly SettingsFinder SettingsFinder = new();

    public static Action<DbContextOptionsBuilder> GetDbOption(this IServiceCollection services, string module, string defaultName)
    {
        var config = services.BuildServiceProvider().GetRequiredService<IConfigurationRoot>();
        var connectionString = config[module + ":connectionString"];
        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = $"Data Source = {Path.Combine(SettingsFinder.GetSettingsPath(), defaultName)}";
        bool.TryParse(config[module + ":enableSensitiveDataLogging"], out var logSensitive);

        return opt =>
        {
            opt.EnableSensitiveDataLogging(logSensitive);
            var providerName = config[module + ":providerName"];
            switch (DatabaseAbstraction.GetDbType(providerName))
            {
                case DatabaseType.Postgres:
                    opt.UseNpgsql(connectionString);
                    break;
                case DatabaseType.Sqlite:
                    opt.UseSqlite(connectionString);
                    break;
                case DatabaseType.MsSql:
                    opt.UseSqlServer(connectionString);
                    break;
                case DatabaseType.Memory:
                    opt.UseInMemoryDatabase(Guid.NewGuid().ToString());
                    break;
                case DatabaseType.MemoryShared:
                    opt.UseInMemoryDatabase(module);
                    break;
                default:
                    throw new System.Exception($"Unknown DB-provider received: {providerName}");
            }
        };
    }

    public static IServiceProvider BuildServiceProviderForExplicitUser(this IServiceCollection services, string userName, Guid? tenant = null)
    {
        var userContextDescriptor = ServiceDescriptor.Scoped<IUserContext>(_ => new ExplicitUserContext(userName, tenant));
        services.Add(userContextDescriptor);
        var builder = services.BuildServiceProvider();
        services.Remove(userContextDescriptor);

        return builder;
    }

    public static IServiceScope BuildScopeForExplicitUser(this IServiceProvider serviceProvider, string userName, Guid? tenant = null)
    {
        var scope = serviceProvider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IUserContext>().SetExplicitUser(userName, tenant);

        return scope;
    }
}