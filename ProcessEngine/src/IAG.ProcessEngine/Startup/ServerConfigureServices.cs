using System.Composition;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.ProcessEngine.Configuration;
using IAG.Infrastructure.Settings;
using IAG.Infrastructure.Startup;
using IAG.ProcessEngine.DataLayer.Settings.Context;
using IAG.ProcessEngine.DataLayer.State.Context;
using IAG.ProcessEngine.Execution;
using IAG.ProcessEngine.Execution.Condition;
using IAG.ProcessEngine.Provider;
using IAG.ProcessEngine.Store;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IAG.ProcessEngine.Startup;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Export(typeof(IConfigureServices))]
public class ServerConfigureServices : IConfigureServices
{
    private const string ProcessEngine = "ProcessEngine";
    private const string ProcessEngineState = "ProcessEngine.State";

    public void ConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        var dbOptionsConfig = services.GetDbOption(ProcessEngine, SettingsConst.SettingsDb);
        var dbOptionsState = services.GetDbOption(ProcessEngineState, $"{ProcessEngineState}.db");

        services.AddTransient<IPluginLoader, PluginLoader>();
        services.AddDbContext<SettingsDbContext>(dbOptionsConfig);
        services.AddDbContext<ProcessEngineStateDbContext>(dbOptionsState);
        services.AddTransient<IJobConfigStore, JobConfigStoreDb>();
        services.AddTransient<IJobDataStore, JobDataStoreDb>();
        services.AddSingleton<IJobCatalogue, JobCatalogue>();
        services.AddTransient<IJobStore, JobStoreDb>();
        services.AddTransient<IJobBuilder, JobBuilder>();
        services.AddSingleton<IJobExecuter, JobExecuter>();
        services.AddTransient<IJobService, JobService>();
        services.AddSingleton<IScheduler, Scheduler>();
        services.AddSingleton<IMonitor, Monitor>();
        services.AddSingleton<IServiceLifetime, ServiceLifetime>();
        services.AddTransient<IConditionChecker, ConditionChecker>();
        services.AddTransient<IConditionParser, ConditionParser>();
        services.AddTransient<IJobConfigProvider, JobConfigProvider>();

        var builder = services.BuildServiceProviderForExplicitUser(SchemaMigrator.MigratorUser);
        using (var scope = builder.CreateScope())
        {
            var settingsDbContext = scope.ServiceProvider.GetRequiredService<SettingsDbContext>();
            var stateDbContext = scope.ServiceProvider.GetRequiredService<ProcessEngineStateDbContext>();
            var migrator = new SchemaMigrator(scope.ServiceProvider);
            migrator.DoMigration(settingsDbContext);
            migrator.DoMigration(stateDbContext);
        }

        builder = services.BuildServiceProvider();
        using (var scope = builder.CreateScope())
        {
            var jobCatalogue = scope.ServiceProvider.GetRequiredService<IJobCatalogue>();
            var logger = scope.ServiceProvider.GetService<ILogger<ServerConfigureServices>>();
            var configStore = scope.ServiceProvider.GetRequiredService<IJobConfigStore>();
            foreach (var jobMeta in jobCatalogue.Catalogue)
            {
                logger?.LogInformation("Registering process-plugin " + jobMeta.PluginName);
                services.AddTransient(jobMeta.JobType);
                if (jobMeta.AutoActivate)
                {
                    configStore.EnsureConfig(jobMeta.TemplateId, jobMeta.ConfigType);
                }
            }
            UpdateConfigs(configStore);
        }
        
        ServiceProviderHolder.ServiceProvider = builder;
    }

    /// <summary>
    /// Inserts new properties and removes stale ones
    /// </summary>
    /// <param name="store"></param>
    private void UpdateConfigs(IJobConfigStore store)
    {
        foreach (var config in store.GetAllUnprocessed())
        {
            store.Update(config);
        }
    }
}