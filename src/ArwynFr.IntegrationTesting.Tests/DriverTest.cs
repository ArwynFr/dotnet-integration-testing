using ArwynFr.IntegrationTesting.Tests.Target;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting.Tests;

public partial class DriverTest(ITestOutputHelper output) : IntegrationTestBase<Program, DummyDbContext>(output)
{
    [Theory, InlineData("random")]
    public async Task ExecuteAsync(string name)
    {
        await Services.GetRequiredService<DummyDriver>()
            .Given(x => x.ThereIsAnEntity(name))
            .When(x => x.FindEntityByName(name))
            .Then(x => x.ResultShouldBe(name))
            .ExecuteAsync();
    }

    protected override void ConfigureAppServices(IServiceCollection services)
    {
        base.ConfigureAppServices(services);
        services.AddSingleton(_ => new DummyDriver(Database, Client));
    }

    protected override void ConfigureDbContext(DbContextOptionsBuilder builder)
    => builder.UseSqlite($"Data Source={Guid.NewGuid()}.sqlite");

    protected override IDatabaseTestStrategy<DummyDbContext> DatabaseTestStrategy
    => IDatabaseTestStrategy<DummyDbContext>.DatabasePerTest;
}