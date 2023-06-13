using System;
using System.Linq;

using IAG.Infrastructure.DataLayer.Model.System;
using IAG.Infrastructure.Test.DataLayer.Context.TestContext;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.Infrastructure.Test.DataLayer.Context;

public class BaseDbContextTest
{
    [Fact]
    public void SimpleTest()
    {
        using var dbContext = CreateSqliteContext();
        var testSystemLog = new SystemLog {LogType = SystemLogType.Info, Message = "Just a Test"};
        var testTenant = new Tenant {Name = "Test", ParentTenant = new Tenant(), ParentTenantId = new Guid(), TenantContactId = new Guid()};
        var testDivision = new Division {Name = "Test", Tenant = testTenant};
        var testAEntity = new TestAEntity {TestNumber = 42};
        var testBEntity = new TestBEntity {TestString = "Hello World"};
        var testGenericEntity = new TestGenericEntity<string> {TestField = "Generic"};
        var testStart = DateTime.UtcNow;

        dbContext.SystemLogs.Add(testSystemLog);
        dbContext.Tenants.Add(testTenant);
        dbContext.Divisions.Add(testDivision);
        dbContext.TestAEntities.Add(testAEntity);
        dbContext.TestBEntities.Add(testBEntity);
        dbContext.TestGenericEntities.Add(testGenericEntity);

        dbContext.SaveChanges();

        var testEnd = DateTime.UtcNow;

        var sysLogEntry = Assert.Single(dbContext.SystemLogs);
        var entityA = Assert.Single(dbContext.TestAEntities);
        var entityB = Assert.Single(dbContext.TestBEntities);
        Assert.NotNull(sysLogEntry);
        Assert.NotNull(entityA);
        Assert.NotNull(entityB);
        Assert.Single(dbContext.Divisions);
        var entityGeneric = Assert.Single(dbContext.TestGenericEntities);
        Assert.NotNull(entityGeneric);
        Assert.Equal(2, dbContext.Tenants.Count());
        Assert.Equal(TestDbContext.TestUserName, sysLogEntry.CreatedBy);
        Assert.Equal(TestDbContext.TestUserName, entityA.CreatedBy);
        Assert.Equal(TestDbContext.TestUserName, entityGeneric.CreatedBy);
        Assert.Equal(TestDbContext.TestUserName, sysLogEntry.ChangedBy);
        Assert.Equal(TestDbContext.TestUserName, entityA.ChangedBy);
        Assert.Equal(TestDbContext.TestUserName, entityGeneric.ChangedBy);
        Assert.True(CheckDate(sysLogEntry.CreatedOn, testStart, testEnd));
        Assert.True(CheckDate(sysLogEntry.ChangedOn, testStart, testEnd));
        Assert.True(CheckDate(entityA.CreatedOn, testStart, testEnd));
        Assert.True(CheckDate(entityA.ChangedOn, testStart, testEnd));
        Assert.True(CheckDate(entityB.CreatedOn, testStart, testEnd));
        Assert.True(CheckDate(entityB.ChangedOn, testStart, testEnd));
        Assert.True(CheckDate(entityGeneric.CreatedOn, testStart, testEnd));
        Assert.True(CheckDate(entityGeneric.ChangedOn, testStart, testEnd));
    }

    [Fact]
    public void UpdateTest()
    {
        using var dbContext = CreateSqliteContext();
        var testEntityInsert = new TestAEntity { TestNumber = 42 };
        var testStart = DateTime.UtcNow;

        dbContext.TestAEntities.Add(testEntityInsert);
        dbContext.SaveChanges();
        var changedOnAfterInsert = testEntityInsert.ChangedOn;
        var afterInsert = DateTime.UtcNow;

        var testEntityUpdate = dbContext.TestAEntities.Single();
        testEntityUpdate.TestNumber = 23;
        dbContext.SaveChanges();

        var afterUpdate = DateTime.UtcNow;

        var entityA = Assert.Single(dbContext.TestAEntities);
        Assert.NotNull(entityA);
        Assert.Equal(testEntityInsert.CreatedOn, changedOnAfterInsert);
        Assert.Equal(testEntityInsert.CreatedOn, testEntityUpdate.CreatedOn);
        Assert.Equal(testEntityInsert.CreatedOn, entityA.CreatedOn);
        Assert.Equal(entityA.ChangedOn, testEntityInsert.ChangedOn);
        Assert.Equal(entityA.ChangedOn, testEntityUpdate.ChangedOn);
        Assert.True(CheckDate(entityA.CreatedOn, testStart, afterInsert));
        Assert.True(CheckDate(entityA.ChangedOn, afterInsert, afterUpdate));
        Assert.Equal(TestDbContext.TestUserName, testEntityInsert.CreatedBy);
        Assert.Equal(TestDbContext.TestUserName, testEntityInsert.ChangedBy);
        Assert.Equal(TestDbContext.TestUserName, testEntityUpdate.CreatedBy);
        Assert.Equal(TestDbContext.TestUserName, testEntityUpdate.ChangedBy);
        Assert.Equal(TestDbContext.TestUserName, entityA.CreatedBy);
        Assert.Equal(TestDbContext.TestUserName, entityA.ChangedBy);
    }


