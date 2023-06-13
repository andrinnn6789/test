using System;
using System.Collections;
using System.Collections.Generic;

using IAG.Infrastructure.Exception.DbException;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.Exception.DbException;

public class DbExceptionTestHelperData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] {new SlDbExceptionTestHelper()};
        yield return new object[] {new PgDbExceptionTestHelper()};
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class DbExceptionResolverTest
{

    [Theory]
    [ClassData(typeof(DbExceptionTestHelperData))]
    public void AnalyzeUniqueConstraintTest(IDbExceptionTestHelper testHelper)
    {
        System.Exception dbEx = null;
        try
        {
            testHelper.GenerateUniqueConstraintError();
        }
        catch (System.Exception ex)
        {
            dbEx = new DbExceptionResolver(ex).TranslateException();
        }

        Assert.IsType<DbUniqueConstraintException>(dbEx);
    }

    [Theory]
    [ClassData(typeof(DbExceptionTestHelperData))]
    public void AnalyzeCannotInsertNullConstraintTest(IDbExceptionTestHelper testHelper)
    {
        System.Exception dbEx = null;
        try
        {
            testHelper.GenerateCannotInsertNullConstraintError();
        }
        catch (System.Exception ex)
        {
            dbEx = new DbExceptionResolver(ex).TranslateException();
        }

        Assert.IsType<DbCannotInsertNullException>(dbEx);
    }

    [Theory]
    [ClassData(typeof(DbExceptionTestHelperData))]
    // SQLite has no limit
    public void AnalyzeMaxLengthTest(IDbExceptionTestHelper testHelper)
    {
        try
        {
            testHelper.GenerateMaxLengthError();
        }
        catch (System.Exception ex)
        {
            var dbEx = new DbExceptionResolver(ex).TranslateException();
            Assert.IsType<DbMaxLengthExceededException>(dbEx);
        }
    }

    [Theory]
    [ClassData(typeof(DbExceptionTestHelperData))]
    // SQLite has no limit
    public void AnalyzeNumericOverflowTest(IDbExceptionTestHelper testHelper)
    {
        try
        {
            testHelper.GenerateNumericOverflowError();
        }
        catch (System.Exception ex)
        {
            var dbEx = new DbExceptionResolver(ex).TranslateException();
            Assert.IsType<DbNumericOverflowException>(dbEx);
        }
    }

    [Theory]
    [ClassData(typeof(DbExceptionTestHelperData))]
    public void AnalyzeUndefinedTableTest(IDbExceptionTestHelper testHelper)
    {
        System.Exception dbEx = null;
        try
        {
            testHelper.GenerateUndefinedTableError();
        }
        catch (System.Exception ex)
        {
            dbEx = new DbExceptionResolver(ex).TranslateException();
        }

        Assert.IsType<DbUndefinedTableException>(dbEx);
    }


    [Theory]
    [ClassData(typeof(DbExceptionTestHelperData))]
    public void AnalyzeUnknownErrorTest(IDbExceptionTestHelper testHelper)
    {
        System.Exception dbEx = null;
        try
        {
            testHelper.GenerateUnknownError();
        }
        catch (System.Exception ex)
        {
            dbEx = new DbExceptionResolver(ex).TranslateException();
        }

        Assert.IsType<DbGenericException>(dbEx);
    }

    [Theory]
    [ClassData(typeof(DbExceptionTestHelperData))]
    public void AnalyzeSyntaxErrorTest(IDbExceptionTestHelper testHelper)
    {
        System.Exception dbEx = null;
        try
        {
            testHelper.GenerateSyntaxError();
        }
        catch (System.Exception ex)
        {
            dbEx = new DbExceptionResolver(ex).TranslateException();
        }

        Assert.True(dbEx is DbGenericException || dbEx is DbSyntaxErrorException);
    }

    [Theory]
    [ClassData(typeof(DbExceptionTestHelperData))]
#pragma warning disable xUnit1026
    public void AnalyzeNoDbErrorTest(IDbExceptionTestHelper testHelper)
    {
        var dbEx = new DbExceptionResolver(new ArrayTypeMismatchException()).TranslateException();

        Assert.IsType<ArrayTypeMismatchException>(dbEx);
    }
#pragma warning restore xUnit1026
}