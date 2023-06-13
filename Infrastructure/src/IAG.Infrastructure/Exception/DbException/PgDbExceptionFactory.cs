using Npgsql;

namespace IAG.Infrastructure.Exception.DbException;

public class PgDbExceptionFactory : IDbExceptionFactory
{
    public LocalizableException GetException(System.Exception ex)
    {
        var pgEx = ex as PostgresException;
        if (pgEx == null)
            return null;
        switch (pgEx.SqlState)
        {
            case "22001":
                return new DbMaxLengthExceededException(ex);
            case "23505":
                return new DbUniqueConstraintException(ex);
            case "23502":
                return new DbCannotInsertNullException(ex);
            case "22003":
                return new DbNumericOverflowException(ex);
            case "42P01":
                return new DbUndefinedTableException(ex);
            case "42601":
                return new DbSyntaxErrorException(ex);
            default:
                return new DbGenericException(ex);
        }
    }
}