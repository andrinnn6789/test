using IAG.DataLayer.GenerateCreateScript.Helper;
using IAG.Infrastructure.DataLayer.Context;

using Xunit;

namespace IAG.DataLayer.GenerateCreateScript.Test.Helper;

public class ArgumentHandlerTest
{
    [Fact]
    public void NoArgumentsTest()
    {
        var arguments = new string[] {};
        var argumentHelper = new ArgumentHandler(arguments);

        Assert.False(argumentHelper.ShowHelp);
        Assert.Null(argumentHelper.DbContextType);
        Assert.Null(argumentHelper.DbContextAssemblyPath);
        Assert.Equal(ArgumentHandler.DefaultDatabaseType, argumentHelper.Database);
    }

    [Fact]
    public void HelpArgumentTest()
    {
        var arguments = new [] { "help" };
        var argumentHelper = new ArgumentHandler(arguments);

        Assert.True(argumentHelper.ShowHelp);
        Assert.Null(argumentHelper.DbContextType);
        Assert.Null(argumentHelper.DbContextAssemblyPath);
        Assert.Equal(ArgumentHandler.DefaultDatabaseType, argumentHelper.Database);
    }

    [Fact]
    public void DbTypeOnlyArgumentsTest()
    {
        var arguments = new [] { DatabaseType.Sqlite.ToString() };
        var argumentHelper = new ArgumentHandler(arguments);

        Assert.False(argumentHelper.ShowHelp);
        Assert.Null(argumentHelper.DbContextType);
        Assert.Null(argumentHelper.DbContextAssemblyPath);
        Assert.Equal(DatabaseType.Sqlite, argumentHelper.Database);
    }

    [Fact]
    public void AssemblyOnlyArgumentsTest()
    {
        var assemblyPath = "FullPathToAssembly";
        var arguments = new[] { assemblyPath };
        var argumentHelper = new ArgumentHandler(arguments);

        Assert.False(argumentHelper.ShowHelp);
        Assert.Null(argumentHelper.DbContextType);
        Assert.Equal(assemblyPath, argumentHelper.DbContextAssemblyPath);
        Assert.Equal(ArgumentHandler.DefaultDatabaseType, argumentHelper.Database);
    }

    [Fact]
    public void AssemblyWithDbTypeArgumentsTest()
    {
        var assemblyPath = "FullPathToAssembly";
        var arguments = new[] { assemblyPath, DatabaseType.Sqlite.ToString() };
        var argumentHelper = new ArgumentHandler(arguments);

        Assert.False(argumentHelper.ShowHelp);
        Assert.Null(argumentHelper.DbContextType);
        Assert.Equal(assemblyPath, argumentHelper.DbContextAssemblyPath);
        Assert.Equal(DatabaseType.Sqlite, argumentHelper.Database);
    }

    [Fact]
    public void FullContextArgumentsTest()
    {
        var dbContextType = "MyPackage.MyDbContext";
        var assemblyPath = "FullPathToAssembly";
        var arguments = new[] { $"{dbContextType}@{assemblyPath}" };
        var argumentHelper = new ArgumentHandler(arguments);

        Assert.False(argumentHelper.ShowHelp);
        Assert.Equal(dbContextType, argumentHelper.DbContextType);
        Assert.Equal(assemblyPath, argumentHelper.DbContextAssemblyPath);
        Assert.Equal(ArgumentHandler.DefaultDatabaseType, argumentHelper.Database);
    }

    [Fact]
    public void FullArgumentsTest()
    {
        var dbContextType = "MyPackage.MyDbContext";
        var assemblyPath = "FullPathToAssembly";
        var arguments = new[] { $"{dbContextType}@{assemblyPath}", DatabaseType.Sqlite.ToString() };
        var argumentHelper = new ArgumentHandler(arguments);

        Assert.False(argumentHelper.ShowHelp);
        Assert.Equal(dbContextType, argumentHelper.DbContextType);
        Assert.Equal(assemblyPath, argumentHelper.DbContextAssemblyPath);
        Assert.Equal(DatabaseType.Sqlite, argumentHelper.Database);
    }
}