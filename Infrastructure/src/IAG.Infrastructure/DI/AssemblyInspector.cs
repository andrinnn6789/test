using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace IAG.Infrastructure.DI;

public class AssemblyInspector : IDisposable
{
    private static readonly string CustomerPluginInfoAttributeTypeName = typeof(CustomerPluginInfoAttribute).AssemblyQualifiedName;

    private readonly MetadataLoadContext _metadataLoadContext;

    public AssemblyInspector(string directoryName)
    {
        var assemblyResolver = new PathAssemblyResolver(GetDllPaths(directoryName));
        _metadataLoadContext = new MetadataLoadContext(assemblyResolver);
    }

    public async Task<Assembly> GetAssemblyAsync(string assemblyPath)
    {
        var assemblyContent = await File.ReadAllBytesAsync(assemblyPath);
            
        return GetAssembly(assemblyContent);
    }

    public Assembly GetAssembly(byte[] assemblyContent)
    {
        return _metadataLoadContext.LoadFromByteArray(assemblyContent);
    }

    [ExcludeFromCodeCoverage()] // can only be tested partially....
    public static Guid? GetCustomerPluginId(Assembly assembly)
    {
        var customerPluginInfoType = assembly.GetTypes().FirstOrDefault(t => t.Name == "CustomerPluginInfo");
        if (customerPluginInfoType == null)
        {
            return null;
        }

        if (!(customerPluginInfoType.GetCustomAttributesData().FirstOrDefault(attr =>
                    attr.AttributeType.AssemblyQualifiedName == CustomerPluginInfoAttributeTypeName &&
                    attr.ConstructorArguments.Count == 1)?
                .ConstructorArguments.First().Value is string customerIdString))
        {
            return null;
        }

        return Guid.TryParse(customerIdString, out var customerId) ? customerId : null;
    }

    public static string GetProductVersion(string filePath)
    {
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(filePath);

        return ExtractProductVersion(fileVersionInfo.ProductVersion);
    }

    public static string ExtractProductVersion(string fullProductVersion)
    {
        return fullProductVersion?.Split('+').FirstOrDefault();
    }

    private IEnumerable<string> GetDllPaths(string directoryName)
    {
        var coreDllFilePath = Directory
            .GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll")
            .ToDictionary(Path.GetFileName, path => path);

        foreach (var dllFilePath in Directory.GetFiles(directoryName, "*.dll"))
        {
            yield return dllFilePath;
            coreDllFilePath.Remove(Path.GetFileName(dllFilePath));
        }
        foreach (var missingCoreDllFilePath in coreDllFilePath.Values)
        {
            yield return missingCoreDllFilePath;
        }
    }

    public void Dispose()
    {
        _metadataLoadContext.Dispose();
    }
}