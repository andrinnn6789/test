using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;

using JetBrains.Annotations;

namespace IAG.IdentityServer.Plugin.UserDatabase.DataLayer.Model;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class Scope : BaseEntityWithTenant, ITenantUniqueNamedEntity
{
    public string Name { get; set; }
}