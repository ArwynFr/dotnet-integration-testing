using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace ArwynFr.IntegrationTesting.Tests.Target;

public class DummyDbContext : DbContext
{
    public DummyDbContext(DbContextOptions options) : base(options) => Expression.Empty();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
}