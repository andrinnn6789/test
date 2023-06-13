using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Model.System;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Exception.DbException;

using Microsoft.EntityFrameworkCore;

namespace IAG.Infrastructure.DataLayer.Migration;

// ReSharper disable IdentifierTypo
public class SchemaMigrator : ISchemaMigrator
{
    private const string AssemblyPrefix = "IAG.";
    private const string SqlScriptPrefix = "UpdateDb";
    private const string SqlScriptExtension = ".sql";
    private const string BatchSeparator = "GO";
    public const string MigratorUser = "SchemaMigrator";
    private readonly IServiceProvider _serviceProvider;

    public SchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void DoMigration(IMigratorDbContext dbContext)
    {
        foreach (ContextInfo contextInfo in TraverseInheritance(dbContext))
        {
            UpdateDbSchema(dbContext, contextInfo);
        }
    }

    public static string GetModuleNameFromContextType(Type type)
    {
        var contextNameAttribute = type.GetCustomAttributes(typeof(ContextInfoAttribute)).FirstOrDefault() as ContextInfoAttribute;
        if (contextNameAttribute == null || string.IsNullOrEmpty(contextNameAttribute.Name))
        {
            throw new ArgumentException($"No context name defined for {type.FullName}");
        }

        return GetModuleNameFromContextInfo(contextNameAttribute.Type ?? type, contextNameAttribute.Name);
    }

    private static string GetModuleNameFromContextInfo(Type type, string name)
    {
        var assemblyName = type.Assembly.GetName().Name;
        assemblyName = assemblyName?.Substring(AssemblyPrefix.Length);

        return $"{assemblyName}.{name}";
    }

    private IEnumerable<ContextInfo> TraverseInheritance(IMigratorDbContext dbContext)
    {
        var type = dbContext.GetType();
        var typeStack = new Stack<ContextInfo>();
        do
        {
            if (type.GetCustomAttributes(typeof(ContextInfoAttribute)).FirstOrDefault() is ContextInfoAttribute contextInfoAttribute)
            {
                if (string.IsNullOrEmpty(contextInfoAttribute.Name))
                {
                    throw new ArgumentException($"No context name defined for {type}");
                }

                var contextInfo = new ContextInfo
                {
                    Type = contextInfoAttribute.Type ?? type,
                    Name = contextInfoAttribute.Name
                };

                typeStack.Push(contextInfo);
            }

            type = type.BaseType;
        } while (type != null && typeof(IMigratorDbContext).IsAssignableFrom(type));

        while (typeStack.Count > 0)
        {
            yield return typeStack.Pop();
        }
    }

    private void UpdateDbSchema(IMigratorDbContext dbContext, ContextInfo contextInfo)
    {
        var dbType = DatabaseAbstraction.GetDbType(dbContext.Database.ProviderName);
        var scriptPrefix = $"{SqlScriptPrefix}{dbType}";

        var scripts = GetMigrationScriptsFromType(contextInfo.Type, scriptPrefix).ToList().OrderBy(s => s.Key);
        var preProcessorsSql = GetProcessorsFromType<IPreProcessorSql>(contextInfo.Type, dbType);
        var moduleName = GetModuleNameFromContextInfo(contextInfo.Type, contextInfo.Name);
        var currentVersion = GetDbVersion(dbType, dbContext, moduleName);

        foreach (var script in scripts)
        {
            if (!CheckUpdateScriptVersion(currentVersion, script.Key)) continue;

            foreach (var sqlScript in ReadBatchScriptAndSplit(contextInfo.Type.Assembly, script.Value))
            {
                var preProcessedSqlScript = preProcessorsSql.Where(p => CheckProcessorVersion(script.Key, p.ForVersion))
                    .Aggregate(sqlScript, (current, preProcessor) => preProcessor.Process(current));

                dbContext.Database. ExecuteSqlRaw(preProcessedSqlScript);
            }

            SetDbVersion(dbContext, moduleName, script.Key);
            currentVersion = script.Key;

            var postProcessors = GetProcessorsFromType<IPostProcessor>(contextInfo.Type, dbType);
            foreach (var postProcessor in postProcessors)
            {
                if (!CheckProcessorVersion(currentVersion, postProcessor.ForVersion)) continue;

                postProcessor.Process(dbContext.Database, _serviceProvider);
            }
        }
    }

