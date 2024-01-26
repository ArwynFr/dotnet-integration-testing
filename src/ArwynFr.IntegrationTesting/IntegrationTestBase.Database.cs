using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;

namespace ArwynFr.IntegrationTesting;

public abstract class IntegrationTestBase<TProgram, TContext> : IntegrationTestBase<TProgram>
where TProgram : class
where TContext : DbContext
{
    protected IntegrationTestBase(ITestOutputHelper output) : base(output) => Expression.Empty();

    protected TContext Database => Services.GetRequiredService<TContext>();

    protected virtual IDatabaseTestStrategy<TContext> DatabaseTestStrategy => IDatabaseTestStrategy<TContext>.Default;

    public override Task DisposeAsync() => DatabaseTestStrategy.DisposeAsync(Database);

    public override Task InitializeAsync() => DatabaseTestStrategy.InitializeAsync(Database);

    protected override void ConfigureAppServices(IServiceCollection services) => services.AddSingleton(new ServiceCollection()
        .AddDbContext<TContext>(ConfigureDbContext, ServiceLifetime.Singleton)
        .BuildServiceProvider().GetRequiredService<TContext>());

    protected abstract void ConfigureDbContext(DbContextOptionsBuilder builder);

}