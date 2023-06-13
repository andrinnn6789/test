using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Rest;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IAG.Infrastructure.Startup.Extensions;

[ExcludeFromCodeCoverage]
public static class MvcExtensions
{
    public static IServiceCollection AddIagMvc(this IServiceCollection services, IConfiguration configuration, IPluginLoader pluginLoader)
    {
        var mvcBuilder = services
            .AddControllers(options =>
            {
                options.EnableEndpointRouting = false;
                options.Conventions.Add(new ApiExplorerDefaultGroupConvention(ApiExplorerDefaults.DefaultGroup));
            })
            .AddOData(opt => opt
                .AddRouteComponents("odata", GetEdmModel(pluginLoader))
                .Select()
                .Expand()
                .Filter()
                .OrderBy()
                .SetMaxTop(1000)
                .Count()
            );

        mvcBuilder.AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            options.SerializerSettings.ContractResolver = new JsonContractResolver(services.BuildServiceProvider().GetService<IHttpContextAccessor>);
        });
        mvcBuilder.AddIagServerAuthorization(configuration);

        foreach (Assembly pluginAssembly in pluginLoader.GetAssemblies())
        {
            mvcBuilder.AddApplicationPart(pluginAssembly);
        }

        return services;
    }

    public static IApplicationBuilder UseIagMvc(this IApplicationBuilder appBuilder)
    {
        appBuilder.UseMvc();
        return appBuilder;
    }

    private static IEdmModel GetEdmModel(IPluginLoader pluginLoader)
    {
        var builder = new ODataConventionModelBuilder();
        foreach (var plugin in pluginLoader.GetExports<IEdmModelBuilder>())
        {
            plugin.AddModels(builder);
        }
        return builder.GetEdmModel();
    }
}