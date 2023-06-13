using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.DataLayer.Model.System;
using IAG.Infrastructure.Test.DataLayer.Migration.TestContext;
using IAG.Infrastructure.Test.DataLayer.Migration.TestDummy;
using IAG.Infrastructure.TestHelper.xUnit;

using Microsoft.EntityFrameworkCore;

using Moq;

using Xunit;

namespace IAG.Infrastructure.Test.DataLayer.Migration;

public class SchemaMigratorTest
{
    private readonly dynamic _dynMigrator;
    private readonly DbContextOptions _dbOptions;

    public SchemaMigratorTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<BaseDbContext>().UseInMemoryDatabase($"SchemaMigratorTestDb_{Guid.NewGuid()}");
        _dbOptions = optionsBuilder.Options;
        var mockServiceProvider = new Mock<IServiceProvider>();
        _dynMigrator = new PrivateObject<SchemaMigrator>(new SchemaMigrator(mockServiceProvider.Object));
    }

    [Fact]
    public void TraverseTypeTreeTest()
    {
        IEnumerable<ContextInfo> types = _dynMigrator.TraverseInheritance(new SpecificTestProjectDbContext(_dbOptions));
        var typeList = types.ToList();

        Assert.Equal(3, typeList.Count);
        Assert.Equal(typeof(BaseDbContext), typeList[0].Type);
        Assert.Equal(typeof(TestProjectDbContext), typeList[1].Type);
        Assert.Equal("TestProject", typeList[1].Name);
        Assert.Equal(typeof(SpecificTestProjectDbContext), typeList[2].Type);
        Assert.Equal("MySpecificTestProject", typeList[2].Name);
    }

    [Fact]
    public void TraverseTypeTreeFailureTest()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            IEnumerable<ContextInfo> types = _dynMigrator.TraverseInheritance(new TestWithEmptyDbContext(_dbOptions));
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            types.ToList();
        });
    }

    [Fact]
    public void GetModuleNameFromContextTest()
    {
        string infrastructureModuleName = SchemaMigrator.GetModuleNameFromContextType(typeof(BaseDbContext));
        string infrastructureTestModuleName = SchemaMigrator.GetModuleNameFromContextType(typeof(TestProjectImplDbContext));


        Assert.Equal("Infrastructure.Base", infrastructureModuleName);
        Assert.Equal("Infrastructure.Test.TestProject", infrastructureTestModuleName);
    }

    [Fact]
    public void GetModuleNameFromContextFailsTest()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            SchemaMigrator.GetModuleNameFromContextType(typeof(TestWithoutAttributeDbContext));
        });
    }

    [Fact]
    public void GetVersionFromScriptTest()
    {
        var scriptPrefix = "DbUpdatePostgres";
        var namespacePrefix = "My.Test.Namespace";

        string noScript = _dynMigrator.GetVersionFromScript("My.Test.Namespace.NotAScript.jpg", scriptPrefix, namespacePrefix);
        string noMigrationScript = _dynMigrator.GetVersionFromScript("My.Test.Namespace.NotAMigrationScript.sql", scriptPrefix, namespacePrefix);
        string wrongNamespace = _dynMigrator.GetVersionFromScript("Some.Other.Namespace.DbUpdatePostgres1.2.3.sql", scriptPrefix, namespacePrefix);
        string version123 = _dynMigrator.GetVersionFromScript("My.Test.Namespace.DbUpdatePostgres1.2.3.sql", scriptPrefix, namespacePrefix);
        string version234 = _dynMigrator.GetVersionFromScript("My.Test.Namespace.SubNamespace.AnotherSubNamespace.DbUpdatePostgres2.3.4.sql", scriptPrefix, namespacePrefix);

        Assert.Null(noScript);
        Assert.Null(noMigrationScript);
        Assert.Null(wrongNamespace);
        Assert.Equal("1.2.3", version123);
        Assert.Equal("2.3.4", version234);
    }

    [Fact]
    public void CheckUpdateVersionTest()
    {
        var currentVersion = "1.2.3";
        var lowerVersion = "1.1.4";
        var higherVersion = "1.3.1";

        bool lower = _dynMigrator.CheckUpdateScriptVersion(currentVersion, lowerVersion);
        bool same = _dynMigrator.CheckUpdateScriptVersion(currentVersion, currentVersion);
        bool higher = _dynMigrator.CheckUpdateScriptVersion(currentVersion, higherVersion);

        Assert.False(lower);
        Assert.False(same);
        Assert.True(higher);
    }

    [Fact]
    public void GetMigrationScriptsFromTypeTest()
    {
        Dictionary<string, string> postgresUpdates = _dynMigrator.GetMigrationScriptsFromType(typeof(TestProjectDbContext), "DbUpdatePostgres");
        Dictionary<string, string> sqliteUpdates = _dynMigrator.GetMigrationScriptsFromType(typeof(TestProjectDbContext), "DbUpdateSqlite");
        Dictionary<string, string> nonsenseUpdates = _dynMigrator.GetMigrationScriptsFromType(typeof(TestProjectDbContext), "PrefixWithoutAnyMatch");

        Assert.NotEmpty(postgresUpdates);
        Assert.NotEmpty(sqliteUpdates);
        Assert.Empty(nonsenseUpdates);
        Assert.Equal(2, postgresUpdates.Count);
        Assert.True(postgresUpdates.ContainsKey("1.0.0"));
        Assert.True(postgresUpdates.ContainsKey("1.1.0"));
        var singleSqliteUpdate = Assert.Single(sqliteUpdates);
        Assert.Equal("1.0.0", singleSqliteUpdate.Key);
    }

    [Fact]
    public void ReadBatchScriptAndSplitTest()
    {
        IEnumerable<string> scripts100 = _dynMigrator.ReadBatchScriptAndSplit(GetType().Assembly, "IAG.Infrastructure.Test.DataLayer.Migration.TestContext.MigrationScripts.DbUpdatePostgres1.0.0.sql");
        IEnumerable<string> scripts110 = _dynMigrator.ReadBatchScriptAndSplit(GetType().Assembly, "IAG.Infrastructure.Test.DataLayer.Migration.TestContext.MigrationScripts.DbUpdatePostgres1.1.0.sql");
        var scripts100Array = scripts100.ToArray();
        var scripts110Array = scripts110.ToArray();

        Assert.NotNull(scripts100Array);
        Assert.Equal(2, scripts100Array.Length);
        Assert.Equal("SELECT 'Test line one'", scripts100Array[0]);
        Assert.Equal($"SELECT 'Test line two'{Environment.NewLine}SELECT 'Test line three'", scripts100Array[1]);
        Assert.NotNull(scripts110Array);
        var singleScript = Assert.Single(scripts110Array);
        Assert.NotNull(singleScript);
        Assert.Equal($"SELECT 'Test line 1'{Environment.NewLine}SELECT 'Test line 2'{Environment.NewLine}SELECT 'Test line 3'", singleScript);
    }

    [Fact]
    public void GetPreProcessorsFromTypeTest()
    {
        List<IPreProcessorSql> preProcessors = _dynMigrator.GetProcessorsFromType<IPreProcessorSql>(typeof(TestProjectDbContext), DatabaseType.Sqlite);

        Assert.NotNull(preProcessors);
        Assert.Equal(3, preProcessors.Count);
        Assert.Contains(preProcessors, p => p.GetType() == typeof(PreProcessorSqlForSqlite));
        Assert.Contains(preProcessors, p => p.GetType() == typeof(PreProcessorSqlForAllDbs));
        Assert.Contains(preProcessors, p => p.GetType() == typeof(PreProcessorSqlForAnyDb));
        Assert.DoesNotContain(preProcessors, p => p.GetType() == typeof(PreProcessorSqlForMsSql));
        Assert.DoesNotContain(preProcessors, p => p.GetType() == typeof(WrongLocatedPreProcessorSql));
    }

    [Fact]
    public void ReadBatchScriptAndSplitExceptionTest()
    {
        IEnumerable<string> scripts = _dynMigrator.ReadBatchScriptAndSplit(GetType().Assembly, "NotExistingResource");
        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        Assert.Throws<System.Exception>(() => { scripts.ToList(); });
    }

    [Fact]
    public void GetDbVersionTest()
    {
        var dbContext = new TestProjectDbContext(_dbOptions);
        dbContext.SchemaVersions.Add(new SchemaVersion { Version = "1.0.1", Module = "B" });
        dbContext.SchemaVersions.Add(new SchemaVersion { Version = "1.2.0", Module = "A" });
        dbContext.SchemaVersions.Add(new SchemaVersion { Version = "1.0.2", Module = "B" });
        dbContext.SchemaVersions.Add(new SchemaVersion { Version = "1.0.0", Module = "A" });
        dbContext.SchemaVersions.Add(new SchemaVersion { Version = "1.0.0", Module = "B" });
        dbContext.SchemaVersions.Add(new SchemaVersion { Version = "1.1.0", Module = "A" });
        dbContext.SaveChanges();

        string versionA = _dynMigrator.GetDbVersion(DatabaseType.Memory, dbContext, "A");
        string versionB = _dynMigrator.GetDbVersion(DatabaseType.Memory, dbContext, "B");
        string versionC = _dynMigrator.GetDbVersion(DatabaseType.Memory, dbContext, "C");

        Assert.Equal("1.2.0", versionA);
        Assert.Equal("1.0.2", versionB);
        Assert.Null(versionC);
    }

    [Fact]
    public void SetDbVersionTest()
    {
        var dbContext = new TestProjectDbContext(_dbOptions);

        _dynMigrator.SetDbVersion(dbContext, "A", "1.2.3");

        var version = Assert.Single(dbContext.SchemaVersions);
        Assert.NotNull(version);
        Assert.Equal("A", version.Module);
        Assert.Equal("1.2.3", version.Version);
    }
}