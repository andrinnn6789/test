using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.IdentityServer.Authentication;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace IAG.Infrastructure.Globalisation.Context;

[ContextInfo("Resource")]
public class ResourceContext : BaseDbContext
{
    [UsedImplicitly]
    public ResourceContext(DbContextOptions<ResourceContext> options, IUserContext userContext)
        : base(options, userContext)
    {
    }

    [UsedImplicitly]
    public DbSet<Culture> Cultures { get; set; }

    [UsedImplicitly]
    public DbSet<Model.Resource> Resources { get; set; }

    [UsedImplicitly]
    public DbSet<Translation> Translations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Culture>()
            .ToTable("Culture")
            .HasIndex(c => c.Name).IsUnique();
        modelBuilder.Entity<Model.Resource>()
            .ToTable("Resource")
            .HasIndex(r => r.Name).IsUnique();
        modelBuilder.Entity<Translation>()
            .ToTable("Translation")
            .HasIndex(t => new { t.CultureId, t.ResourceId}).IsUnique();
    }
}