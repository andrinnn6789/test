using System;
using System.IO;
using System.Linq;
using System.Reflection;

using IAG.Infrastructure.DI;
using IAG.Infrastructure.Test.DI.TestData;

using Xunit;

namespace IAG.Infrastructure.Test.DI;

[Collection("UseCurrentDirectory")]
[FakeAttribute]
public class PluginLoaderTest
{
    [Fact]
    public void GetAssembliesWithoutFilterTest()
    {
        var loader = new PluginLoader();

        var assemblies = loader.GetAssemblies();

        Assert.NotNull(assemblies);
        Assert.NotEmpty(assemblies.ToList());
    }

    [Fact]
    public void GetAssembliesWithSimpleFilterTest()
    {
        var loader = new PluginLoader();

        var assemblies = loader.GetAssemblies("xunit*.dll");

        Assert.NotNull(assemblies);
        Assert.NotEmpty(assemblies.ToList());
    }

    [Fact]
    public void GetAssembliesWithNotMatchingFilterTest()
    {
        var loader = new PluginLoader();

        var assemblies = loader.GetAssemblies("SomeSillyFiltername.dll");

        Assert.NotNull(assemblies);
        Assert.Empty(assemblies);
    }

    [Fact]
    public void GetAssembliesRelativeFilterPathTest()
    {
        var loader = new PluginLoader();

        var assemblies = loader.GetAssemblies("./xunit*.dll");

        Assert.NotNull(assemblies);
        Assert.NotEmpty(assemblies.ToList());
    }

    [Fact]
    public void GetAssembliesAbsoluteFilterPathTest()
    {
        var loader = new PluginLoader();

        var assemblies = loader.GetAssemblies(Path.Combine(Directory.GetCurrentDirectory(), "xunit*.dll"));

        Assert.NotNull(assemblies);
        Assert.NotEmpty(assemblies.ToList());
    }

    [Fact]
    public void GetAssembliesOtherPathTest()
    {
        var testAssemblySource = Assembly.GetAssembly(GetType())?.Location ?? string.Empty;
        var testTempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var testAssemblyTarget = Path.Combine(testTempPath, Path.GetFileName(testAssemblySource));
        var oldCurrentDir = Environment.CurrentDirectory;
        try
        {
            Directory.CreateDirectory(testTempPath);
            File.Copy(testAssemblySource, testAssemblyTarget, true);
            Environment.CurrentDirectory = testTempPath;

            var loader = new PluginLoader();
            var assemblies = loader.GetAssemblies()?.ToList();

            Assert.NotNull(assemblies);
            Assert.NotEmpty(assemblies);
        }
        finally
        {
            Environment.CurrentDirectory = oldCurrentDir;
            Directory.Delete(testTempPath, true);
        }
    }

    [Fact]
    public void GetExportsTest()
    {
        var loader = new PluginLoader();

        var exports = loader.GetExports<IPluginConfigureServices>();

        Assert.NotNull(exports);
        Assert.NotEmpty(exports);
    }

    [Fact]
    public void GetImplementationsTest()
    {
        var loader = new PluginLoader();

        var implementations = loader.GetImplementations<IPluginLoader>();

        Assert.NotNull(implementations);
        Assert.NotEmpty(implementations.ToList());
    }

    [Fact]
    public void GetMostSpecificImplementationsTest()
    {
        var loader = new PluginLoader();

        var implementationsOfA = loader.GetMostSpecificImplementations(typeof(A));
        var implementationsOfBase = loader.GetMostSpecificImplementations(typeof(BaseContextImpl))?.ToList();
        var implementationsOfTwo = loader.GetMostSpecificImplementations(typeof(SpecificContextTwo).BaseType)?.ToList();

        Assert.NotNull(implementationsOfA);
        Assert.NotNull(implementationsOfBase);
        Assert.NotNull(implementationsOfTwo);
        Assert.Single(implementationsOfA);
        Assert.Equal(4, implementationsOfBase.Count);
        Assert.Equal(2, implementationsOfTwo.Count);
        Assert.Contains(typeof(BaseContext), implementationsOfBase);
        Assert.Contains(typeof(SpecificContextOne), implementationsOfBase);
        Assert.Contains(typeof(SpecificContextTwo), implementationsOfBase);
        Assert.Contains(typeof(SpecificContextThree), implementationsOfBase);
        Assert.Contains(typeof(SpecificContextTwo), implementationsOfTwo);
        Assert.Contains(typeof(SpecificContextThree), implementationsOfTwo);
    }

    [Fact]
    public void IsImplementingTest()
    {
        var loader = new PluginLoader();

        Assert.True(loader.IsImplementing(typeof(A), typeof(B)));
        Assert.False(loader.IsImplementing(typeof(B), typeof(A)));
        Assert.True(loader.IsImplementing(typeof(BaseContextImpl), typeof(BaseContext)));
        Assert.True(loader.IsImplementing(typeof(BaseContextImpl), typeof(SpecificContextOneImpl)));
        Assert.True(loader.IsImplementing(typeof(BaseContextImpl), typeof(SpecificContextOne)));
        Assert.True(loader.IsImplementing(typeof(BaseContextImpl), typeof(SpecificContextTwo)));
        Assert.True(loader.IsImplementing(typeof(BaseContextImpl), typeof(SpecificContextThree)));
        Assert.False(loader.IsImplementing(typeof(BaseContext), typeof(SpecificContextOneImpl)));
        Assert.False(loader.IsImplementing(typeof(BaseContext), typeof(SpecificContextOne)));
        Assert.False(loader.IsImplementing(typeof(BaseContext), typeof(SpecificContextTwo)));
        Assert.False(loader.IsImplementing(typeof(BaseContext), typeof(SpecificContextThree)));
        Assert.True(loader.IsImplementing(typeof(SpecificContextOneImpl), typeof(SpecificContextOne)));
        Assert.False(loader.IsImplementing(typeof(SpecificContextOneImpl), typeof(BaseContext)));
        Assert.False(loader.IsImplementing(typeof(SpecificContextOneImpl), typeof(SpecificContextTwo)));
        Assert.False(loader.IsImplementing(typeof(SpecificContextOneImpl), typeof(SpecificContextThree)));
        Assert.True(loader.IsImplementing(typeof(SpecificContextTwo).BaseType, typeof(SpecificContextThree)));
        Assert.False(loader.IsImplementing(typeof(SpecificContextTwo), typeof(SpecificContextOne)));
        Assert.False(loader.IsImplementing(typeof(SpecificContextThree), typeof(SpecificContextTwo)));
    }

    [Fact]
    public void GetTypesWithAttribute_ShouldReturnTypesWithAttribute()
    {
        // Arrange
        var loader = new PluginLoader();
        
        // Act
        var types = loader.GetTypesWithAttribute(typeof(FakeAttributeAttribute));
        
        // Assert
        Assert.Single(types);
    }
}

public class FakeAttributeAttribute : Attribute
{
    
}