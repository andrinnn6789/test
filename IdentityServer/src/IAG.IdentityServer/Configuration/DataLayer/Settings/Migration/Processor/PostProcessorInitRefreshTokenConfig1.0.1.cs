using System;
using System.Diagnostics.CodeAnalysis;

using IAG.IdentityServer.Configuration.Model.Config;
using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace IAG.IdentityServer.Configuration.DataLayer.Settings.Migration.Processor;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class PostProcessorInitRefreshTokenConfig : IPostProcessor
{
    [ExcludeFromCodeCoverage]
    public string ForVersion => "1.0.1";

    [ExcludeFromCodeCoverage]
    public DatabaseType[] ForDatabaseTypes => new[] { DatabaseType.Sqlite };

    public void Process(DatabaseFacade database, IServiceProvider serviceProvider)
    {
        var configDbContext = serviceProvider.GetRequiredService<IdentityDataStoreDbContext>();

        var dbConfig = new ConfigDb
        {
            Id = Guid.NewGuid(),
            CreatedBy = SchemaMigrator.MigratorUser,
            CreatedOn = DateTime.UtcNow,
            ChangedBy = SchemaMigrator.MigratorUser,
            ChangedOn = DateTime.UtcNow,
            Name = RefreshTokenConfig.ConfigName,
            Data = JsonConvert.SerializeObject(new RefreshTokenConfig())
        };
        configDbContext.ConfigEntries.Add(dbConfig);

        configDbContext.SaveChanges();
    }
}