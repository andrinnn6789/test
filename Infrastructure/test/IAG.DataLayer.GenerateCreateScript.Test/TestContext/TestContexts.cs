using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.IdentityServer.Authentication;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace IAG.DataLayer.GenerateCreateScript.Test.TestContext;

public class TestADbContext : BaseDbContext
{
    public DbSet<TestEntryA> AEntries { get; set; }

    public TestADbContext(DbContextOptions options) : base(options, new ExplicitUserContext("test", null))
    {
    }
}

public class TestBDbContext : TestADbContext
{
    public DbSet<TestEntryB> BEntries { get; set; }

    public TestBDbContext(DbContextOptions<TestBDbContext> options) : base(options)
    {
    }
}

public class TestUnsupportedDbContext : TestADbContext
{
    [UsedImplicitly]
    // ReSharper disable once UnusedTypeParameter
    public class DummyDbContextOptions<T> : DbContextOptions<TestUnsupportedDbContext> { }

    [UsedImplicitly]
    public TestUnsupportedDbContext(string whatever) : base(null)
    {
    }

    [UsedImplicitly]
    public TestUnsupportedDbContext(DummyDbContextOptions<TestBDbContext> options) : base(options)
    {
    }

    [UsedImplicitly]
    public TestUnsupportedDbContext(DbContextOptions<TestBDbContext> options, string anotherDummyConstructorDoesNotMatter) : base(options)
    {
    }
}