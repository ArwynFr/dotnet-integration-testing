using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ArwynFr.IntegrationTesting.Tests.Target;

public class DummyDbContext : DbContext
{
    public DummyDbContext(DbContextOptions<DummyDbContext> options) : base(options) => Expression.Empty();

    public DbSet<DummyEntity> Entities => Set<DummyEntity>();
}
