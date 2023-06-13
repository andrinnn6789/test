using IAG.Infrastructure.DataLayer.Context;

namespace IAG.Infrastructure.DataLayer.Migration;

public interface IProcessor
{
    string ForVersion { get; }

    DatabaseType[] ForDatabaseTypes { get; }
}