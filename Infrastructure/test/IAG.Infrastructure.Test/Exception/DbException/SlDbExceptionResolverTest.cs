using Xunit;
using IAG.Infrastructure.Exception.DbException;

namespace IAG.Infrastructure.Test.Exception.DbException;

public class SlDbExceptionResolverTest
{
    [Fact]
    public void UnknownExceptionErrorTypeTest()
    {
        var ex = new System.Exception("Test");
        var errorType = new SlDbExceptionFactory().GetException(ex);

        Assert.Null(errorType);
    }
}