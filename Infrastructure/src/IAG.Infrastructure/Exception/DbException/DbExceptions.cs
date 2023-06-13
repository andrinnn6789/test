using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Resource;

namespace IAG.Infrastructure.Exception.DbException;

public class DbException : LocalizableException
{
    protected DbException(string resourceId, System.Exception innerException, params object[] args) : base(resourceId, innerException, args)
    {
    }
}

public class DbGenericException : DbException
{
    public DbGenericException(System.Exception innerException, params object[] args) : base(ResourceIds.DbGenericExceptionMessage, innerException, args)
    {
    }
}

public class DbUniqueConstraintException : DbException
{
    public DbUniqueConstraintException(System.Exception innerException, params object[] args) : base(ResourceIds.DbUniqueConstraintExceptionMessage, innerException, args)
    {
    }
}

public class DbCannotInsertNullException : DbException
{
    public DbCannotInsertNullException(System.Exception innerException, params object[] args) : base(ResourceIds.DbCannotInsertNullExceptionMessage, innerException, args)
    {
    }
}

public class DbMaxLengthExceededException : DbException
{
    public DbMaxLengthExceededException(System.Exception innerException, params object[] args) : base(ResourceIds.DbMaxLengthExceededExceptionMessage, innerException, args)
    {
    }
}

public class DbNumericOverflowException : DbException
{
    public DbNumericOverflowException(System.Exception innerException, params object[] args) : base(ResourceIds.DbNumericOverflowExceptionMessage, innerException, args)
    {
    }
}

public class DbUndefinedTableException : DbException
{
    public DbUndefinedTableException(System.Exception innerException, params object[] args) : base(ResourceIds.DbUndefinedTableExceptionMessage, innerException, args)
    {
    }
}

public class DbSyntaxErrorException : DbException
{
    public DbSyntaxErrorException(System.Exception innerException, params object[] args) : base(ResourceIds.DbSyntaxErrorMessage, innerException,
        args)
    {
    }
}

[ExcludeFromCodeCoverage]
public class DbCommitWithoutTransaction : DbException
{
    public DbCommitWithoutTransaction() : base(ResourceIds.DbCommitWithoutTransaction, null, null)
    {
    }
}

[ExcludeFromCodeCoverage]
public class DbNoRowUpdated : DbException
{
    public DbNoRowUpdated(int recordsAffected) : base(ResourceIds.DbNoRowUpdated, null, new[] {recordsAffected})
    {
    }
}

[ExcludeFromCodeCoverage]
public class DbNoRowDeleted : DbException
{
    public DbNoRowDeleted(int recordsAffected) : base(ResourceIds.DbNoRowDeleted, null, new[] {recordsAffected})
    {
    }
}