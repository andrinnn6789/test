using System.Composition;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DI;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace IAG.Infrastructure.Globalisation;

[ExcludeFromCodeCoverage]   // is covered in the integration tests
[UsedImplicitly]
[Export(typeof(IConfigure))]
public class ResourceConfigure : IConfigure
{
    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
    }
}