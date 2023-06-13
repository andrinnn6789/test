using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using IAG.Infrastructure.DI;
using IAG.Infrastructure.Swagger;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace IAG.Infrastructure.Startup.Extensions.Swagger;

[ExcludeFromCodeCoverage]
public static class SwaggerExtensions
{
    private static readonly List<SwaggerEndpointDefinition> SwaggerEndpointDefinitions = new();
    private static readonly Regex VersionRegex = new Regex("^v\\d+\\.\\d+");

    public static IServiceCollection AddIagSwaggerDocs(this IServiceCollection services, PluginLoader pluginLoader)
    {
        FindApis(pluginLoader);

        services
            .AddSwaggerGen(options =>
            {
                AddApis(options);
                AddSecurity(options);
                OrderApis(options);
                TagApis(options);
                ApplyOperationFilters(options);
                AddXmlComments(options);
            })
            .AddSwaggerGenNewtonsoftSupport();
        return services;
    }

    private static void FindApis(PluginLoader pluginLoader)
    {
        SwaggerEndpointDefinitions.Clear();
        foreach (var providerType in pluginLoader.GetImplementations<ISwaggerEndpointProvider>().ToList())
        {
            var provider = (ISwaggerEndpointProvider) Activator.CreateInstance(providerType);
            if (provider == null)
                throw new System.Exception($"cannot instantiate {providerType.FullName}");

            foreach (var definition in provider.EndpointDefinitions)
            {
                SwaggerEndpointDefinitions.Add(definition);
            }
        }
    }

    private static void AddApis(SwaggerGenOptions options)
    {
        options.CustomSchemaIds(x => x
                                         .GetCustomAttributes(false)
                                         .OfType<DisplayNameAttribute>()
                                         .FirstOrDefault()?.DisplayName 
                                     ?? x.Name);
        foreach (var definition in SwaggerEndpointDefinitions)
        {
            options.SwaggerDoc(definition.Name, new OpenApiInfo
            {
                Title = definition.Title,
                Version = definition.Version,
                Description = definition.Description
            });
        }
    }

    private static void AddSecurity(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer",
            new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header, Description = "Please enter JWT with Bearer into field", Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                    {Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer"}},
                new string[] { }
            }
        });
    }

    private static void OrderApis(SwaggerGenOptions options)
    {
        options.OrderActionsBy(apiDesc =>
            $"{apiDesc.RelativePath}_{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
    }

    private static void TagApis(SwaggerGenOptions options)
    {
        options.TagActionsBy(tag =>
        {
            var urlParts = tag.RelativePath?.Split('/') ?? Array.Empty<string>();
            var titleTag = urlParts.Length > 2 && VersionRegex.IsMatch(urlParts[1])
                ? urlParts[2]
                : urlParts[1];
            return new List<string> { titleTag };
        });
    }

    private static void ApplyOperationFilters(SwaggerGenOptions options)
    {
        options.OperationFilter<ODataQueryParameterOperationFilter>();
    }

    private static void AddXmlComments(SwaggerGenOptions options)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var path = Path.GetDirectoryName(assembly.Location);
        var fileNames = Directory.EnumerateFiles(path!, $"{assembly.GetName().Name?.Split('.')[0]}.*.xml").ToList();
        fileNames.ForEach(x => options.IncludeXmlComments(x));
    }

    public static void UseIagSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var definition in SwaggerEndpointDefinitions)
            {
                options.SwaggerEndpoint($"/swagger/{definition.Name}/{definition.UrlEnd}", definition.Title);
            }
        });
    }
}