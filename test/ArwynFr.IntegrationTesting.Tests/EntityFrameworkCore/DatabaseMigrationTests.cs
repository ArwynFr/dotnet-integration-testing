using ArwynFr.IntegrationTesting.Tests.Target;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting.Tests.EntityFrameworkCore;

public class DatabaseMigrationTests(ITestOutputHelper output) : IntegrationTestBase<Program, MigrationDbContext>(output)
{
    [Fact]
    public async Task PerTestStrategy_uses_migrations()
    {
        var migrations = await Database.Database.GetPendingMigrationsAsync();
        migrations.Should().BeEmpty();
    }

    protected override IDatabaseTestStrategy<MigrationDbContext> DatabaseTestStrategy
        => IDatabaseTestStrategy<MigrationDbContext>.DatabasePerTest;

    protected override void ConfigureDbContext(DbContextOptionsBuilder builder)
        => builder.UseSqlite($@"Data Source={Guid.NewGuid()}.sqlite");
}
