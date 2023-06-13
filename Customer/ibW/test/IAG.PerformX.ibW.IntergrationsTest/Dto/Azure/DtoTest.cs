using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.Startup;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.PerformX.ibW.Dto.Azure;

using Xunit;

namespace IAG.PerformX.ibW.IntergrationsTest.Dto.Azure;

public class DtoTest
{
    private readonly ISybaseConnection _connection;

    public DtoTest()
    {
        _connection = new SybaseConnectionFactory(
            new ExplicitUserContext("test", null),
            new MockILogger<SybaseConnection>(),
            Startup.BuildConfig(),
            null).CreateConnection();
    }

    [Fact]
    public void AdminUnitTest()
    {
        var items = _connection.GetQueryable<AdminUnit>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void GroupTest()
    {
        var items = _connection.GetQueryable<Group>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void GroupRelationTest()
    {
        var items = _connection.GetQueryable<GroupRelation>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void MemberTest()
    {
        var items = _connection.GetQueryable<Member>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void OwnerTest()
    {
        var items = _connection.GetQueryable<Owner>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void PersonTest()
    {
        var items = _connection.GetQueryable<Person>().Take(1).ToList();
        Assert.NotEmpty(items);
    }
}