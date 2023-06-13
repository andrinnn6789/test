using System.Composition;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DI;
using IAG.VinX.IAG.ControlCenter.Common;
using IAG.VinX.IAG.JiraVinXSync.ComponentSync.BusinessLogic;
using IAG.VinX.IAG.JiraVinXSync.ComponentSync.DataAccess.VinX;
using IAG.VinX.IAG.JiraVinXSync.IssueSync.BusinessLogic;
using IAG.VinX.IAG.JiraVinXSync.IssueSync.DataAccess.VinX;
using IAG.VinX.IAG.JiraVinXSync.VersionSync.BusinessLogic;
using IAG.VinX.IAG.JiraVinXSync.VersionSync.DataAccess.VinX;
using IAG.VinX.IAG.JiraVinXSync.WorklogSync.BusinessLogic;
using IAG.VinX.IAG.JiraVinXSync.WorklogSync.DataAccess.VinX;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IAG.VinX.IAG.Startup;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Export(typeof(IConfigureServices))]
public class ApplicationConfigureServices : IConfigureServices
{
    public void ConfigureServices(IServiceCollection services, IHostEnvironment env)
    {
        services.AddScoped<IControlCenterTokenRequest, ControlCenterTokenRequest>();
        services.AddScoped<IWorklogSyncer, WorklogSyncer>();
        services.AddSingleton<IWorklogSyncVinXConnector, WorklogSyncVinXConnector>();
        services.AddScoped<IIssueSyncer, IssueSyncer>();
        services.AddSingleton<IIssueSyncVinXConnector, IssueSyncVinXConnector>();
        services.AddScoped<IVersionSyncer, VersionSyncer>();
        services.AddSingleton<IVersionSyncVinXConnector, VersionSyncVinXConnector>();
        services.AddScoped<IComponentSyncer, ComponentSyncer>();
        services.AddSingleton<IComponentSyncVinXConnector, ComponentSyncVinXConnector>();
    }
}