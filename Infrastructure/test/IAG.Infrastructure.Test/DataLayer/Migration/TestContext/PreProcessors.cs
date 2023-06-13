using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Test.DataLayer.Migration.TestContext;

[UsedImplicitly]
public class PreProcessorSqlForSqlite: IPreProcessorSql
{
    public string ForVersion => string.Empty;
    public DatabaseType[] ForDatabaseTypes => new[] {DatabaseType.Sqlite};

    public string Process(string command)
    {
        return command;
    }
}

[UsedImplicitly]
public class PreProcessorSqlForMsSql : IPreProcessorSql
{
    public string ForVersion => string.Empty;
    public DatabaseType[] ForDatabaseTypes => new[] { DatabaseType.MsSql };

    public string Process(string command)
    {
        return command;
    }
}

[UsedImplicitly]
public class PreProcessorSqlForAllDbs : IPreProcessorSql
{
    public string ForVersion => string.Empty;
    public DatabaseType[] ForDatabaseTypes => new[] { DatabaseType.MsSql, DatabaseType.Sqlite, DatabaseType.Postgres };

    public string Process(string command)
    {
        return command;
    }
}

[UsedImplicitly]
public class PreProcessorSqlForAnyDb : IPreProcessorSql
{
    public string ForVersion => string.Empty;
    public DatabaseType[] ForDatabaseTypes => null;

    public string Process(string command)
    {
        return command;
    }
}