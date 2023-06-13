using System;
using System.IO;

using IAG.DataLayer.GenerateCreateScript.Test.TestContext;
using IAG.Infrastructure.DataLayer.Context;

using Xunit;

namespace IAG.DataLayer.GenerateCreateScript.Test;

public class ProgramTest
{
    private readonly StringWriter _output = new();

    public ProgramTest()
    {
        Console.SetOut(_output);
    }

    [Fact]
    public void HelpTest()
    {
        var arguments = new [] { "help" };

        Program.Main(arguments);

        var output = _output.ToString();
        Assert.NotNull(output);
        Assert.StartsWith("Usage: GenerateCreateScript", output);
    }

    [Fact]
    public void BaseDbContextPostgresTest()
    {
        var arguments = new [] { DatabaseType.Postgres.ToString() };
        Program.Main(arguments);

        var output = _output.ToString();
        Assert.NotNull(output);
        Assert.Contains("CREATE TABLE \"SchemaVersion\"", output);
        Assert.Contains("CREATE TABLE \"SystemLog\"", output);
        Assert.Contains("\"Id\" uuid NOT NULL", output);
        Assert.DoesNotContain("CREATE TABLE \"TestEntryA\"", output);
        Assert.DoesNotContain("CREATE TABLE \"TestEntryB\"", output);
    }

    [Fact]
    public void BaseDbContextSqliteTest()
    {
        var arguments = new[] { DatabaseType.Sqlite.ToString() };
        Program.Main(arguments);

        var output = _output.ToString();
        Assert.NotNull(output);
        Assert.Contains("CREATE TABLE \"SchemaVersion\"", output);
        Assert.Contains("CREATE TABLE \"SystemLog\"", output);
        Assert.Contains("\"Id\" TEXT NOT NULL CONSTRAINT \"PK_SystemLog\" PRIMARY KEY", output);
        Assert.DoesNotContain("CREATE TABLE \"TestEntryA\"", output);
        Assert.DoesNotContain("CREATE TABLE \"TestEntryB\"", output);
    }

    [Fact]
    public void BaseDbContextMsSqlTest()
    {
        var arguments = new[] { DatabaseType.MsSql.ToString() };
        Program.Main(arguments);

        var output = _output.ToString();
        Assert.NotNull(output);
        Assert.Contains("CREATE TABLE [SchemaVersion]", output);
        Assert.Contains("CREATE TABLE [SystemLog]", output);
        Assert.Contains("[Id] uniqueidentifier NOT NULL", output);
        Assert.DoesNotContain("CREATE TABLE [TestEntryA]", output);
        Assert.DoesNotContain("CREATE TABLE [TestEntryB]", output);
    }

    [Fact]
    public void TestUnspecifiedDbContextPostgresTest()
    {
        var assemblyPath = GetType().Assembly.Location;
        var arguments = new[] { assemblyPath, DatabaseType.Postgres.ToString() };
        Program.Main(arguments);

        var output = _output.ToString();
        Assert.NotNull(output);
        Assert.Contains("CREATE TABLE \"SchemaVersion\"", output);
        Assert.Contains("CREATE TABLE \"SystemLog\"", output);
        Assert.Contains("CREATE TABLE \"TestEntryA\"", output);
    }

    [Fact]
    public void TestDbContextAPostgresTest()
    {
        var assemblyPath = GetType().Assembly.Location;
        var dbContextType = typeof(TestADbContext).FullName;
        var arguments = new[] { $"{dbContextType}@{assemblyPath}", DatabaseType.Postgres.ToString() };
        Program.Main(arguments);

        var output = _output.ToString();
        Assert.NotNull(output);
        Assert.Contains("CREATE TABLE \"SchemaVersion\"", output);
        Assert.Contains("CREATE TABLE \"SystemLog\"", output);
        Assert.Contains("CREATE TABLE \"TestEntryA\"", output);
        Assert.DoesNotContain("CREATE TABLE \"TestEntryB\"", output);
    }

    [Fact]
    public void TestDbContextBPostgresTest()
    {
        var assemblyPath = GetType().Assembly.Location;
        var dbContextType = typeof(TestBDbContext).FullName;
        var arguments = new[] { $"{dbContextType}@{assemblyPath}", DatabaseType.Postgres.ToString() };
        Program.Main(arguments);

        var output = _output.ToString();
        Assert.NotNull(output);
        Assert.Contains("CREATE TABLE \"SchemaVersion\"", output);
        Assert.Contains("CREATE TABLE \"SystemLog\"", output);
        Assert.Contains("CREATE TABLE \"TestEntryA\"", output);
        Assert.Contains("CREATE TABLE \"TestEntryB\"", output);
    }

    [Fact]
    public void TestInvalidAssemblyPathTest()
    {
        var assemblyPath = "X:\\NotExistingAssembly.dll";
        var arguments = new[] { assemblyPath };

        Assert.Throws<FileNotFoundException>(() => Program.Main(arguments));
    }

    [Fact]
    public void TestAssemblyWithoutDbContextTest()
    {
        var assemblyPath = typeof(Program).Assembly.Location;
        var arguments = new[] { assemblyPath };

        Assert.Throws<ArgumentException>(() => Program.Main(arguments));
    }

    [Fact]
    public void TestInvalidDbContextTest()
    {
        var assemblyPath = GetType().Assembly.Location;
        var dbContextType = "NotExisting.DbContext.Type";
        var arguments = new[] { $"{dbContextType}@{assemblyPath}", DatabaseType.Postgres.ToString() };

        Assert.Throws<ArgumentException>(() => Program.Main(arguments));
    }

    [Fact]
    public void TestUnsupportedDbContextTest()
    {
        var assemblyPath = GetType().Assembly.Location;
        var dbContextType = typeof(TestUnsupportedDbContext).FullName;
        var arguments = new[] { $"{dbContextType}@{assemblyPath}", DatabaseType.Postgres.ToString() };

        Assert.Throws<ArgumentException>(() => Program.Main(arguments));
    }
}