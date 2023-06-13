using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.EntityFrameworkCore;

namespace IAG.Infrastructure.Test.DataLayer.Migration.TestContext;

public class TestProjectDbContext : TestProjectImplDbContext
{
    public TestProjectDbContext(DbContextOptions options) : base(options)
    {
    }
}

[ContextInfo("TestProject", typeof(TestProjectDbContext))]
public abstract class TestProjectImplDbContext : BaseDbContext
{
    public TestProjectImplDbContext(DbContextOptions options) : base(options, new ExplicitUserContext("test", null))
    {
    }
}

[ContextInfo("MySpecificTestProject")]
public class SpecificTestProjectDbContext : TestProjectImplDbContext
{
    public SpecificTestProjectDbContext(DbContextOptions options) : base(options)
    {
    }
}


public class TestWithoutAttributeDbContext : BaseDbContext
{
    public TestWithoutAttributeDbContext(DbContextOptions options) : base(options, new ExplicitUserContext("test", null))
    {
    }
}

[ContextInfo("")]
public class TestWithEmptyDbContext : BaseDbContext
{
    public TestWithEmptyDbContext(DbContextOptions options) : base(options, new ExplicitUserContext("test", null))
    {
    }
}