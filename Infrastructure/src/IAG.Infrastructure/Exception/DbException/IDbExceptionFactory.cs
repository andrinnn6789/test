namespace IAG.Infrastructure.Exception.DbException;

public interface IDbExceptionFactory
{
    LocalizableException GetException(System.Exception ex);
}