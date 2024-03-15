using Microsoft.EntityFrameworkCore;

namespace ArwynFr.IntegrationTesting.Tests.Target;

public class MigrationDbContext(DbContextOptions<MigrationDbContext> options) : DbContext(options)
{
    public DbSet<DummyEntity> Entities => Set<DummyEntity>();
}
