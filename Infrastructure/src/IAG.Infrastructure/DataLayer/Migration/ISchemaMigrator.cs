
namespace IAG.Infrastructure.DataLayer.Migration;

interface ISchemaMigrator
{
    void DoMigration(IMigratorDbContext dbContext);
}