using IAG.IdentityServer.Configuration.Model.Realm;
using IAG.Infrastructure.ImportExport;

namespace IAG.IdentityServer.SeedImportExport;

public interface IRealmSeedImporterExporter : ISeedImporter
{
    object Export(string id, out string fileName);
        
    void ImportRealm(RealmImportExport importData);
}