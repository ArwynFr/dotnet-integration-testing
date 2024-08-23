using ArwynFr.IntegrationTesting.Tests.Target;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting.Tests.EntityFrameworkCore;

public class DatabaseDefaultStrategyTests(ITestOutputHelper output) : IntegrationTestBase<Program, DummyDbContext>(output)
{
    [Fact]
    public void DefaultStrategy_access_existing_database()
    {
        Database.Database.GetDbConnection().Should().NotBeNull();
    }

    public override async Task InitializeAsync()
    {
        await Database.Database.EnsureCreatedAsync();
        await base.InitializeAsync();
    }

    protected override void ConfigureDbContext(IServiceProvider services, DbContextOptionsBuilder builder)
        => builder.UseSqlite($@"Data Source={Guid.NewGuid()}.sqlite");
}
