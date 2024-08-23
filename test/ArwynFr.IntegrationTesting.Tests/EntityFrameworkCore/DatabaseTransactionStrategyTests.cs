using ArwynFr.IntegrationTesting.Tests.Target;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting.Tests.EntityFrameworkCore;

public class DatabaseTransactionStrategyTests(ITestOutputHelper output) : IntegrationTestBase<Program, DummyDbContext>(output)
{
    [Fact]
    public void TransactionStrategy_runs_in_transaction()
    {
        Database.Database.CurrentTransaction.Should().NotBeNull();
    }

    public override async Task InitializeAsync()
    {
        await Database.Database.EnsureCreatedAsync();
        await base.InitializeAsync();
    }

    protected override IDatabaseTestStrategy<DummyDbContext> DatabaseTestStrategy
        => IDatabaseTestStrategy<DummyDbContext>.Transaction;

    protected override void ConfigureDbContext(IServiceProvider services, DbContextOptionsBuilder builder)
        => builder.UseSqlite($@"Data Source={Guid.NewGuid()}.sqlite");
}
