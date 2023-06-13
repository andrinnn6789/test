using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.Startup;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.PerformX.ibW.Dto.Web;

using Xunit;

namespace IAG.PerformX.ibW.IntergrationsTest.Dto.Web;

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
    public void AngebotTest()
    {
        var items = _connection.GetQueryable<Angebot>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void AngebotZusatzTest()
    {
        var items = _connection.GetQueryable<AngebotZusatz>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void AngebotECommerceTest()
    {
        var items = _connection.GetQueryable<AngebotECommerce>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void ECommerceTest()
    {
        var items = _connection.GetQueryable<ECommerce>().Take(1).ToList();
        Assert.NotEmpty(items);
    }
}