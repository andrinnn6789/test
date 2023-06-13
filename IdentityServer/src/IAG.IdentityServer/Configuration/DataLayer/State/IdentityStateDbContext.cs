using IAG.Infrastructure.DataLayer.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.IdentityServer.Authentication;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace IAG.IdentityServer.Configuration.DataLayer.State;

[UsedImplicitly]
[ContextInfo("IdentityServerState")]
public class IdentityStateDbContext : BaseDbContext
{
    public IdentityStateDbContext(DbContextOptions<IdentityStateDbContext> options, IUserContext userContext)
        : base(options, userContext)
    {
    }

    public DbSet<FailedRequestDb> FailedRequestEntries { get; set; }
    public DbSet<RefreshTokenDb> RefreshTokenEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<FailedRequestDb>().ToTable("FailedRequest");
        modelBuilder.Entity<RefreshTokenDb>().ToTable("RefreshToken");
    }
}