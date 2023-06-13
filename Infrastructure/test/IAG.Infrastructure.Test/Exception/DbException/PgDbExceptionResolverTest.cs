using Xunit;
using IAG.Infrastructure.Exception.DbException;

namespace IAG.Infrastructure.Test.Exception.DbException;

public class PgDbExceptionResolverTest
{
    [Fact]
    public void UnknownExceptionErrorTypeTest()
    {
        var ex = new System.Exception("Test");
        var errorType = new PgDbExceptionFactory().GetException(ex);

        Assert.Null(errorType);
    }
}