using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.ProcessEngine.Configuration;

namespace IAG.InstallClient.ProcessEngineJob.Inventory;

[ExcludeFromCodeCoverage]
public class InventoryJobConfig : JobConfig<InventoryJob>
{
    public InventoryJobConfig()
    {
        var random = new Random();

        Active = true;
        CronExpression = $"{random.Next(0, 59)} 6 * * *"; // daily at 6 at random minute
    }
}