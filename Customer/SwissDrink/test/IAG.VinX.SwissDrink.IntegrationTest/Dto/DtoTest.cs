using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.SwissDrink.Dto;

using Xunit;

namespace IAG.VinX.SwissDrink.IntegrationTest.Dto;

public class DtoTest
{
    private readonly ISybaseConnection _connection;

    public DtoTest()
    {
        _connection = new SybaseConnectionFactory(
            new ExplicitUserContext("test", null),
            new MockILogger<SybaseConnection>(),
            Infrastructure.Startup.Startup.BuildConfig(),
            null).CreateConnection();
    }

    [Fact]
    public void OpData()
    {
        _ = _connection.GetQueryable<OpData>().FirstOrDefault();
    }
}