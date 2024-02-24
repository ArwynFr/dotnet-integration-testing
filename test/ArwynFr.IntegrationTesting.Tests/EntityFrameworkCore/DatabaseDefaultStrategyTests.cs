using ArwynFr.IntegrationTesting.Tests.Target;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Xunit;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting.Tests.EntityFrameworkCore;

public class DatabaseDefaultStrategyTests(ITestOutputHelper output) : IntegrationTestBase<Program, DummyDbContext>(output)
{
    [Fact]
    public void TestShouldCreateDatabase()
    {
        Database.Database.GetDbConnection().Should().NotBeNull();
    }

    public override async Task InitializeAsync()
    {
        await Database.Database.EnsureCreatedAsync();
        await base.InitializeAsync();
    }

    protected override void ConfigureDbContext(DbContextOptionsBuilder builder)
        => builder.UseSqlite($@"Data Source={Guid.NewGuid()}.sqlite");
}