using System;
using System.Threading.Tasks;

using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.Test.DataLayer.Context.TestContext;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.Infrastructure.Test.DataLayer.Context;

public class DatabaseAbstractionTest
{
    [Fact]
    public void UnknownProviderTest()
    {
        Assert.Throws<System.Exception>(() => DatabaseAbstraction.GetDbType("hallo"));
    }

    [Fact]
    public void GetDbParameterTest()
    {
        Assert.Throws<System.Exception>(() => DatabaseAbstraction.GetDbParameter("hallo", "xx", "xx"));
        Assert.NotNull(DatabaseAbstraction.GetDbParameter("sqlite", "xx", "xx"));
    }

    [Fact]
    public void GetDbTypeSqliteTest()
    {
        using var dbContext = CreateSqliteContext();
        var dbAbstraction = new DatabaseAbstraction(dbContext.Database.ProviderName);

        Assert.Equal(DatabaseType.Sqlite, dbAbstraction.DbType);
    }

    [Fact]
    public void GetDbTypeMemoryTest()
    {
        using var dbContext = CreateInMemoryContext();
        var dbAbstraction = new DatabaseAbstraction(dbContext.Database.ProviderName);

        Assert.Equal(DatabaseType.Memory, dbAbstraction.DbType);
    }

    [Fact]
    public void GetDbTypeMsSqlTest()
    {
        var dbAbstraction = new DatabaseAbstraction("blabalsqlServerwhatever");
        Assert.Equal(DatabaseType.MsSql, dbAbstraction.DbType);
    }

    [Fact]
    public async Task CurrentTimestampSqliteTest()
    {
        await using var dbContext = CreateSqliteContext();
        dbContext.TestAEntities.Add(new TestAEntity() {TestNumber = 42});
        await dbContext.SaveChangesAsync();
        await Task.Delay(5);

        var dbAbstraction = new DatabaseAbstraction(dbContext.Database.ProviderName);
        var sqlCmd = $"UPDATE TestAEntity SET TestNumber=TestNumber WHERE CreatedOn<={dbAbstraction.SqlDefaultValueCurrentTimestamp}";

        var updates = dbContext.Database.ExecuteSqlRaw(sqlCmd);

        Assert.Equal(1, updates);
    }

    [Fact]
    public void CurrentTimestampMsSqlTest()
    {
        using var dbContext = CreateInMemoryContext();
        var dbAbstraction = new DatabaseAbstraction(dbContext.Database.ProviderName);

        // "GETUTCDATE()" is not supported by in memory db...
        Assert.NotNull(dbAbstraction.SqlDefaultValueCurrentTimestamp);
        Assert.NotEmpty(dbAbstraction.SqlDefaultValueCurrentTimestamp);
    }

    private static TestDbContext CreateSqliteContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        var optionsBuilder = new DbContextOptionsBuilder().UseSqlite(connection);
        connection.Open();

        var dbContext = new TestDbContext(optionsBuilder.Options);
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