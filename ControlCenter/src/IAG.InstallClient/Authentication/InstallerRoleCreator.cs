using System;
using System.IO;
using System.Linq;
using System.Reflection;

using IAG.Infrastructure.ImportExport;

using Newtonsoft.Json.Linq;

namespace IAG.InstallClient.Authentication;

public class InstallerRoleCreator
{
    public static void CreateRole(ISeedImporter importer)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var json = assembly.GetManifestResourceNames().First(r => r.Contains("Seed.Realm.Integrated.InstallClient.json"));
        using var stream = assembly.GetManifestResourceStream(json);
        using var streamReader = new StreamReader(stream ?? throw new InvalidOperationException("seed for installer role not found"));
        var jsonImport = streamReader.ReadToEnd();
        var roleImport = JObject.Parse(jsonImport);
        importer.Import(roleImport);
    }
}