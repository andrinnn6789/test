using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.ProcessEngine.Store;

using JetBrains.Annotations;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace IAG.ProcessEngine.DataLayer.Settings.Context.Migration.Processor;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class PostProcessorMigrateOldConfig : IPostProcessor
{
    public string OldDbFileName => "ProcessEngine.db";
    public string MigrationPostfix => ".migratedConfig";

    [ExcludeFromCodeCoverage]
    public string ForVersion => "1.0.0";

    [ExcludeFromCodeCoverage]
    public DatabaseType[] ForDatabaseTypes => new[] { DatabaseType.Sqlite };
    
    public void Process(DatabaseFacade database, IServiceProvider serviceProvider)
    {
        var oldDbs = Directory.EnumerateFiles(Environment.CurrentDirectory, $"{OldDbFileName}*", SearchOption.AllDirectories)
            .Where(f => !f.Substring(OldDbFileName.Length).Contains(MigrationPostfix)).ToList();

        if (oldDbs.Count == 0)
        {
            return;
        }

        var oldDb = oldDbs.First();

#pragma warning disable EF1000 // Possible SQL injection vulnerability.
        database. ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS IdLookup (
                    Id BLOB NOT NULL,
                    Name TEXT NOT NULL);
                DELETE FROM IdLookup;");
        var jobCatalogue = serviceProvider.GetRequiredService<IJobCatalogue>();
        var paramId = new SqliteParameter("$id", SqliteType.Blob);
        var paramName = new SqliteParameter("$name", SqliteType.Text);
        foreach (var jobMeta in jobCatalogue.Catalogue)
        {
            paramId.Value = jobMeta.TemplateId;
            paramName.Value = jobMeta.PluginName;
            database. ExecuteSqlRaw(@"
                    INSERT INTO IdLookup (Id, Name) values ($id, $name);", paramId, paramName);
        }

        var cmd = $@"
                BEGIN TRANSACTION;               
                ATTACH DATABASE '{Path.GetRelativePath(Environment.CurrentDirectory, oldDb)}' as oldDb; 

                INSERT INTO ConfigProcessEngine(Id, Name, Data, Disabled, CreatedOn, ChangedOn) 
                SELECT IdLookup.Id, Name, Data, 0, strftime('%Y-%m-%d %H:%M:%f', 'now'), strftime('%Y-%m-%d %H:%M:%f', 'now')  
                FROM oldDb.Config 
                JOIN IdLookup on Name = oldDb.Config.Id;

                COMMIT;
                DROP TABLE IdLookup;
                DETACH DATABASE oldDb;";

        database. ExecuteSqlRaw(cmd);
#pragma warning restore EF1000 // Possible SQL injection vulnerability.

        var oldDbFileAfterMigration = oldDb + MigrationPostfix;
        File.Move(oldDb, oldDbFileAfterMigration);
    }
}