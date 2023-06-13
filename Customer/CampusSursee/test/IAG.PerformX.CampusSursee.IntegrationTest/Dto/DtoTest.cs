using System.IO;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.DataLayerSybase;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.Startup;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.PerformX.CampusSursee.Dto.Address;
using IAG.PerformX.CampusSursee.Dto.Event;
using IAG.PerformX.CampusSursee.Dto.Lookup;
using IAG.PerformX.CampusSursee.Dto.Registration;

using Xunit;

namespace IAG.PerformX.CampusSursee.IntegrationTest.Dto;

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
    public void AddressTest()
    {
        var items = _connection.GetQueryable<Address>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void DocumentTest()
    {
        var items = _connection.GetQueryable<Document>().Take(1).ToList();
        Assert.NotEmpty(items);
        Assert.True(items[0].AtlasId > 0);
    }

    [Fact]
    public void RelationTest()
    {
        var items = _connection.GetQueryable<Relation>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void ChangeTest()
    {
        SybaseTransctionHelper.ExecuteInRollbackTransaction(_connection, () =>
        {
            var item = new RegistrationAddress
            {
                LastName = "test",
                AddressTypeId = 2,
                Foto = File.ReadAllBytes(Path.Combine("Dto", "Capture.jpg"))
            };
            _connection.Insert(item);
            Assert.True(item.Id > 0);
        });
    }

    [Fact]
    public void RegistrationInsertTest()
    {
        SybaseTransctionHelper.ExecuteInRollbackTransaction(_connection, () =>
        {
            var item = new RegistrationPending
            {
                EventId= 123
            };
            _connection.Insert(item);
            Assert.True(item.Id > 0);
        });
    }

    [Fact]
    public void RegistrationTest()
    {
        var items = _connection.GetQueryable<Registration>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void EventTest()
    {
        var items = _connection.GetQueryable<Event>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void OccurenceTest()
    {
        var items = _connection.GetQueryable<Occurence>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void AdditionalTest()
    {
        var items = _connection.GetQueryable<Additional>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void EventModuleTest()
    {
        var items = _connection.GetQueryable<EventModule>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void SalutationTest()
    {
        var items = _connection.GetQueryable<Salutation>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void RegistrationStatusTest()
    {
        var items = _connection.GetQueryable<RegistrationStatus>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void EventStatusTest()
    {
        var items = _connection.GetQueryable<EventStatus>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void EventKindTest()
    {
        var items = _connection.GetQueryable<EventKind>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void AddressRelationKindTest()
    {
        var items = _connection.GetQueryable<AddressRelationKind>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void CommunicationChannelTest()
    {
        var items = _connection.GetQueryable<CommunicationChannel>().Take(1).ToList();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void CountryTest()
    {
        var items = _connection.GetQueryable<Country>().Take(1).ToList();
        Assert.NotEmpty(items);
    }
}