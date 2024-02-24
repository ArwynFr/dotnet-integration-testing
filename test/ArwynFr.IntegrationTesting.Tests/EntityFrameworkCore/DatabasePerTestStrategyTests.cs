using ArwynFr.IntegrationTesting.Tests.Target;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Xunit;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting.Tests.EntityFrameworkCore;

public class DatabasePerTestStrategyTests(ITestOutputHelper output) : IntegrationTestBase<Program, DummyDbContext>(output)
{
    [Fact]
    public void TestShouldCreateDatabase()
    {
        Database.Database.GetDbConnection().Should().NotBeNull();
    }

    protected override IDatabaseTestStrategy<DummyDbContext> DatabaseTestStrategy
        => IDatabaseTestStrategy<DummyDbContext>.DatabasePerTest;

    protected override void ConfigureDbContext(DbContextOptionsBuilder builder)
        => builder.UseSqlite($@"Data Source={Guid.NewGuid()}.sqlite");
}