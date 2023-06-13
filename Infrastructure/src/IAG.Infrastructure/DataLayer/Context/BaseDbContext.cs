using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

using IAG.Infrastructure.Crud;
using IAG.Infrastructure.DataLayer.Migration;
using IAG.Infrastructure.DataLayer.Model.Base;
using IAG.Infrastructure.DataLayer.Model.System;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.ObjectMapper;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IAG.Infrastructure.DataLayer.Context;

[ContextInfo("Base")]
public class BaseDbContext : DbContext, IMigratorDbContext
{
    protected IUserContext UserContext { get; }

    #region public

    public BaseDbContext(DbContextOptions options, IUserContext userContext) : base(options)
    {
        UserContext = userContext;
    }

    [UsedImplicitly]
    public DbSet<SchemaVersion> SchemaVersions { get; set; }

    [UsedImplicitly]
    public DbSet<SystemLog> SystemLogs { get; set; }

    [UsedImplicitly]
    public DbSet<Division> Divisions { get; set; }

    [UsedImplicitly]
    public DbSet<Tenant> Tenants { get; set; }

    public override int SaveChanges()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var now = DateTime.UtcNow;
            ((BaseEntity) entityEntry.Entity).ChangedOn = now;
            ((BaseEntity) entityEntry.Entity).ChangedBy = UserContext.UserName;

            if (entityEntry.State == EntityState.Added)
            {
                ((BaseEntity) entityEntry.Entity).CreatedOn = now;
                ((BaseEntity) entityEntry.Entity).CreatedBy = UserContext.UserName;
            }
            else
            {
                // set createdOn/createdBy to unmodified!
                entityEntry.Properties
                    .Single(p => p.Metadata.Name.Equals(nameof(BaseEntity.CreatedOn), StringComparison.InvariantCultureIgnoreCase))
                    .IsModified = false;
                entityEntry.Properties
                    .Single(p => p.Metadata.Name.Equals(nameof(BaseEntity.CreatedBy), StringComparison.InvariantCultureIgnoreCase))
                    .IsModified = false;
            }
        }

        return base.SaveChanges();
    }

    public void RemoveDeletedEntries<T>(IReadOnlyCollection<T> remoteEntities)
        where T : BaseEntity
    {
        var entitiesToRemove = new List<T>();
        foreach (var entry in Set<T>())
        {
            if (remoteEntities.Any(t => t.Id.Equals(entry.Id)))
                continue;
            entitiesToRemove.Add(entry);
        }

        foreach (var systemToRemove in entitiesToRemove)
            Set<T>().Remove(systemToRemove);
    }

    public void SyncLocalEntities<TRemote, TDest>(IReadOnlyCollection<TRemote> remoteEntities, IObjectMapper<TRemote, TDest> mapper)
        where TRemote : BaseEntity
        where TDest : BaseEntity, new()
    {
        foreach (var remoteEntity in remoteEntities)
        {
            SyncLocalEntity(remoteEntity, mapper);
        }
    }

    public void SyncLocalEntitiesByName<TRemote, TDest>(IReadOnlyCollection<TRemote> remoteEntities, IObjectMapper<TRemote, TDest> mapper)
        where TRemote : BaseEntity, IUniqueNamedEntity
        where TDest : BaseEntity, IUniqueNamedEntity, new()
    {
        foreach (var remoteEntity in remoteEntities)
        {
            var localEntity = Set<TDest>().FirstOrDefault(s => s.Name.Equals(remoteEntity.Name));
            AddOrUpdateEntity(mapper, localEntity, remoteEntity);
        }
    }

    public void SyncLocalEntity<TRemote, TDest>(TRemote remoteEntity, IObjectMapper<TRemote, TDest> mapper)
        where TRemote : IEntityKey<Guid>
        where TDest : BaseEntity, new()
    {
        var localEntity = Set<TDest>().FirstOrDefault(s => s.Id.Equals(remoteEntity.Id));
        AddOrUpdateEntity(mapper, localEntity, remoteEntity);
    }

    #endregion

    #region protected

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Tenant>()
            .HasIndex(c => c.Name).IsUnique();

        var dbAbstraction = new DatabaseAbstraction(Database.ProviderName);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            OnEntityModelCreating(entityType, dbAbstraction);
        }

        dbAbstraction.AddDbSpecificStuff(modelBuilder);
    }

    protected virtual void OnEntityModelCreating(IMutableEntityType entityType, DatabaseAbstraction dbAbstraction)
    {
        var tableAnnotation = entityType.ClrType.GetCustomAttributes(typeof(TableAttribute)).FirstOrDefault() as TableAttribute;
        // ReSharper disable once PossibleNullReferenceException
        var tableName = !string.IsNullOrEmpty(tableAnnotation?.Name) ? tableAnnotation.Name : GetTableName(entityType);

        entityType.SetTableName(tableName);

        dbAbstraction.AddDbEntitySpecificStuff(entityType);

        var propertyTenant = entityType.GetProperties().FirstOrDefault(p => p.Name == nameof(BaseEntityWithTenant.TenantId));
        if (!typeof(ITenantUniqueNamedEntity).IsAssignableFrom(entityType.ClrType))
            return;
        var propertyName = entityType.GetProperties().FirstOrDefault(p => p.Name == nameof(ITenantUniqueNamedEntity.Name));
        var index = entityType.AddIndex(new[] {propertyTenant, propertyName});
        index.IsUnique = true;
    }

    protected virtual string GetTableName(IMutableEntityType entityType)
    {
        var tableName = entityType.ClrType.Name;
        if (entityType.ClrType.IsSubclassOf(typeof(BaseEntity)))
        {
            int pos = tableName.IndexOf('`');
            if (pos > 0)
            {
                // class name without any generics info...
                tableName = tableName.Substring(0, pos);
            }
        }

        return tableName;
    }

    #endregion

    #region private

    private void AddOrUpdateEntity<TRemote, TDest>(IObjectMapper<TRemote, TDest> mapper, TDest localEntity, TRemote remoteEntity)
        where TDest : BaseEntity, new()
    {
        if (localEntity == null)
        {
            Set<TDest>().Add(mapper.NewDestination(remoteEntity));
        }
        else
        {
            mapper.UpdateDestination(localEntity, remoteEntity);
        }
    }

    #endregion
}