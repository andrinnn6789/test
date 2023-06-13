using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;

using JetBrains.Annotations;

namespace IAG.Infrastructure.IntegrationTest.DataLayer.Migration.TestContext.MigrationScripts;

[UsedImplicitly]
public class PreProcessorSql23Replacer: IPreProcessorSql
{
    public static string VersionSetter = "1.1.0";

    public string ForVersion => VersionSetter;
    public DatabaseType[] ForDatabaseTypes => new[] { DatabaseType.Sqlite };

    public string Process(string command)
    {
        return command.Replace("{ReplaceMeWith23}", "23");
    }
}