using System;
using System.Collections.Generic;

using IAG.IdentityServer.Configuration.Model.Realm;

namespace IAG.IdentityServer.Configuration;

public interface IRealmConfigStore
{
    IEnumerable<IRealmConfig> GetAll();

    IRealmConfig LoadConfig(IRealmConfig config, Type configType);

    IRealmConfig Read(string realm, Type configType);

    void Insert(IRealmConfig config);

    void Update(IRealmConfig config);

    void Delete(string realm);
}