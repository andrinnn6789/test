using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.ProcessEngine.Store;

using JetBrains.Annotations;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace IAG.ProcessEngine.DataLayer.Settings.Context.Migration.Processor;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class PostProcessorJobName : IPostProcessor
{
    [ExcludeFromCodeCoverage]
    public string ForVersion => "1.2.2";

    [ExcludeFromCodeCoverage]
    public DatabaseType[] ForDatabaseTypes => new[] { DatabaseType.Sqlite };
    
    public void Process(DatabaseFacade database, IServiceProvider serviceProvider)
    {
        // rename existing default-configs to the name of the job-template   
        var jobCatalogue = serviceProvider.GetRequiredService<IJobCatalogue>();
        var templateId = new SqliteParameter("$templateId", SqliteType.Text);
        var paramName = new SqliteParameter("$name", SqliteType.Text);
        foreach (var jobMeta in jobCatalogue.Catalogue)
        {
            templateId.Value = jobMeta.TemplateId;
            paramName.Value = jobMeta.PluginName;
            database.ExecuteSqlRaw(@"
                    UPDATE ConfigProcessEngine SET Name = $name WHERE Id = $templateId;", paramName, templateId);
        }
    }
}