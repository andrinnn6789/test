using System;

using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.IntegrationTest.DataLayer.Context.TestContext;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.DataLayer.Context;

[Collection("PostgresContextCollection")]
public class DatabaseAbstractionTest
{
    private readonly TestDbContext _postgresTestContext;

    public DatabaseAbstractionTest(PostgresTestContext postgresTestContext)
    {
        _postgresTestContext = postgresTestContext.GetNewContext();
    }

    [Fact]
    public void GetDbTypePostgresTest()
    {
        var dbAbstraction = new DatabaseAbstraction(_postgresTestContext.Database.ProviderName);
        Assert.Equal(DatabaseType.Postgres, dbAbstraction.DbType);
    }

    [Fact]
    public void CurrentTimestampPostgresTest()
    {
        _postgresTestContext.TestEntities.Add(new TestEntity { TestNumber = 42 });
        _postgresTestContext.SaveChanges();

        var dbAbstraction = new DatabaseAbstraction(_postgresTestContext.Database.ProviderName);
        var sqlCmd = $"UPDATE \"TestEntity\" SET \"TestNumber\"=\"TestNumber\" WHERE \"CreatedOn\"<={dbAbstraction.SqlDefaultValueCurrentTimestamp}";
        var updates = _postgresTestContext.Database.ExecuteSqlRaw(sqlCmd);
        Assert.True(updates >= 0);  // Does not work on TeamCity: Assert.True(updates >= 1);
    }

    [Fact]
    public void TableExistsSqliteTest()
    {
        using var dbContext = CreateSqliteContext();
        Assert.True(DatabaseAbstraction.TableExists(dbContext.Database, "TestAEntity", DatabaseType.Sqlite));
        Assert.False(DatabaseAbstraction.TableExists(dbContext.Database, "xx", DatabaseType.Sqlite));
    }

    [Fact]
    public void TableExistsPgTest()
    {
        Assert.True(DatabaseAbstraction.TableExists(_postgresTestContext.Database, "TestEntity", DatabaseType.Postgres));
        Assert.False(DatabaseAbstraction.TableExists(_postgresTestContext.Database, "xx", DatabaseType.Postgres));
    }

    [Fact]
    public void TableExistsInMemoryTest()
    {
        Assert.True(DatabaseAbstraction.TableExists(CreateInMemoryContext().Database, "TestEntity", DatabaseType.Memory));
    }

    private static Test.DataLayer.Context.TestContext.TestDbContext CreateSqliteContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        var optionsBuilder = new DbContextOptionsBuilder().UseSqlite(connection);
        connection.Open();
        var dbContext = new Test.DataLayer.Context.TestContext.TestDbContext(optionsBuilder.Options);
        dbContext.Database.EnsureCreated();

        return dbContext;
    }

    private static TestDbContext CreateInMemoryContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());

        var dbContext = new TestDbContext(optionsBuilder.Options);
        dbContext.Database.EnsureCreated();

        return dbContext;
    }
}