using System;
using System.Collections.Generic;
using System.Reflection;

namespace IAG.Infrastructure.DI;

public interface IPluginLoader
{
    IEnumerable<Assembly> GetAssemblies(string assemblyFilter = null);

    IEnumerable<T> GetExports<T>(string assemblyFilter = null);

    IEnumerable<Type> GetImplementations<T>(string assemblyFilter = null, bool includeAbstract = false);
    IEnumerable<Type> GetImplementations(Type type, string assemblyFilter = null, bool includeAbstract = false);

    IEnumerable<Type> GetImplementationsInAssembly<T>(Assembly assembly, bool includeAbstract = false);
    IEnumerable<Type> GetImplementationsInAssembly(Assembly assembly, Type type, bool includeAbstract = false);

    IEnumerable<Type> GetMostSpecificImplementations(Type type);

    IEnumerable<Type> GetTypesWithAttribute(Type attributeType, string assemblyFilter = null);

}