    [Fact]
    public void OverridenOnCreatedOnUpdatedIgnoredTest()
    {
        using var dbContext = CreateSqliteContext();
        var yesterday = DateTime.UtcNow.AddDays(-1);
        var testEntityInsert = new TestAEntity { TestNumber = 42, CreatedOn = yesterday.AddDays(-1), ChangedOn = yesterday };
        var testStart = DateTime.UtcNow;

        dbContext.TestAEntities.Add(testEntityInsert);
        dbContext.SaveChanges();
        var afterInsert = DateTime.UtcNow;
        var createdOnAfterInsert = testEntityInsert.CreatedOn;
        var changedOnAfterInsert = testEntityInsert.ChangedOn;

        var testEntityUpdate = dbContext.TestAEntities.Single();
        testEntityUpdate.TestNumber = 23;
        testEntityUpdate.CreatedOn = yesterday.AddDays(-1);
        testEntityUpdate.ChangedOn = yesterday;
        testEntityUpdate.CreatedBy = "fakeUser";
        testEntityUpdate.ChangedBy = "fakeUser";
        dbContext.SaveChanges();

        var afterUpdate = DateTime.UtcNow;

        var entityA = Assert.Single(dbContext.TestAEntities);
        Assert.NotNull(entityA);
        Assert.Equal(createdOnAfterInsert, changedOnAfterInsert);
        Assert.Equal(createdOnAfterInsert, testEntityInsert.CreatedOn);
        Assert.Equal(createdOnAfterInsert, testEntityUpdate.CreatedOn);
        Assert.Equal(createdOnAfterInsert, entityA.CreatedOn);
        Assert.Equal(entityA.ChangedOn, testEntityInsert.ChangedOn);
        Assert.Equal(entityA.ChangedOn, testEntityUpdate.ChangedOn);
        Assert.True(CheckDate(entityA.CreatedOn, testStart, afterInsert));
        Assert.True(CheckDate(entityA.ChangedOn, afterInsert, afterUpdate));
        Assert.Equal(TestDbContext.TestUserName, testEntityUpdate.CreatedBy);
        Assert.Equal(TestDbContext.TestUserName, testEntityUpdate.ChangedBy);
    }

    [Fact]
    public void TableNameTest()
    {
        using var dbContext = CreateSqliteContext();
        var testAEntity = new TestAEntity { TestNumber = 42 };
        var testBEntity = new TestBEntity { TestString = "Hello World" };
        var testGenericEntity = new TestGenericEntity<string> { TestField = "Generic" };

        dbContext.TestAEntities.Add(testAEntity);
        dbContext.TestBEntities.Add(testBEntity);
        dbContext.TestGenericEntities.Add(testGenericEntity);

        dbContext.SaveChanges();

        var updateCountA = dbContext.Database. ExecuteSqlRaw("UPDATE TestAEntity SET TestNumber=23");
        var updateCountB = dbContext.Database. ExecuteSqlRaw("UPDATE TestBetaEntity SET TestString='Hello Test'");
        var updateCountGeneric = dbContext.Database. ExecuteSqlRaw("UPDATE TestGenericEntity SET TestField='Generic Test'");

        dbContext.Entry(testAEntity).State = EntityState.Detached;
        dbContext.Entry(testBEntity).State = EntityState.Detached;
        dbContext.Entry(testGenericEntity).State = EntityState.Detached;

        Assert.Equal(1, updateCountA);
        Assert.Equal(1, updateCountB);
        Assert.Equal(1, updateCountGeneric);
        Assert.Equal(23, Assert.Single(dbContext.TestAEntities)?.TestNumber);
        Assert.Equal("Hello Test", Assert.Single(dbContext.TestBEntities)?.TestString);
        Assert.Equal("Generic Test", Assert.Single(dbContext.TestGenericEntities)?.TestField);
        Assert.Throws<SqliteException>(() => { dbContext.Database. ExecuteSqlRaw("SELECT * FROM TestAEntities"); });
        Assert.Throws<SqliteException>(() => { dbContext.Database. ExecuteSqlRaw("SELECT * FROM TestBEntity"); });
    }

    [Fact]
    public void RowVersionTest()
    {
        using var dbContext = CreateSqliteContext();
        var testEntity = new TestAEntity { TestNumber = 42};
        dbContext.TestAEntities.Add(testEntity);
        dbContext.SaveChanges();

        dbContext.Database. ExecuteSqlRaw("UPDATE TestAEntity SET TestNumber=17");

        testEntity.TestNumber = 23;
        var updatedCount =  dbContext.SaveChanges();

        var test17Counter = dbContext.Database. ExecuteSqlRaw("UPDATE TestAEntity SET TestNumber=TestNumber WHERE TestNumber=17");
        var test23Counter = dbContext.Database. ExecuteSqlRaw("UPDATE TestAEntity SET TestNumber=TestNumber WHERE TestNumber<>17");

        Assert.Equal(1, updatedCount);
        Assert.Equal(0, test17Counter);
        Assert.Equal(1, test23Counter);
    }


    private TestDbContext CreateSqliteContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        var optionsBuilder = new DbContextOptionsBuilder().UseSqlite(connection);
        connection.Open();

        var dbContext = new TestDbContext(optionsBuilder.Options);
        dbContext.Database.EnsureCreated();

        return dbContext;
    }

    private bool CheckDate(DateTime? dtToCheck, DateTime from, DateTime till)
    {
        if (dtToCheck == null)
            return false;

        return dtToCheck >= from && dtToCheck <= till;
    }
}