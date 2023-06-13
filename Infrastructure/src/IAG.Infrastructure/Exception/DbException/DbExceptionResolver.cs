using Microsoft.Data.Sqlite;

using Npgsql;

namespace IAG.Infrastructure.Exception.DbException;

public class DbExceptionResolver
{
    private readonly System.Exception _ex;

    public DbExceptionResolver(System.Exception ex)
    {
        _ex = ex;
        ResponsibleFactory = GetResponsibleFactory(_ex);
    }

    // public set for unit-testing
    private IDbExceptionFactory ResponsibleFactory { get; }

    public System.Exception TranslateException()
    {
        return ResponsibleFactory == null ? _ex : ResponsibleFactory.GetException(_ex);
    }

    private IDbExceptionFactory GetResponsibleFactory(System.Exception ex)
    {
        switch (ex)
        {
            case PostgresException _:
                return new PgDbExceptionFactory();
            case SqliteException _:
                return new SlDbExceptionFactory();
            default:
                return null;
        }
    }
}