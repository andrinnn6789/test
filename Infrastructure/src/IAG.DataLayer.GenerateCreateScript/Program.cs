using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;

using IAG.DataLayer.GenerateCreateScript.Helper;
using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.EntityFrameworkCore;

namespace IAG.DataLayer.GenerateCreateScript;

public static class Program
{
    public static void Main(string[] args)
    {
        var argHandler = new ArgumentHandler(args);
        if (argHandler.ShowHelp)
        {
            ShowHelp();
            return;
        }

        var dbContextType = GetDbContext(argHandler.DbContextAssemblyPath, argHandler.DbContextType);
        var dbOptions = GetDbOptions(dbContextType);

        var optionsBuilder = new DbContextOptionsBuilder(dbOptions);
        if (argHandler.Database == DatabaseType.MsSql)
        {
            optionsBuilder = optionsBuilder.UseSqlServer("Host=dummy;Database=dummy");
        }
        else if (argHandler.Database == DatabaseType.Sqlite)
        {
            optionsBuilder = optionsBuilder.UseSqlite("Data Source=dummy");
        }
        else
        {
            optionsBuilder = optionsBuilder.UseNpgsql("Host=dummy;Database=dummy");
        }

        using var context = CreateContext(dbContextType, optionsBuilder.Options);
        var createScript = context.Database.GenerateCreateScript();

        Console.WriteLine(createScript);
        File.WriteAllText(context + ".sql", createScript);
    }

    private static Type GetDbContext(string dbContextAssemblyPath, string specificDbContext)
    {
        var commonDbContextType = typeof(BaseDbContext);
        if (string.IsNullOrEmpty(dbContextAssemblyPath))
        {
            return commonDbContextType;
        }

        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dbContextAssemblyPath);
        var assemblyResolver = new AssemblyResolver(Path.GetDirectoryName(dbContextAssemblyPath));

        AppDomain.CurrentDomain.AssemblyResolve += assemblyResolver.Resolve;
        Type dbContextType;
        try
        {
            if (string.IsNullOrEmpty(specificDbContext))
            {
                dbContextType = assembly.GetTypes().FirstOrDefault(t => typeof(BaseDbContext).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsGenericType);
                if (dbContextType == null)
                {
                    throw new ArgumentException($"Could not find type '{specificDbContext}' in assembly '{dbContextAssemblyPath}'");
                }
            }
            else
            {
                dbContextType = assembly.GetTypes().FirstOrDefault(t => t.ToString().Equals(specificDbContext, StringComparison.InvariantCultureIgnoreCase));
                if (dbContextType == null)
                {
                    throw new ArgumentException($"Could not find any subclass of '{typeof(BaseDbContext)}' in assembly '{dbContextAssemblyPath}'");
                }
            }
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolver.Resolve;
        }

        return dbContextType;
    }

    private static DbContextOptions GetDbOptions(Type dbContextType)
    {
        foreach (var constructor in dbContextType.GetConstructors())
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length == 0 || !typeof(DbContextOptions).IsAssignableFrom(parameters[0].ParameterType))
            {
                continue;
            }

            if (!parameters[0].ParameterType.IsGenericType)
            {
                return new DynamicDbContextOptions(dbContextType);
            }

            if (parameters[0].ParameterType.GenericTypeArguments.Length == 1 && dbContextType.IsAssignableFrom(parameters[0].ParameterType.GenericTypeArguments[0].UnderlyingSystemType))
            {
                var genericType = parameters[0].ParameterType;
                if (!genericType.IsGenericTypeDefinition)
                {
                    genericType = genericType.GetGenericTypeDefinition();
                }
                var constructedType = genericType.MakeGenericType(dbContextType);
                return ActivatorWithCheck.CreateInstance< DbContextOptions>(constructedType);
            }
        }

        throw new ArgumentException($"Could not find any suitable constructor for context type '{dbContextType}'");
    }

    private static DbContext CreateContext(Type dbContextType, DbContextOptions dbContextOptions)
    {
        var constructorParams = new List<object>
        {
            dbContextOptions
        };
        foreach (var constructor in dbContextType.GetConstructors())
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length < 2 || !typeof(IUserContext).IsAssignableFrom(parameters[1].ParameterType))
                continue;
            constructorParams.Add(new ExplicitUserContext("system", null));
            break;
        }

        return Activator.CreateInstance(dbContextType, constructorParams.ToArray()) as DbContext;
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Usage: GenerateCreateScript [[DbContextClassType@]DbContextAssemblyPath] [Database]");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("   DbContextClassType:    Type of the DB context for which the script should be generated.");
        Console.WriteLine($"                          If missing, the first found subclass of '{typeof(BaseDbContext)}'");
        Console.WriteLine("                          will be taken.");
        Console.WriteLine("   DbContextAssemblyPath: Full path to .NET core assembly with specific DB context.");
        Console.WriteLine($"                          If missing, '{typeof(BaseDbContext)}' will be taken");
        Console.WriteLine("   Database:              'mssql', 'sqlite' or 'postgres'");
        Console.WriteLine($"                          If missing, '{ArgumentHandler.DefaultDatabaseType.ToString().ToLower()}' will be taken");
    }
}