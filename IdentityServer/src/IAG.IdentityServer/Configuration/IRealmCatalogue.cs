using System.Collections.Generic;

using IAG.IdentityServer.Configuration.Model.Realm;

namespace IAG.IdentityServer.Configuration;

public interface IRealmCatalogue
{
    List<IRealmConfig> Realms { get; }

    IRealmConfig GetRealm(string realm);

    void Save(IRealmConfig config);

    void Delete(string realm);

    void Reload();
}