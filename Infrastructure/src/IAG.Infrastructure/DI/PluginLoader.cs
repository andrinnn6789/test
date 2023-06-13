using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace IAG.Infrastructure.DI;

public class PluginLoader : IPluginLoader
{
    public IEnumerable<Assembly> GetAssemblies(string assemblyFilter = null)
    {
        var currentDir = Directory.GetCurrentDirectory();
        var executableLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? String.Empty;
        if (string.IsNullOrWhiteSpace(assemblyFilter))
        {
            assemblyFilter = "IAG*.dll";
        }
        else
        {
            var filterPath = Path.GetDirectoryName(assemblyFilter);
            if (!string.IsNullOrEmpty(filterPath))
            {
                if (Path.IsPathRooted(filterPath))
                {
                    currentDir = filterPath;
                    executableLocation = filterPath;
                }
                else
                {
                    currentDir = Path.Combine(currentDir, filterPath);
                    executableLocation = Path.Combine(executableLocation, filterPath);
                }

                assemblyFilter = Path.GetFileName(assemblyFilter);
            }
        }

        foreach (var assemblyPath in
                 Directory.GetFiles(executableLocation, assemblyFilter, SearchOption.AllDirectories))
        {
            yield return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
        }

        if (executableLocation != currentDir && executableLocation.IndexOf(currentDir, StringComparison.Ordinal) != 0)
        {
            foreach (var assemblyPath in Directory.GetFiles(currentDir, assemblyFilter, SearchOption.AllDirectories))
            {
                yield return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
            }
        }
    }

    public IEnumerable<T> GetExports<T>(string assemblyFilter = null)
    {
        var configuration = new ContainerConfiguration().WithAssemblies(GetAssemblies(assemblyFilter));
        using var container = configuration.CreateContainer();
        return container.GetExports<T>().ToList();
    }

    public IEnumerable<Type> GetImplementations<T>(string assemblyFilter = null, bool includeAbstract = false)
    {
        return GetImplementations(typeof(T), assemblyFilter, includeAbstract);
    }

    public IEnumerable<Type> GetImplementations(Type type, string assemblyFilter = null, bool includeAbstract = false)
    {
        foreach (var assembly in GetAssemblies(assemblyFilter))
        {
            foreach (var implType in GetImplementationsInAssembly(assembly, type, includeAbstract))
            {
                yield return implType;
            }
        }
    }

    public IEnumerable<Type> GetImplementationsInAssembly<T>(Assembly assembly, bool includeAbstract = false)
    {
        return GetImplementationsInAssembly(assembly, typeof(T), includeAbstract);
    }

    public IEnumerable<Type> GetImplementationsInAssembly(Assembly assembly, Type type, bool includeAbstract = false)
    {
        return assembly.GetTypes()
            .Where(t => IsImplementing(type, t) && t.IsClass && (includeAbstract || !t.IsAbstract));
    }

    public IEnumerable<Type> GetMostSpecificImplementations(Type type)
    {
        var candidates = GetImplementations(type, null, true).ToHashSet();
        foreach (Type implType in candidates.ToList())
        {
            var baseType = implType.BaseType;
            if (baseType?.IsGenericType == true)
            {
                baseType = baseType.GetGenericTypeDefinition();
            }

            candidates.Remove(baseType);
        }

        return candidates.Where(t => !t.IsAbstract);
    }

    public bool IsImplementing(Type baseType, Type implementationCandidateType)
    {
        if (baseType.IsGenericType)
        {
            baseType = baseType.GetGenericTypeDefinition();
        }

        while (implementationCandidateType != null)
        {
            var curType = implementationCandidateType.IsGenericType
                ? implementationCandidateType.GetGenericTypeDefinition()
                : implementationCandidateType;
            if (baseType == curType || baseType.IsAssignableFrom(curType))
            {
                return true;
            }

            implementationCandidateType = implementationCandidateType.BaseType;
        }

        return false;
    }

    public IEnumerable<Type> GetTypesWithAttribute(Type attributeType, string assemblyFilter = null)
    {
        var types = new List<Type>();
        
        foreach (var assembly in GetAssemblies(assemblyFilter))
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.GetCustomAttributes(attributeType, inherit: true).Any()) continue;
                types.Add(type);
            }
        }

        return types;
    }
}