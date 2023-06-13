namespace IAG.Infrastructure.IntegrationTest.Exception.DbException;

public interface IDbExceptionTestHelper
{
    void GenerateUniqueConstraintError();
    void GenerateCannotInsertNullConstraintError();
    void GenerateMaxLengthError();
    void GenerateNumericOverflowError();
    void GenerateUndefinedTableError();
    void GenerateUnknownError();
    void GenerateSyntaxError();
}