using System;
using System.Data;

using Npgsql;

namespace IAG.Infrastructure.IntegrationTest.Exception.DbException;

public class PgDbExceptionTestHelper : IDbExceptionTestHelper
{
    private readonly NpgsqlConnection _testDatabaseConnection;
    private readonly string _testDatabaseName;

    public PgDbExceptionTestHelper()
    {
        _testDatabaseName = "Test.Integration";
        _testDatabaseConnection = InitializeTestDatabase(_testDatabaseName);
    }

    public void GenerateUniqueConstraintError()
    {
        var sql = GetTestTableInsertScript(Guid.NewGuid().ToString(), "123", 123);
        ExecuteQuery(_testDatabaseConnection, sql);

        sql = GetTestTableInsertScript(Guid.NewGuid().ToString(), "123", 123);
        ExecuteQuery(_testDatabaseConnection, sql);
    }

    public void GenerateCannotInsertNullConstraintError()
    {
        var id = Guid.NewGuid().ToString();
        var sql = $"INSERT INTO \"TestTable\"(\"Id\") VALUES (\'{id}\');";
        ExecuteQuery(_testDatabaseConnection, sql);
    }
    public void GenerateMaxLengthError()
    {
        var sql = GetTestTableInsertScript(Guid.NewGuid().ToString(), "Lorem ipsum dolor sit amet, consectetur adipiscing elit.", 123);
        ExecuteQuery(_testDatabaseConnection, sql);
    }

    public void GenerateNumericOverflowError()
    {
        var sql = GetTestTableInsertScript(Guid.NewGuid().ToString(), "123", 32768);
        ExecuteQuery(_testDatabaseConnection, sql);
    }

    public void GenerateUndefinedTableError()
    {
        var sql = "SELECT * FROM \"TableBlaBla\"";
        ExecuteQuery(_testDatabaseConnection, sql);
    }

    public void GenerateUnknownError()
    {
        var sql = "SELECT * FROM yyy.xxx.\"TableBlaBla\"";
        ExecuteQuery(_testDatabaseConnection, sql);
    }

    public void GenerateSyntaxError()
    {
        var sql = "SELCT FROM \"TestTable\"";
        ExecuteQuery(_testDatabaseConnection, sql);
    }

    private NpgsqlConnectionStringBuilder SetupEmptyPgAccess()
    {
        return new()
        {
            Host = "postgres",
            Port = 5432,
            Username = "postgres",
            Password = "iagiag",
            Pooling = false
        };
    }

    private void OpenDbConnection(NpgsqlConnection conn)
    {
        if (conn.State != ConnectionState.Open)
            conn.Open();
    }

    private void CloseDbConnection(NpgsqlConnection conn)
    {
        if (conn.State != ConnectionState.Closed)
            conn.Close();
    }

    private NpgsqlConnection InitializeTestDatabase(string dbName)
    {
        DeleteTestDatabase();
        var connStringBuilder = SetupEmptyPgAccess();

        // create empty db
        var createDbSql = $"CREATE DATABASE \"{dbName}\"";
        using (var conn = new NpgsqlConnection(connStringBuilder.ConnectionString))
        {
            try
            {
                ExecuteQuery(conn, createDbSql);
            }
            finally
            {
                CloseDbConnection(conn);
            }
        }

        // create test-table
        connStringBuilder.Database = dbName;
        var createTestTableSql = "CREATE TABLE \"TestTable\" (" +
                                 "\"Id\" uuid NOT NULL," +
                                 "\"StringField\" character varying(32) NOT NULL," +
                                 "\"NumericField\" smallint NOT NULL," +
                                 "CONSTRAINT \"PK_TestTable\" PRIMARY KEY (\"Id\")," +
                                 "UNIQUE (\"StringField\", \"NumericField\"));";
        using (var conn = new NpgsqlConnection(connStringBuilder.ConnectionString))
        {
            try
            {
                ExecuteQuery(conn, createTestTableSql);
            }
            finally
            {
                CloseDbConnection(conn);
            }
        }

        return new NpgsqlConnection(connStringBuilder.ConnectionString);
    }

    private void DeleteTestDatabase()
    {
        var connStringBuilder = SetupEmptyPgAccess();

        var dropDbSql = $"DROP DATABASE IF EXISTS \"{_testDatabaseName}\"";
        using (var conn = new NpgsqlConnection(connStringBuilder.ConnectionString))
        {
            ExecuteQuery(conn, dropDbSql);
        }
    }

    private void ExecuteQuery(NpgsqlConnection conn, string sqlString)
    {
        try
        {
            using (var cmd = new NpgsqlCommand(sqlString, conn))
            {
                OpenDbConnection(conn);
                cmd.ExecuteNonQuery();
            }
        }
        finally
        {
            CloseDbConnection(conn);
        }
    }

    private string GetTestTableInsertScript(string id, string stringTestField, int? numericTestField)
    {
        return "INSERT INTO \"TestTable\"(" +
               "\"Id\", \"StringField\", \"NumericField\")" +
               $"VALUES (\'{id}\', \'{stringTestField}\', {numericTestField});";
    }
}