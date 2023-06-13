using System;
using System.IO;

using IAG.Infrastructure.IntegrationTest.DataLayer.Migration.TestContext;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.DataLayer.Migration;

public class BetweenDbMigrationTest
{
    private const string PathOld = "Old";
    private const string PathNew = "New";

    [Fact]
    public void CopyDataBetweenSqliteDbsTest()
    {
        var testEntity = new TestEntity
        {
            Id = Guid.NewGuid(),
            TestNumber = 41,
            TestString = "World"
        };

        if (!Directory.Exists(PathOld))
            Directory.CreateDirectory(PathOld);
        if (!Directory.Exists(PathNew))
            Directory.CreateDirectory(PathNew);

        var dbFileOld = Path.Combine(PathOld, "old.db");
        if (File.Exists(dbFileOld))
            File.Delete(dbFileOld);
        var connectionOld = new SqliteConnection("DataSource=" + dbFileOld);
        var optionsBuilderOld = new DbContextOptionsBuilder().UseSqlite(connectionOld);
        var dbContextOld = new TestProjectDbContext(optionsBuilderOld.Options);
        dbContextOld.Database.EnsureCreated();
        dbContextOld.TestEntities.Add(testEntity);
        dbContextOld.SaveChanges();

        var dbFileNew = Path.Combine(PathNew, "new.db");
        if (File.Exists(dbFileNew))
            File.Delete(dbFileNew);
        var connectionNew = new SqliteConnection("DataSource=" + dbFileNew);
        var optionsBuilderNew = new DbContextOptionsBuilder().UseSqlite(connectionNew);
        var dbContextNew = new TestProjectDbContext(optionsBuilderNew.Options);
        dbContextNew.Database.EnsureCreated();

        var script = $@"attach database '{dbFileOld}' as oldDb; 
                            insert into main.TestEntity(Id, TestString, TestNumber)
                            select Id, TestString || '_New', TestNumber + 1 from oldDb.TestEntity";


#pragma warning disable EF1000 // Possible SQL injection vulnerability.
        dbContextNew.Database. ExecuteSqlRaw(script);
#pragma warning restore EF1000 // Possible SQL injection vulnerability.

        var copiedEntry = Assert.Single(dbContextNew.TestEntities);
        Assert.NotNull(copiedEntry);
        Assert.Equal(testEntity.Id, copiedEntry.Id);
        Assert.Equal(testEntity.TestNumber + 1, copiedEntry.TestNumber);
        Assert.Equal(testEntity.TestString + "_New", copiedEntry.TestString);
    }
}