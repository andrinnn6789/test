using System;
using System.ComponentModel.DataAnnotations;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;


namespace IAG.Infrastructure.IntegrationTest.Exception.DbException;

public class TestData
{
    [Key]
    [UsedImplicitly]
    public string Id { get; set; }

    [MaxLength(5)]
    [Required]
    [UsedImplicitly]
    public string StringField { get; set; }

    [MaxLength(4)]
    [UsedImplicitly]
    public int? NumericField { get; set; }
}

public class TestContext : DbContext
{
    private readonly string _dbName;

    [UsedImplicitly]
    public DbSet<TestData> TestDatas { get; set; }

    public TestContext(string dbName)
    {
        _dbName = dbName;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=" + _dbName);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TestData>()
            .HasAlternateKey(k => k.StringField);
    }
}

public class SlDbExceptionTestHelper : IDbExceptionTestHelper
{
    private readonly TestContext _testContext;

    public SlDbExceptionTestHelper()
    {
        _testContext = new TestContext("DbExceptiontest.db");
        _testContext.Database.EnsureDeleted();
        _testContext.Database.EnsureCreated();
    }

    public void GenerateUniqueConstraintError()
    {
        _testContext.Database. ExecuteSqlRaw(GetInsertScript("1", "1"));
        _testContext.Database. ExecuteSqlRaw(GetInsertScript("1", "2"));
    }

    public void GenerateCannotInsertNullConstraintError()
    {
        _testContext.Database. ExecuteSqlRaw("INSERT INTO TestDatas(Id) VALUES ('3')");
    }

    public void GenerateMaxLengthError()
    {
        // does not work
        _testContext.Database. ExecuteSqlRaw(GetInsertScript("sldköfslödkfösldkfjlsdf", "1"));
    }

    public void GenerateNumericOverflowError()
    {
        // does not work
        _testContext.Database. ExecuteSqlRaw(GetInsertScript("4", "999999999999999999999999999999999999999999"));
    }

    public void GenerateUndefinedTableError()
    {
        _testContext.Database. ExecuteSqlRaw("SELECT * FROM TableBlaBl");
    }

    public void GenerateUnknownError()
    {
        var testContext = new TestContext("\\");
        testContext.Database.EnsureCreated();
    }

    public void GenerateSyntaxError()
    {
        _testContext.Database. ExecuteSqlRaw("SELECT FROM TableBlaBl");
    }

    private string GetInsertScript(string stringTestField, string numericTestField)
    {
        var id = Guid.NewGuid().ToString();
        return $"INSERT INTO TestDatas (Id, StringField, NumericField) VALUES (\'{id}\', \'{stringTestField}\', {numericTestField});";
    }
}