    private Dictionary<string, string> GetMigrationScriptsFromType(Type type, string scriptPrefix)
    {
        var resourceNames = type.Assembly.GetManifestResourceNames();

        var scripts = new Dictionary<string, string>();
        foreach (var resourceName in resourceNames)
        {
            var version = GetVersionFromScript(resourceName, scriptPrefix, type.Namespace);
            if (!string.IsNullOrEmpty(version))
            {
                scripts.Add(version, resourceName);
            }
        }

        return scripts;
    }

    private List<T> GetProcessorsFromType<T>(Type type, DatabaseType dbType)
        where T: class, IProcessor
    {
        var pluginLoader = new PluginLoader();
        var processorTypes = pluginLoader.GetImplementationsInAssembly<T>(type.Assembly);
        var processors = new List<T>();
        foreach (var processorType in processorTypes)
        {
            if (processorType.Namespace != null && type.Namespace != null && !processorType.Namespace.StartsWith(type.Namespace, StringComparison.InvariantCultureIgnoreCase))
            {
                continue; // Processor is in another namespace
            }

            var processor = Activator.CreateInstance(processorType) as T;

            if (CheckForDatabaseTypes(processor?.ForDatabaseTypes, dbType))
            {
                processors.Add(processor);
            }
        }

        return processors;
    }

    private bool CheckForDatabaseTypes(DatabaseType[] databaseTypes, DatabaseType dbType)
    {
        if (databaseTypes == null || databaseTypes.Length == 0)
        {
            // For all types of database
            return true;
        }

        return databaseTypes.Contains(dbType);
    }

    private string GetVersionFromScript(string resourceName, string scriptPrefix, string namespacePrefix)
    {
        if (!resourceName.StartsWith(namespacePrefix, StringComparison.InvariantCultureIgnoreCase))
        {
            return null;    // resource is in another namespace
        }

        if (!resourceName.EndsWith(SqlScriptExtension, StringComparison.InvariantCultureIgnoreCase))
        {
            return null;    // resource is not a SQL script
        }

        var prefixIndex = resourceName.LastIndexOf(scriptPrefix, StringComparison.Ordinal);
        if (prefixIndex < 0)
        {
            return null;    // resource is not a migration script or not for this database type
        }

        prefixIndex += scriptPrefix.Length;
        return resourceName.Substring(prefixIndex, resourceName.Length - prefixIndex - ".sql".Length);
    }

    private bool CheckUpdateScriptVersion(string currentVersion, string scriptVersion)
    {
        // version of update has to be higher than current version of module
        return string.CompareOrdinal(currentVersion, scriptVersion) < 0;
    }

    private bool CheckProcessorVersion(string currentVersion, string processorVersion)
    {
        // version of processor has to equal to the current version
        return currentVersion.Equals(processorVersion, StringComparison.OrdinalIgnoreCase);
    }

    private IEnumerable<string> ReadBatchScriptAndSplit(Assembly assembly, string resourceName)
    {
        var sb = new StringBuilder();

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            // should not happen! Therefore just a simple exception since it could only happen during development...
            throw new System.Exception($"Assembly {assembly} contains no resources");
        }

        string block;
        using (var streamReader = new StreamReader(stream))
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.ToUpper() == BatchSeparator)
                {
                    block = sb.ToString().TrimEnd();
                    if (block.Length > 0)
                    {
                        yield return block;
                    }

                    sb.Clear();
                }
                else if (line.Length > 0)
                {
                    sb.AppendLine(line);
                }
            }
        }

        block = sb.ToString().TrimEnd();
        if (block.Length > 0)
        {
            yield return block;
        }
    }

    private string GetDbVersion(DatabaseType databaseType, IMigratorDbContext dbContext, string moduleName)
    {
        try
        {
            if (!DatabaseAbstraction.TableExists(dbContext.Database, "SchemaVersion", databaseType))
                return null;

            return dbContext.SchemaVersions
                .Where(ver => ver.Module == moduleName)
                .Select(t => t.Version)
                .DefaultIfEmpty().ToList().Max();
        }
        catch (System.Exception ex)
        {
            var dbException = new DbExceptionResolver(ex).TranslateException();
            if (dbException.GetType() == typeof(DbUndefinedTableException))
            {
                return null;
            }

            throw;
        }
    }

    private void SetDbVersion(IMigratorDbContext dbContext, string moduleName, string version)
    {
        var dbVersion = new SchemaVersion
        {
            Id = Guid.NewGuid(),
            ChangedBy = MigratorUser,
            CreatedOn = DateTime.UtcNow,
            Module = moduleName,
            Version = version
        };

        dbContext.SchemaVersions.Add(dbVersion);
        dbContext.SaveChanges();
    }
}