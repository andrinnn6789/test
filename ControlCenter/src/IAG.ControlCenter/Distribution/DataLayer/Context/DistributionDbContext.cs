using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.Configuration.Context;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.EntityFrameworkCore;

namespace IAG.ControlCenter.Distribution.DataLayer.Context;

[ContextInfo("Distribution")]
public class DistributionDbContext: ConfigCommonDbContext
{
    public DistributionDbContext(DbContextOptions<DistributionDbContext> options, IUserContext userContext)
        : base(options, userContext)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Release> Releases { get; set; }
    public DbSet<ReleaseFileStore> ReleaseFileStores { get; set; }
    public DbSet<FileStore> FileStores { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<ProductCustomer> ProductCustomers { get; set; }
    public DbSet<Installation> Installations { get; set; }
    public DbSet<Link> Links { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Releases)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductId);
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Installations)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductId);
        modelBuilder.Entity<Product>()
            .HasOne(p => p.DependsOn)
            .WithMany()
            .HasForeignKey(p => p.DependsOnProductId);

        modelBuilder.Entity<FileStore>()
            .HasMany(p => p.ReleaseFileStores)
            .WithOne(r => r.FileStore)
            .HasForeignKey(r => r.FileStoreId);

        modelBuilder.Entity<ReleaseFileStore>()
            .HasIndex(rfs => new { rfs.ReleaseId, rfs.FileStoreId }).IsUnique();
        modelBuilder.Entity<ReleaseFileStore>()
            .HasOne<Release>()
            .WithMany(r => r.ReleaseFileStores)
            .HasForeignKey(rfs => rfs.ReleaseId);

        modelBuilder.Entity<Customer>();
        modelBuilder.Entity<ProductCustomer>()
            .HasIndex(e => new {e.ProductId, e.CustomerId}).IsUnique();
        modelBuilder.Entity<ProductCustomer>()
            .HasOne(pc => pc.Product)
            .WithMany()
            .HasForeignKey(pc => pc.ProductId);
        modelBuilder.Entity<ProductCustomer>()
            .HasOne<Customer>()
            .WithMany(c => c.ProductCustomers)
            .HasForeignKey(pc => pc.CustomerId);

        modelBuilder.Entity<Installation>()
            .HasOne<Customer>()
            .WithMany()
            .HasForeignKey(i => i.CustomerId);

        modelBuilder.Entity<Link>();
    }
}