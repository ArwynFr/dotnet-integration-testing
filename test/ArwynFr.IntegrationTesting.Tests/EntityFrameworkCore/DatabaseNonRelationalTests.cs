using ArwynFr.IntegrationTesting.Tests.Target;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting.Tests.EntityFrameworkCore;

public class DatabaseNonRelationalTests(ITestOutputHelper output) : IntegrationTestBase<Program, DummyDbContext>(output)
{
    [Fact]
    public void PerTestStrategy_creates_non_relational_DB()
    {
        Database.Database.IsInMemory().Should().BeTrue();
    }

    protected override IDatabaseTestStrategy<DummyDbContext> DatabaseTestStrategy
        => IDatabaseTestStrategy<DummyDbContext>.DatabasePerTest;

    protected override void ConfigureDbContext(DbContextOptionsBuilder builder)
        => builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
}
