using System.IO;
using System.Threading.Tasks;

using IAG.Infrastructure.DI;

namespace IAG.Infrastructure.Settings;

public class VersionInformation
{
    private readonly string _installationPath;

    public VersionInformation(string installationPath)
    {
        _installationPath = installationPath;
    }

    public string BpeVersion
    {
        get
        {
            var mainFile = Path.Combine(_installationPath, "IAG.Infrastructure.dll");
            return File.Exists(mainFile) ? AssemblyInspector.GetProductVersion(mainFile) : null;
        }
    }

    public string ProductName
    {
        get
        {
            var exeFiles = Directory.GetFiles(_installationPath, "IAG.*.Host*.exe");
            return exeFiles.Length > 0 ? Path.GetFileName(exeFiles[0]).Split(".")[1] : null;
        }
    }

    public async Task<string> CustomerPluginNameAsync()
    {
        var iagAssemblyPaths = Directory.GetFiles(_installationPath, "IAG*.dll");
        using var assemblyInspector = new AssemblyInspector(_installationPath);
        foreach (var iagAssemblyPath in iagAssemblyPaths)
        {
            var assembly = await assemblyInspector.GetAssemblyAsync(iagAssemblyPath);
            var customerPluginId = AssemblyInspector.GetCustomerPluginId(assembly);
            if (customerPluginId != null)
            {
                return Path.GetFileNameWithoutExtension(iagAssemblyPath);
            }
        }

        return null;
    }
}