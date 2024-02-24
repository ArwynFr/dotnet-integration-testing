using Microsoft.EntityFrameworkCore;

namespace ArwynFr.IntegrationTesting.Tests.Target;

public class MigrationDbContext : DbContext
{
    public MigrationDbContext(DbContextOptions<MigrationDbContext> options) : base(options) { }

    public DbSet<DummyEntity> Entities => Set<DummyEntity>();
}