using Microsoft.Data.Sqlite;

namespace IAG.Infrastructure.Exception.DbException;

public class SlDbExceptionFactory : IDbExceptionFactory
{
    public LocalizableException GetException(System.Exception ex)
    {
        var slEx = ex as SqliteException;
        if (slEx == null)
            return null;
        switch (slEx.SqliteExtendedErrorCode)
        {
            case 2067:
                return new DbUniqueConstraintException(ex);
            case 1299:
                return new DbCannotInsertNullException(ex);
            case 1:
                if (slEx.Message.Contains("no such table"))
                    return new DbUndefinedTableException(ex);
                return new DbGenericException(ex);
            default:
                return new DbGenericException(ex);
        }
    }
}