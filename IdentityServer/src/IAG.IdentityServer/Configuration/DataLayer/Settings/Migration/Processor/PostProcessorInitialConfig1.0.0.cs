using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
public class PostProcessorInitialConfig : IPostProcessor
{
    [ExcludeFromCodeCoverage]
    public string ForVersion => "1.0.0";

    [ExcludeFromCodeCoverage]
    public DatabaseType[] ForDatabaseTypes => new[] { DatabaseType.Sqlite };

    public void Process(DatabaseFacade database, IServiceProvider serviceProvider)
    {
        var configDbContext = serviceProvider.GetRequiredService<IdentityDataStoreDbContext>();

        AddConfig(configDbContext, TokenGenerationConfig.ConfigName, new TokenGenerationConfig());
        AddConfig(configDbContext, AttackDetectionConfig.ConfigName, new AttackDetectionConfig());
        AddConfig(configDbContext, SmtpConfig.ConfigName, new SmtpConfig());
        AddConfig(configDbContext, CertificateManagerConfig.ConfigName, new CertificateManagerConfig());
        AddTokenGeneratorConfig(configDbContext);

        configDbContext.SaveChanges();
    }

    private static void AddTokenGeneratorConfig(IdentityDataStoreDbContext dbContext)
    {
        var existingConfig = dbContext.ConfigEntries.FirstOrDefault(c => c.Name == TokenGenerationConfig.ConfigName);
        if (existingConfig == null)
        {
            return;
        }

        var existingData = JsonConvert.DeserializeObject<TokenGenerationConfig>(existingConfig.Data);
        var data = new TokenGenerationConfig
        {
            ExpirationTime = existingData!.ExpirationTime
        };
        existingConfig.Data = JsonConvert.SerializeObject(data);
    }

    private static void AddConfig<T>(IdentityDataStoreDbContext dbContext, string name, T data) where T : new()
    {
        var dbConfig = new ConfigDb
        {
            Id = Guid.NewGuid(),
            CreatedBy = SchemaMigrator.MigratorUser,
            CreatedOn = DateTime.UtcNow,
            ChangedBy = SchemaMigrator.MigratorUser,
            ChangedOn = DateTime.UtcNow,
            Name = name,
            Data = JsonConvert.SerializeObject(data)
        };

        dbContext.ConfigEntries.Add(dbConfig);
    }
}