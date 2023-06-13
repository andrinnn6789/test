using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.ImportExport;

using Newtonsoft.Json.Linq;

namespace IAG.IdentityServer.Configuration.Model.Realm;

[ExcludeFromCodeCoverage]
public class RealmImportExport : ImportExportData
{
    public RealmConfig RealmConfig { get; set; }

    public JObject AuthenticationPluginData { get; set; }
}