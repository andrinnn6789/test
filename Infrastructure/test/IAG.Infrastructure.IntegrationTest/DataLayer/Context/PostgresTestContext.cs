using System;

using IAG.Infrastructure.IntegrationTest.DataLayer.Context.TestContext;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace IAG.Infrastructure.IntegrationTest.DataLayer.Context;

[UsedImplicitly]
public class PostgresTestContext: IDisposable
{
    private readonly DbContextOptionsBuilder<TestDbContext> _optionsBuilder;
    private readonly TestDbContext _context;

    public PostgresTestContext()
    {
        var dbName = Guid.NewGuid().ToString();
        var connStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = "postgres", 
            Port = 5432, 
            Username = "postgres", 
            Password = "iagiag", 
            Database = dbName
        };
        _optionsBuilder = new DbContextOptionsBuilder<TestDbContext>().UseNpgsql(connStringBuilder.ConnectionString);
        _context = new TestDbContext(_optionsBuilder.Options);
        _context.Database.EnsureCreated();
    }

    public TestDbContext GetNewContext()
    {
        return new(_optionsBuilder.Options);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}