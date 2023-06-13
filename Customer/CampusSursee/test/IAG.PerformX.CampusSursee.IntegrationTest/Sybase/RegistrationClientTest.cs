using System;

using IAG.Common.DataLayerSybase;
using IAG.Common.TestHelper.DataLayerSybase;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.Startup;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.PerformX.CampusSursee.Dto;
using IAG.PerformX.CampusSursee.Dto.Registration;
using IAG.PerformX.CampusSursee.Sybase;

using Xunit;

namespace IAG.PerformX.CampusSursee.IntegrationTest.Sybase;

public class RegistrationClientTest
{
    private readonly RegistrationClient _client;
    private readonly ISybaseConnection _connection;

    public RegistrationClientTest()
    {
        _connection = new SybaseConnectionFactory(
            new ExplicitUserContext("test", null),
            new MockILogger<SybaseConnection>(),
            Startup.BuildConfig(),
            null).CreateConnection();
        _client = new RegistrationClient(_connection);
    }

    [Fact]
    public void AddressChangeTest()
    {
        SybaseTransctionHelper.ExecuteInRollbackTransaction(_connection, () =>
        {
            var item = new RegistrationAddress
            {
                LastName = "test",
                AddressTypeId = 2,
                Language = "de",
                SbvIsMember = (int)SbvMemberEnum.Member
            };
            _client.AddressChange(item);
            Assert.True(item.Id > 0);
            Assert.Equal((int)ChangeTypeEnum.AddressChange, item.EntryType);
        });
    }

    [Fact]
    public void AddressChangeFailTest()
    {
        var item = new RegistrationAddress
        {
            LastName = "test",
            AddressTypeId = 2,
            Language = "xx"
        };
        Assert.Throws<ArgumentException>(() => _client.AddressChange(item));
    }

    [Fact]
    public void AddressNewTest()
    {
        SybaseTransctionHelper.ExecuteInRollbackTransaction(_connection, () =>
        {
            var item = new RegistrationAddress
            {
                LastName = "test",
                AddressTypeId = 2,
                Language = "de",
                SbvIsMember = 10,
                UserName = Guid.NewGuid().ToString()
            };
            _client.AddressNew(item);
            Assert.True(item.Id > 0);
            Assert.Equal((int)ChangeTypeEnum.OrderAddress, item.EntryType);
        });
    }

    [Fact]
    public void AddressNewFailTest()
    {
        var item = new RegistrationAddress
        {
            LastName = "test",
            AddressTypeId = 2,
            Language = "xx",
            UserName = Guid.NewGuid().ToString()
        };
        Assert.Throws<ArgumentException>(() => _client.AddressNew(item));

        item = new RegistrationAddress
        {
            LastName = "test",
            AddressTypeId = 2,
            Language = "de",
            UserName = Guid.NewGuid().ToString()
        };
        Assert.Throws<ArgumentException>(() => _client.AddressNew(item));
    }

    [Fact]
    public void AddressNewUsernameOnlineDuplicateTest()
    {
        SybaseTransctionHelper.ExecuteInRollbackTransaction(_connection, () =>
        {
            var item = new RegistrationAddress
            {
                LastName = "test",
                AddressTypeId = 2,
                Language = "de",
                SbvIsMember = 10,
                UserName = "it's me from test"
            };
            _client.AddressNew(item);
            _client.AddressNew(item);
        });
    }

    [Fact]
    public void AddressNewUsernameAddressFailTest()
    {
        const string userName = "it's me from test";
        SybaseTransctionHelper.ExecuteInRollbackTransaction(_connection, () =>
        {
            var cmd = _connection.CreateCommand(@"
                INSERT INTO Adresse (
                    Adr_OnlineBenutzername, Adr_OnlineAktiv, Adr_AnrId, Adr_LandID, Adr_Sprache, Adr_MWSTVerrechnung, 
                    Adr_AufnahmeartID, Adr_KGrpPreisID
                ) 
                SELECT TOP 1 ?, -1, 1, 1, 'DE', -1, 1, KundPreis_ID 
                FROM KundengruppePreis", userName);
            cmd.ExecuteNonQuery();
            var item = new RegistrationAddress
            {
                LastName = "test",
                AddressTypeId = 2,
                Language = "de",
                UserName = userName
            };
            Assert.Throws<DuplicateKeyException>(() => _client.AddressNew(item));
        });
    }

    [Fact]
    public void RegistrationTest()
    {
        SybaseTransctionHelper.ExecuteInRollbackTransaction(_connection, () =>
        {
            var item = new RegistrationPending
            {
                EventId = 123
            };
            _client.RegistrationNew(item);
            Assert.True(item.Id > 0);
        });
    }
}