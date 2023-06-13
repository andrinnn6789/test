using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.Startup;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.PerformX.ibW.Dto.Web;
using IAG.PerformX.ibW.Sybase;

using Xunit;

namespace IAG.PerformX.ibW.IntergrationsTest.Sybase;

public class WebClientTest
{
    private readonly ISybaseConnection _connection;

    public WebClientTest()
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
        var items = _connection.GetQueryable<Angebot>().Where(a => a.MaxTeilnehmer > 50).Take(10);
        var angebote = new WebClient(_connection).AddSubLinks(items);
        Assert.NotEmpty(angebote);
    }
}