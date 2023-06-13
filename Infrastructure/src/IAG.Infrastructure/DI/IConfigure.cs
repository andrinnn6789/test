using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace IAG.Infrastructure.DI;

public interface IConfigure
{
    void Configure(IApplicationBuilder app, IHostEnvironment env);
}