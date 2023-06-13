using Newtonsoft.Json.Linq;

namespace IAG.Infrastructure.ImportExport;

public interface ISeedImporter
{
    string SeedFilePattern { get; }

    void Import(JObject data);
}