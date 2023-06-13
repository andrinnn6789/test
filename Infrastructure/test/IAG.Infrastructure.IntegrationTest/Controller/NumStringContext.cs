using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace IAG.Infrastructure.IntegrationTest.Controller;

public class NumStringContext : DbContext
{
    [ExcludeFromCodeCoverage]
    public NumStringContext()
    {
    }

    public NumStringContext(DbContextOptions<NumStringContext> options)
        : base(options)
    {
    }

    [UsedImplicitly]
    [ExcludeFromCodeCoverage]
    public DbSet<NumKey> NumKeys { get; set; }

    [UsedImplicitly]
    [ExcludeFromCodeCoverage]
    public DbSet<StringKey> StringKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<NumKey>().HasIndex("Name").IsUnique();
        modelBuilder.Entity<StringKey>().HasIndex("Name").IsUnique();
    }
}