using System.Diagnostics.CodeAnalysis;
using System.IO;

using IAG.Infrastructure.DI;
using IAG.Infrastructure.ImportExport;
using IAG.Infrastructure.Settings;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

namespace IAG.Infrastructure.Startup.Extensions;

[ExcludeFromCodeCoverage]
public static class SeedImportExtensions
{
    public static IApplicationBuilder DoSeedImport(this IApplicationBuilder appBuilder)
    {
        using var scope = appBuilder.ApplicationServices.BuildScopeForExplicitUser("SeedImporter");
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
            var settingsPath = new SettingsFinder().GetSettingsPath();
            foreach (var seedImporter in scope.ServiceProvider.GetServices<ISeedImporter>())
            {
                foreach (var seedFile in Directory.EnumerateFiles(settingsPath, seedImporter.SeedFilePattern))
                {
                    logger.LogInformation("Import seed file '{seedFile}'", seedFile);
                    var seedContent = File.ReadAllText(seedFile);
                    seedImporter.Import(JObject.Parse(seedContent));
                    File.Delete(seedFile);
                }
            }
        }

        return appBuilder;
    }
}