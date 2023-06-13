using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.System;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.Infrastructure.DataLayer.ObjectMapper;

[ExcludeFromCodeCoverage]
public class TenantMapper : ObjectMapper<Tenant, Tenant>
{
    protected override Tenant MapToDestination(Tenant source, Tenant destination)
    {
        destination.Id = source.Id;
        destination.Name = source.Name;
        return destination;
    }
}