using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IAG.DataLayer.GenerateCreateScript.Helper;

internal class AssemblyResolver
{
    private readonly string _path;

    internal AssemblyResolver(string additionalPath)
    {
        _path = additionalPath;
    }

    [ExcludeFromCodeCoverage]
    internal Assembly Resolve(object sender, ResolveEventArgs args)
    {
        if (args?.Name == null)
            return null;
        // Ignore missing resources
        if (args.Name.Contains(".resources"))
            return null;

        // check for assemblies already loaded
        var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
        if (assembly != null)
            return assembly;

        // Try to load by filename - split out the filename of the full assembly name
        // and append the base path of the original assembly (ie. look in the same dir)
        var filename = args.Name.Split(',')[0] + ".dll".ToLower();

        var asmFile = Path.Combine(_path, filename);

        try
        {
            return Assembly.LoadFrom(asmFile);
        }
        catch (Exception)
        {
            return null;
        }
    }

}