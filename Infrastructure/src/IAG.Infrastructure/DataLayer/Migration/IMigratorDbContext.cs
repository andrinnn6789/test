using IAG.Infrastructure.DataLayer.Model.System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace IAG.Infrastructure.DataLayer.Migration;

public interface IMigratorDbContext
{
    DbSet<SchemaVersion> SchemaVersions { get; }

    DatabaseFacade Database { get; }

    int SaveChanges();
}