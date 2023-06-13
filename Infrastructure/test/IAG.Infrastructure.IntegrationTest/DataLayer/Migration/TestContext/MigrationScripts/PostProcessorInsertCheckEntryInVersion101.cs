using System;

using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace IAG.Infrastructure.IntegrationTest.DataLayer.Migration.TestContext.MigrationScripts;

[UsedImplicitly]
public class PostProcessorInsertCheckEntryInVersion101: IPostProcessor
{
    public const int UsedTestNumber = 17;

    public string ForVersion => "1.0.1";
    public DatabaseType[] ForDatabaseTypes => new[] { DatabaseType.Sqlite };

    public void Process(DatabaseFacade db, IServiceProvider serviceProvider)
    {
        var sqlCmd = $"INSERT INTO TestEntity (Id, TestNumber, TestString) VALUES ('{Guid.NewGuid()}', {UsedTestNumber}, 'TestEntryByPostProcessor!')";
        db. ExecuteSqlRaw(sqlCmd);
    }
}