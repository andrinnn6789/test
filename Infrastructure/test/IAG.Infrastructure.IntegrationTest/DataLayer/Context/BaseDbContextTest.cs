using IAG.Infrastructure.IntegrationTest.DataLayer.Context.TestContext;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.DataLayer.Context;

[Collection("PostgresContextCollection")]
public class BaseDbContextTest
{
    private readonly TestDbContext _postgresTestContext;

    public BaseDbContextTest(PostgresTestContext postgresTestContext)
    {
        _postgresTestContext = postgresTestContext.GetNewContext();
    }

    [Fact]
    public void RowVersionTest()
    {
        var testEntity = new TestEntity { TestNumber = 40, RowVersion = null};
        _postgresTestContext.TestEntities.Add(testEntity);
        _postgresTestContext.SaveChanges();

        var initialRowVersion = testEntity.RowVersion;

        testEntity.TestNumber = 23;
        _postgresTestContext.SaveChanges();
        var updatedRowVersion = testEntity.RowVersion;

        Assert.NotNull(initialRowVersion);
        Assert.NotEmpty(initialRowVersion);
        Assert.NotNull(updatedRowVersion);
        Assert.NotEmpty(updatedRowVersion);
        Assert.NotEqual(initialRowVersion, updatedRowVersion);
    }

    [Fact]
    public void RowVersionConcurrentTest()
    {
        var testEntity = new TestEntity { TestNumber = 41 };
        _postgresTestContext.TestEntities.Add(testEntity);
        _postgresTestContext.SaveChanges();

        _postgresTestContext.Database. ExecuteSqlRaw("UPDATE \"TestEntity\" SET \"TestNumber\"=17 WHERE \"TestNumber\"=41");

        testEntity.TestNumber = 23;

        Assert.Throws<DbUpdateConcurrencyException>(() => _postgresTestContext.SaveChanges());
        Assert.Equal(1, _postgresTestContext.Database.ExecuteSqlRaw(
            "UPDATE \"TestEntity\" SET \"TestNumber\"=\"TestNumber\" WHERE \"TestNumber\"=17"));
    }

    [Fact]
    public void MaxLengthConstraintTest()
    {
        var testEntity = new TestEntity { TestString = "A text which is much longer the the maximum allowed length of 32 characters..." };
        _postgresTestContext.TestEntities.Add(testEntity);

        Assert.True(testEntity.TestString.Length > 32);
        Assert.Throws<DbUpdateException>(() => _postgresTestContext.SaveChanges());
    }
}