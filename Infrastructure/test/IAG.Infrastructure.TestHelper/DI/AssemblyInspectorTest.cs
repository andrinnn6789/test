using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.DI;

using Xunit;

namespace IAG.Infrastructure.TestHelper.DI;

// Just a dummy class to simulate a customer plugin...
[CustomerPluginInfo(CustomerId)]
public static class CustomerPluginInfo
{
    public const string CustomerId = "6A0F73AE-E8A0-4959-AD5C-D72B4053923E";
}

public class AssemblyInspectorTest
{
    [Fact]
    public async Task GetAssemblyTest()
    {
        var testPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            PrepareTestDirectory(testPath);
            using var assemblyInspector = new AssemblyInspector(testPath);

            var testAssemblyPath = Directory.GetFiles(testPath, "IAG*.dll").First();
            var assembly = await assemblyInspector.GetAssemblyAsync(testAssemblyPath);

            Assert.NotNull(assembly);
        }
        finally
        {
            Directory.Delete(testPath, true);
        }
    }

    [Fact]
    public async Task GetCustomerIdTest()
    {
        var testPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            PrepareTestDirectory(testPath);
            using var assemblyInspector = new AssemblyInspector(testPath);

            var thisAssemblyPath = Path.Combine(testPath, Path.GetFileName(GetType().Assembly.Location));
            var thisAssembly = await assemblyInspector.GetAssemblyAsync(thisAssemblyPath);
            var baseAssemblyPath = Path.Combine(testPath, Path.GetFileName(typeof(AssemblyInspector).Assembly.Location));
            var baseAssembly = await assemblyInspector.GetAssemblyAsync(baseAssemblyPath);

            var customerId = AssemblyInspector.GetCustomerPluginId(thisAssembly);
            var baseCustomerId = AssemblyInspector.GetCustomerPluginId(baseAssembly);

            Assert.NotNull(thisAssembly);
            Assert.NotNull(customerId);
            Assert.NotEqual(CustomerPluginInfo.CustomerId, customerId.ToString());
            Assert.Null(baseCustomerId);
        }
        finally
        {
            Directory.Delete(testPath, true);
        }
    }

    [Fact]
    public void GetProductVersionTest()
    {
        var baseAssemblyPath = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty, "IAG.Infrastructure.dll");
        var productVersion = AssemblyInspector.GetProductVersion(baseAssemblyPath);

        Assert.NotEmpty(productVersion);
    }

    private void PrepareTestDirectory(string testPath)
    {
        var assemblyDirectory = Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty;
        Directory.CreateDirectory(testPath);

        foreach (var file in Directory.GetFiles(assemblyDirectory))
        {
            File.Copy(file, Path.Combine(testPath, Path.GetFileName(file)));
        }
    }
}