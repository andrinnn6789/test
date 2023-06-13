using System.Composition;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

using IAG.IdentityServer.SeedImportExport;
using IAG.Infrastructure.DI;
using IAG.InstallClient.Authentication;
using IAG.InstallClient.Authorization;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace IAG.InstallClient.Startup;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
[Export(typeof(IConfigure))]
public class ConfigInstallClient : IConfigure
{
    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error/Error");
        }
        app.UseHttpsRedirection();
        var assembly = GetType().Assembly;
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new EmbeddedFileProvider(assembly, $"{assembly.GetName().Name}.wwwroot")
        });

        app.UseSession();

        app.UseStatusCodePages(context =>
        {
            if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized
                || context.HttpContext.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                BearerTokenCookieHandler.ClearBearerToken(context.HttpContext);
                context.HttpContext.Response.Redirect("Login");
            }

            return Task.CompletedTask;
        });
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });

        InstallerRoleCreator.CreateRole(app.ApplicationServices.GetRequiredService<IRealmSeedImporterExporter>());

        if (!Debugger.IsAttached)
        {
            app.PublishGlobalSettings();
        }
    }
}