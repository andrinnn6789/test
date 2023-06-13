using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Test.DataLayer.Migration.TestDummy;

[UsedImplicitly]
public class WrongLocatedPreProcessorSql: IPreProcessorSql
{
    public string ForVersion => string.Empty;
    public DatabaseType[] ForDatabaseTypes => null;

    public string Process(string command)
    {
        return command;
    }
}