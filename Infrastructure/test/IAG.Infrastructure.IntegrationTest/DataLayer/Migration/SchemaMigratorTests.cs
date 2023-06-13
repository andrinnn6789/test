using System;
using System.Linq;

using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.DataLayer.Model.System;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IntegrationTest.DataLayer.Migration.TestContext;
using IAG.Infrastructure.IntegrationTest.DataLayer.Migration.TestContext.MigrationScripts;
using IAG.Infrastructure.TestHelper.xUnit;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Moq;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.DataLayer.Migration;

[TestCaseOrderer("IAG.Infrastructure.Test.xUnit.PriorityOrderer", "IAG.Infrastructure.Test")]
public class SchemaMigratorTest
{
    private const string ModuleName = "Infrastructure.IntegrationTest.TestProject";

    private readonly SchemaMigrator _migrator;
    private readonly dynamic _dynMigrator;

    private readonly DbContextOptions _dbOptions;
    private readonly TestProjectDbContext _dbContext;

    public SchemaMigratorTest()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        var optionsBuilder = new DbContextOptionsBuilder().UseSqlite(connection);
        connection.Open();

        _dbOptions = optionsBuilder.Options;
        var mockServiceProvider = new Mock<IServiceProvider>();
        _migrator = new SchemaMigrator(mockServiceProvider.Object);
        _dynMigrator = new PrivateObject<SchemaMigrator>(_migrator);

        _dbContext = new TestProjectDbContext(_dbOptions);
    }

    [Fact]
    public void GetDbVersionForEmptyDbTest()
    {
        string version = _dynMigrator.GetDbVersion(DatabaseType.Memory, _dbContext, "DoesNotMatter");

        Assert.Null(version);
    }

    [Fact]
    public void GetDbVersionOtherExceptionTest()
    {
        _dbContext.Dispose();

        Assert.Throws<ObjectDisposedException>(() => { _dynMigrator.GetDbVersion(DatabaseType.Memory, _dbContext, "DoesNotMatter"); });
    }


    [Fact, TestPriority(1)]
    public void MigrateFromScratchTo110Test()
    {
        _migrator.DoMigration(_dbContext);

        // because table 'TestEntity' was created by migration script, column "CheckCreatedByScript" is available
        var checkColumnCreatedByScript = _dbContext.Database. ExecuteSqlRaw("UPDATE TestEntity SET CheckCreatedByScript=2 WHERE CheckCreatedByScript=1");
            
        // because we start from version 1.0.0 PostProcessorInsertCheckEntryInVersion101 should be processed
        var checkPostProcess = _dbContext.Database. ExecuteSqlRaw($"UPDATE TestEntity SET TestString='' WHERE TestNumber={PostProcessorInsertCheckEntryInVersion101.UsedTestNumber}");

        AssertFinalMigrationState();
        Assert.Equal(_dbContext.TestEntities.Count(), checkColumnCreatedByScript);
        Assert.Equal(1, checkPostProcess);
    }

    [Fact, TestPriority(1)]
    public void MigrateFrom100To110Test()
    {
        EnsureVersion100Schema();

        _migrator.DoMigration(_dbContext);

        AssertFinalMigrationState();

        // because table 'TestEntity' was created by db context, column "CheckCreatedByScript" is not available
        Assert.Throws<SqliteException>(() => { _dbContext.Database. ExecuteSqlRaw("UPDATE TestEntity SET CheckCreatedByScript=2"); });
    }

    [Fact, TestPriority(1)]
    public void MigrateFrom101To110Test()
    {
        EnsureVersion101Schema();

        _migrator.DoMigration(_dbContext);

        // because we start on version 1.0.1 PostProcessorInsertCheckEntryInVersion101 should not be processed
        var columnCount = _dbContext.Database. ExecuteSqlRaw($"UPDATE TestEntity SET TestString='' WHERE TestNumber={PostProcessorInsertCheckEntryInVersion101.UsedTestNumber}");

        AssertFinalMigrationState();

        // because table 'TestEntity' was created by db context, column "CheckCreatedByScript" is not available
        Assert.Throws<SqliteException>(() => { _dbContext.Database. ExecuteSqlRaw("UPDATE TestEntity SET CheckCreatedByScript=2"); });
        Assert.Equal(0, columnCount);
    }

    [Fact, TestPriority(1)]
    public void NoMigrateBecauseHighVersionNumberTest()
    {
        EnsureBaseSchema();
        AddSchemaVersion(ModuleName, "1.2.0");
        _dbContext.SaveChanges();
        var moduleName = SchemaMigrator.GetModuleNameFromContextType(_dbContext.GetType());

        _migrator.DoMigration(_dbContext);

        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        // Table TestEntities should not be created...
        Assert.Throws<SqliteException>(() => { _dbContext.TestEntities.FirstOrDefault(); });
        Assert.Equal("1.2.0", Assert.Single(_dbContext.SchemaVersions.Where(m => m.Module == moduleName))?.Version);
    }

    [Fact, TestPriority(2)]
    public void MigrateFailsBecausePreprocessorMismatchTest()
    {
        var correctVersion = PreProcessorSql23Replacer.VersionSetter;
        PreProcessorSql23Replacer.VersionSetter = "WrongVersion";

        Assert.Throws<FormatException>(() => _migrator.DoMigration(_dbContext));

        PreProcessorSql23Replacer.VersionSetter = correctVersion;
    }


    private void EnsureBaseSchema()
    {
        using BaseDbContext dbContext = new(_dbOptions, new ExplicitUserContext("test", null));
        dbContext.Database.EnsureCreated();
        UpdateBaseVersions();
        dbContext.SaveChanges();
    }

    private void EnsureVersion100Schema()
    {
        _dbContext.Database.EnsureCreated();
        UpdateBaseVersions();
        AddSchemaVersion(ModuleName, "1.0.0");
        _dbContext.SaveChanges();
    }

    private void UpdateBaseVersions()
    {
        AddSchemaVersion("Infrastructure.Base", "1.0.0");
        AddSchemaVersion("Infrastructure.Base", "1.0.1");
    }

    private void EnsureVersion101Schema()
    {
        EnsureVersion100Schema();

        _dbContext.TestEntities.Add(new TestEntity()
        {
            Id = Guid.NewGuid(),
            TestNumber = 42,
            TestString = "Hello World"
        });

        AddSchemaVersion(ModuleName, "1.0.1");
        _dbContext.SaveChanges();
    }

    private void AddSchemaVersion(string moduleName, string version)
    {
        _dbContext.SchemaVersions.Add(new SchemaVersion()
        {
            Id = Guid.NewGuid(),
            Module = moduleName,
            Version = version
        });
    }

    private void AssertFinalMigrationState()
    {
        var moduleName = SchemaMigrator.GetModuleNameFromContextType(_dbContext.GetType());
        Assert.Equal("1.1.0", _dbContext.SchemaVersions.Where(m => m.Module == moduleName).Max(x => x.Version));
        Assert.Empty(_dbContext.TestEntities.Where(x => x.TestNumber == 42));
        Assert.Single(_dbContext.TestEntities.Where(x => x.TestNumber == 23));
    }
}