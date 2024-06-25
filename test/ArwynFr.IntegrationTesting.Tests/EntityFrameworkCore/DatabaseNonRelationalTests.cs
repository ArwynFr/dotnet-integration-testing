using ArwynFr.IntegrationTesting.Tests.Target;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting.Tests.EntityFrameworkCore;

public class DatabaseNonRelationalTests(ITestOutputHelper output) : IntegrationTestBase<Program, MigrationDbContext>(output)
{
    [Fact]
    public void TestShouldCreateDatabase()
    {
        Database.Database.IsInMemory().Should().BeTrue();
    }

    protected override IDatabaseTestStrategy<MigrationDbContext> DatabaseTestStrategy
        => IDatabaseTestStrategy<MigrationDbContext>.DatabasePerTest;

    protected override void ConfigureDbContext(DbContextOptionsBuilder builder)
        => builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
}
