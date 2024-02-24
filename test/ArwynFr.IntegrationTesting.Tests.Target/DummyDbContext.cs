using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace ArwynFr.IntegrationTesting.Tests.Target;

public class DummyDbContext : DbContext
{
    public DummyDbContext(DbContextOptions<DummyDbContext> options) : base(options) => Expression.Empty();

    public DbSet<DummyEntity> Entities => Set<DummyEntity>();
}