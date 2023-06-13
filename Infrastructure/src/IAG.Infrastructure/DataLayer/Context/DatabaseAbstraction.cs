using System;
using System.Data;
using System.Linq;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IAG.Infrastructure.DataLayer.Context;

public class DatabaseAbstraction
{
    private readonly ValueConverter<byte[], long> _postgresRowVersionConverter;

    public DatabaseType DbType { get; }

    public DatabaseAbstraction(string dbProvider)
    {
        DbType = GetDbType(dbProvider);
        if (DbType == DatabaseType.Postgres)
        {
            _postgresRowVersionConverter = new ValueConverter<byte[], long>(
                v => BitConverter.ToInt64(v, 0),
                v => BitConverter.GetBytes(v));
        }
    }

    public static DatabaseType GetDbType(string dbProvider)
    {
        if (string.IsNullOrWhiteSpace(dbProvider) || dbProvider.Contains("sqlite", StringComparison.OrdinalIgnoreCase))
        {
            return DatabaseType.Sqlite;
        }
        if (dbProvider.Contains("postgreSQL", StringComparison.OrdinalIgnoreCase))
        {
            return DatabaseType.Postgres;
        }
        if (dbProvider.Contains("sqlServer", StringComparison.OrdinalIgnoreCase))
        {
            return DatabaseType.MsSql;
        }
        if (dbProvider.Contains("inMemory", StringComparison.OrdinalIgnoreCase))
        {
            return DatabaseType.Memory;
        }
        if (dbProvider.Equals("memory_shared", StringComparison.OrdinalIgnoreCase))
        {
            return DatabaseType.MemoryShared;
        }

        throw new System.Exception($"Unknown provider received: {dbProvider}");
    }

    public static IDbDataParameter GetDbParameter(string providerName, string name, object value)
    {
        if (providerName.Contains("sqlite", StringComparison.OrdinalIgnoreCase))
        {
            return new SqliteParameter(name, value);
        }

        throw new System.Exception($"Unknown provider received: {providerName}");
    }

    public string SqlDefaultValueCurrentTimestamp
    {
        get
        {
            switch (DbType)
            {
                case DatabaseType.Postgres: return "CURRENT_TIMESTAMP";
                case DatabaseType.Sqlite: return "strftime('%Y-%m-%d %H:%M:%f', 'now')";
                default: return "GETUTCDATE()";
            }
        }
    }

    public static bool TableExists(DatabaseFacade database, string tableName, DatabaseType dbType)
    {
        var sql = string.Empty;
        switch (dbType)
        {
            case DatabaseType.Postgres:
                sql = @$"
                        SELECT COUNT(1) FROM information_schema.tables 
                        WHERE table_schema = current_schema AND table_name = '{tableName}'
                        ";
                break;
            case DatabaseType.Sqlite:
                sql = @$"
                        SELECT count(1) FROM sqlite_master  
                        WHERE  type='table' AND name='{tableName}'
                        ";
                break;
        }
        if (string.IsNullOrWhiteSpace(sql))
            return true;

        var conn = database.GetDbConnection();
        if (conn.State.Equals(ConnectionState.Closed))
            conn.Open();
        using var command = conn.CreateCommand();
        command.CommandText = sql;
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public void AddDbEntitySpecificStuff(IMutableEntityType entityType)
    {
        if (DbType == DatabaseType.Postgres)
        {
            var rowVersionProperty = entityType.GetProperties().FirstOrDefault(p => p.IsConcurrencyToken);
            if (rowVersionProperty != null)
            {
                rowVersionProperty.SetColumnName("xmin");
                rowVersionProperty.SetColumnType("xid");
                rowVersionProperty.SetValueConverter(_postgresRowVersionConverter);
            }
        }

        // add other DB specific entity related stuff if needed....
    }

    public void AddDbSpecificStuff(ModelBuilder modelBuilder)
    {
        // add DB specific database related stuff here if needed....
    }
}